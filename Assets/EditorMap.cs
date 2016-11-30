using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Pályaszerkesztő felület
/// </summary>
public class EditorMap : Editor {
    /// <summary>
    /// Ellenségbejegyzés a pályafájlban
    /// </summary>
    struct Enemy {
        /// <summary>
        /// Szélességi koordináta
        /// </summary>
        public int X;
        /// <summary>
        /// Magassági koordináta
        /// </summary>
        public int Y;
        /// <summary>
        /// Ellenség azonosítója
        /// </summary>
        public int EnemyID;
        /// <summary>
        /// Mozgásirány Y tengelyen
        /// </summary>
        public int Movement;
    }
    /// <summary>
    /// Ellenségek tömbje
    /// </summary>
    Enemy[] Enemies;
    /// <summary>
    /// Ellenségek megnyitáskori tömbje
    /// </summary>
    Enemy[] Original;
    /// <summary>
    /// Az összes ellenség első animációs fázisának modellje, azonosító alapján címezhető adatszerkezetben
    /// </summary>
    Dictionary<int, Objects.Obj> Models = new Dictionary<int, Objects.Obj>();

    /// <summary>
    /// Az Enemy struktúra Movement értékének megfelelő irányok vizuálisan szemléltetve
    /// </summary>
    readonly string[] Directions = new string[3] {"▲", "◄", "▼"};

    /// <summary>
    /// A kiválasztott ellenség ID létezik-e
    /// </summary>
    bool Exists = false;
    /// <summary>
    /// Mentés másként felület megjelenítése
    /// </summary>
    bool SaveAs = false;
    /// <summary>
    /// Jobbkattintva van-e bárhova
    /// </summary>
    bool RightClick = false;
    /// <summary>
    /// Megfogott és mozgatandó ellenség helye a tömbben
    /// </summary>
    int Grabbed = -1;
    /// <summary>
    /// Mentési azonosító
    /// </summary>
    string SaveAsID;
    /// <summary>
    /// Ellenségek adatait tartalmazó mappa
    /// </summary>
    string EnemyFolder;
    /// <summary>
    /// Objektumok adatait tartalmazó mappa
    /// </summary>
    string ObjectFolder;
    /// <summary>
    /// Előzőleg beírt ellenség (ellenség hozzáadása menü)
    /// </summary>
    string PrevEnemy = " ";
    /// <summary>
    /// Jelenleg beírt ellenség (ellenség hozzáadása menü)
    /// </summary>
    string TargetEnemy = "0";
    /// <summary>
    /// Mentés másként menü háttere
    /// </summary>
    Texture2D SaveBG;
    /// <summary>
    /// Egér elmozdulása (ellenségek áthelyezéséhez)
    /// </summary>
    Vector2 MouseMovement;
    /// <summary>
    /// Utolsó egérpozíció (elmozdulás számításához)
    /// </summary>
    Vector2 LastMousePos;
    /// <summary>
    /// Jobbklikkes menü megjelenítése ezen a pozíción
    /// </summary>
    Vector2 RightClickPos;
    /// <summary>
    /// A szerkesztőfelület görgetési állapota
    /// </summary>
    Vector2 Scrolling;

    /// <summary>
    /// Objektum betöltése ID alapján
    /// </summary>
    /// <param name="ID">Objektum ID</param>
    void LoadModel(int ID) {
        if (!Models.ContainsKey(ID)) { // Ha még nincs betöltve
            byte[] EnemyData = File.ReadAllBytes(EnemyFolder + ID.ToString() + ".dat"); // Olvassa be az ellenség adatait...
            Models.Add(ID, Objects.LoadObject(ObjectFolder + EnemyData[0].ToString() + ".dat")); // ...majd az abban található első animációs fázis modelljét
        }
    }

    /// <summary>
    /// Induláskor azonnal lefut
    /// </summary>
    void Start() {
        int AfterBackslash = Path.LastIndexOf('\\') + 1; // Fájlnév kezdetének helye a fájl elérési útvonalában
        SaveAsID = Path.Substring(AfterBackslash, Path.LastIndexOf('.') - AfterBackslash); // Fájlnév, azaz mentési azonosító
        EnemyFolder = Path.Substring(0, AfterBackslash - 1); // Fájlnév levágása
        EnemyFolder = EnemyFolder.Substring(0, EnemyFolder.LastIndexOf('\\')); // Gyökérmappáig visszavágás
        ObjectFolder = EnemyFolder + "\\objects\\"; // Objektumok mappába mozgás
        EnemyFolder += "\\enemies\\"; // Ellenségek mappába mozgás
        byte[] Data = File.ReadAllBytes(Path); // Kiválasztott pálya beolvasása, 0. bájt az ellenségek száma
        Enemies = new Enemy[Data[0]]; // Ellenségek tömbjének inicializálása
        Original = new Enemy[Data[0]]; // Eredeti ellenségtömb inicializálása
        int DataPointer = 0; // A vizsgált bájt száma
        for (byte i = 0; i < Data[0]; ++i) { // Ellenségeken végighaladás
            int ID; // Azonosító
            Enemies[i] = Original[i] = new Enemy { // Ellenség létrehozása mindkét tömbben, mert még az eredeti és a jelenlegi egyenlő
                X = Data[++DataPointer] * 256 + Data[++DataPointer], // Ellenségenként az első két bájt a szélességi pozíció, big endian elrendezésben
                Y = Data[++DataPointer], // A harmadik bájt a magassági pozíció
                EnemyID = ID = Data[++DataPointer], // A negyedik bájt az ellenség azonosítója
                Movement = Data[++DataPointer] // Az ötödik bájt a mozgás iránya a magasságtengelyen + 1
            };
            LoadModel(ID); // Töltse be az ellenség grafikáját
        }
        SaveBG = new Texture2D(1, 1); // Új 1x1-es textúra a mentés másként menü hátterének
        SaveBG.SetPixel(0, 0, new Color(0, 0, 0, .8f)); // Enyhén átlátszó fekete szín beállítása
        SaveBG.Apply(); // Színezés alkalmazása
    }

    /// <summary>
    /// Mentés
    /// </summary>
    /// <param name="Path">Fájlnév</param>
    void Save(string Path) {
        Array.Sort(Enemies, (a, b) => { int xCmp = a.X.CompareTo(b.X); return xCmp != 0 ? xCmp : a.Y.CompareTo(b.Y); }); // Rendezés pozíció, először is szélesség alapján
        BinaryWriter Writer = new BinaryWriter(File.Open(Path, FileMode.Create)); // Fájl megnyitása írásra
        Writer.Write((byte)Enemies.Length); // Ellenségek számának beírása
        for (int i = 0; i < Enemies.Length; ++i) { // Majd ellenségenként
            Writer.Write((byte)(Enemies[i].X / 256)); // Szélességi pozíció (big endian), nagyobb helyi érték
            Writer.Write((byte)(Enemies[i].X % 256)); // Szélességi pozíció (big endian), kisebb helyi érték
            Writer.Write((byte)Enemies[i].Y); // Magassági pozíció
            Writer.Write((byte)Enemies[i].EnemyID); // Ellenség azonosítója
            Writer.Write((byte)(Enemies[i].Movement)); // Mozgás iránya a magasságtengelyen + 1
        }
        Menu.ReloadMenu(); // Menü újramegnyitása, hogy egy esetleges új fájl megjelenjen
    }

    /// <summary>
    /// Mentés másként felület
    /// </summary>
    void SaveAsDialog() {
        GUI.DrawTexture(new Rect(300, 0, Screen.width, Screen.height), SaveBG); // A teljes háttér elsötétítése
        int HalfWidth = Screen.width / 2 + 150, HalfHeight = Screen.height / 2; // A fájlválasztókon kívüli rész szélességének és magasságának középpontja
        GUI.Box(new Rect(HalfWidth - 150, HalfHeight - 50, 300, 80), "Save as"); // Mentés másként doboz
        bool CanSave = Menu.IsNumeric(SaveAsID); // Lehet-e menteni (érvényes azonosító van-e megadva)
        GUI.color = CanSave ? Color.white : Color.red; // Az előzőtől függően legyen vörös a beviteli mezeje
        GUI.Label(new Rect(HalfWidth - 145, HalfHeight - 25, 30, 20), "ID:"); // Mezőnév
        SaveAsID = GUI.TextField(new Rect(HalfWidth - 115, HalfHeight - 25, 100, 20), SaveAsID); // Beviteli mező
        GUI.color = Color.white; // Ha vörösre lett változtatva, oldja fel
        GUI.enabled = CanSave; // A mentés gomb engedélyezése a beviteli mező érvényessége alapján
        if (GUI.Button(new Rect(HalfWidth - 145, HalfHeight, 100, 25), "Save")) { // Mentés gomb
            Save(Path.Substring(0, Path.LastIndexOf('\\') + 1) + SaveAsID + ".dat"); // Mentés végrehajtása
            SaveAs = false; // Mentés másként felület eltüntetése
        }
        GUI.enabled = true; // Esetleges gombtiltás feloldása
        if (GUI.Button(new Rect(HalfWidth + 45, HalfHeight, 100, 25), "Cancel")) { // Mégse
            int AfterBackslash = Path.LastIndexOf('\\') + 1; // Fájlnév kezdetének helye a fájl elérési útvonalában
            SaveAsID = Path.Substring(AfterBackslash, Path.LastIndexOf('.') - AfterBackslash); // Eredeti mentési azonosító visszaolvasása fájlnévből
            SaveAs = false; // Mentés másként felület eltüntetése
        }
    }

    /// <summary>
    /// GUI-kezelő függvény
    /// </summary>
    void OnGUI() {
        int LevelWidth = 84; // Szint szélessége, minimum 84, hogy ne legyen 0 széles a kép üres pályán
        int ScreenHeight = Screen.height - 120; // Kép magassága
        int Block = ScreenHeight / 48; // Blokkméret
        int EnemyCount = Enemies.Length; // Ellenségek száma
        for (int i = 0; i < EnemyCount; ++i) { // Ellenségeken végighaladva
            int BackDistance = Enemies[i].X + Models[Enemies[i].EnemyID].X; // Ellenség jobb széle
            if (LevelWidth <= BackDistance) // Ha az ellenség távolabb van, mint az eddigi legtávolabbi ismert pont
                LevelWidth = BackDistance + 1; // Legyen ott a pálya megjelenítésének vége, és maradjon mellette egy oszlop szabad hely
        }
        //////////////////////
        // Szint megjelenítése
        //////////////////////
        // Görgethető nézet kezdete
        Scrolling = GUI.BeginScrollView(new Rect(300, 0, Screen.width - 300, ScreenHeight + 20), Scrolling, new Rect(0, 0, LevelWidth * Block, ScreenHeight));
        Vector2 FixedMouse = new Vector2(Input.mousePosition.x - 300, Screen.height - Input.mousePosition.y) + Scrolling; // Javított egérpozíció
        for (int i = 0; i < EnemyCount; ++i) { // Ellenségeken végighaladva
            Objects.Obj Model = Models[Enemies[i].EnemyID]; // Az itteni ellenség megjelenése
            Rect EnemyBounds = new Rect(Enemies[i].X * Block, Enemies[i].Y * Block, Model.X * Block, Model.Y * Block); // Az itteni ellenség határai
            if (GUI.Button(new Rect(EnemyBounds.xMax, EnemyBounds.yMin + 25, 20, 20), Directions[Enemies[i].Movement])) // Mozgásirány gomb
                Enemies[i].Movement = (Enemies[i].Movement + 1) % 3; // Mozgásirány váltása
            if (GUI.Button(new Rect(EnemyBounds.xMax, EnemyBounds.yMin, 20, 20), "X")) { // Törlés gomb
                int NewEnemyCount = Enemies.Length - 1; // Ellenségek új száma
                Enemies[i] = Enemies[NewEnemyCount]; // A tömbben rakja az utolsót a jelenlegi helyre...
                Array.Resize(ref Enemies, NewEnemyCount); // ...majd vágja le a végét
                --EnemyCount; // Ellenségszám csökkentése, hogy a későbbiekben ne legyen túlhivatkozás
            }
            if (Input.GetMouseButtonDown(0) && EnemyBounds.Contains(FixedMouse)) { // Ha az ellenségre lett kattintva:
                Grabbed = i; // Megfogás
                LastMousePos = Input.mousePosition; // Utolsó egérpozíció a megfogás helyén
                MouseMovement = new Vector2(0, 0); // Elmozdulás nullázása
            }
            Graphics.Draw(ref Model, EnemyBounds); // Ellenség kirajzolása
        }
        ///////////////////
        // Jobbklikkes menü
        ///////////////////
        if (RightClick) { // Ha meg van nyitva a jobbklikkes menü
            Rect BoxRect = new Rect(RightClickPos, new Vector2(200, 80)); // A menü határai
            GUI.Box(BoxRect, "New enemy"); // Doboz rajzolása a menünek
            if (PrevEnemy.CompareTo(TargetEnemy) != 0) { // Ha az előző képkocka óta változott a beírt ellenség azonosítója
                Exists = File.Exists(EnemyFolder + TargetEnemy.ToString() + ".dat"); // Létezik-e ilyen fájl
                PrevEnemy = TargetEnemy; // Az előző beírt ellenség legyen a jelenlegi
            }
            bool Okay = GUI.enabled = Menu.IsNumeric(TargetEnemy); // Érvényes azonosító van-e megadva, attól függően legyenek engedélyezve a gombok
            GUI.color = Okay ? Color.white : Color.red; // Ha érvénytelen azonosító van megadva, legyen vörös a beviteli mezeje
            if (GUI.Button(new Rect(RightClickPos.x + 5, RightClickPos.y + 25, 20, 20), "-")) // Csökkentés gomb
                TargetEnemy = (Convert.ToInt32(TargetEnemy) - 1).ToString(); // Azonosító csökkentése
            GUI.enabled = true; // Az azonosítóbevitel semmiképp ne legyen letiltva
            TargetEnemy = GUI.TextField(new Rect(RightClickPos.x + 30, RightClickPos.y + 25, 70, 20), TargetEnemy); // Azonosító beviteli mezeje
            GUI.enabled = Okay; // Ha érvényes azonosító van megadva, engedélyezze a további gombokat
            if (GUI.Button(new Rect(RightClickPos.x + 105, RightClickPos.y + 25, 20, 20), "+")) // Növelés gomb
                TargetEnemy = (Convert.ToInt32(TargetEnemy) + 1).ToString(); // Azonosító növelése
            GUI.color = (GUI.enabled = Exists) ? Color.white : Color.red; // A hozzáadás gomb akkor legyen megnyomható és nem vörös, ha létezik a beírt ellenség
            if (GUI.Button(new Rect(RightClickPos.x + 130, RightClickPos.y + 25, 65, 20), "Add")) { // Hozzáadás gomb
                int NewEntry = Enemies.Length; // Új azonosító helye a tömbben
                int ID = Convert.ToInt32(TargetEnemy); // Az ellenség azonosítója számra konvertálva
                Array.Resize(ref Enemies, NewEntry + 1); // Új hely létrehozása a tömbben
                Enemies[NewEntry] = new Enemy() { // Az új helyre hozza létre az új ellenséget:
                    X = (int)RightClickPos.x / Block, // A kattintás szélességi pozícióján
                    Y = (int)RightClickPos.y / Block, // A kattintás magassági pozícióján
                    EnemyID = ID, // A megadott azonosítóval
                    Movement = 0, // Függőleges mozgás nélkül
                };
                LoadModel(ID); // Grafika betöltése az új ellenséghez
                RightClick = false; // Jobbklikkes menü bezárása
            }
            GUI.enabled = true; // A felület legyen engedélyezve a továbbiakban...
            GUI.color = Color.white; // ...az eredeti színében
            if (GUI.Button(new Rect(RightClickPos.x + 5, RightClickPos.y + 50, 190, 20), "Cancel") || // A mégse gombra nyomva...
                (Input.GetMouseButtonDown(0) && !BoxRect.Contains(FixedMouse))) // ...vagy a doboz mellé kattintva...
                RightClick = false; // ...záródjon be a hozzáadó.
        } else if (Input.GetMouseButtonDown(1)) { // Megnyomott jobb egérgomb esetén
            RightClick = true; // Jobbklikkes menü megnyitása...
            RightClickPos = FixedMouse; // ...az egér jelenlegi pozícióján
        }
        GUI.EndScrollView(); // Görgethető nézet vége
        ///////////
        // Mozgatás
        ///////////
        if (Grabbed != -1) { // Ha van megfogott ellenség
            Vector2 MouseDelta = new Vector2(Input.mousePosition.x - LastMousePos.x, LastMousePos.y - Input.mousePosition.y); // Egérelmozdulás az előző képkockához képest
            LastMousePos = Input.mousePosition; // Utolsó egérpozíció frissítése
            MouseMovement += MouseDelta; // Összesített elmozduláshoz adás
            if (Mathf.Abs(MouseMovement.x) > Block) { // Ha elmozdult egy blokknyit szélességben
                int Step = Mathf.FloorToInt(MouseMovement.x / Block); // Ennyi blokknyit mozdult odébb
                Enemies[Grabbed].X += Step; // Odébbmozgatás
                MouseMovement.x -= Block * Step; // Az összesített elmozdulásból vonódjon le ez a lépés
            }
            if (Mathf.Abs(MouseMovement.y) > Block) { // Ha elmozdult egy blokknyit magasságban
                int Step = Mathf.FloorToInt(MouseMovement.y / Block); // Ennyi blokknyit mozdult odébb
                Enemies[Grabbed].Y += Step; // Odébbmozgatás
                MouseMovement.y -= Block * Step; // Az összesített elmozdulásból vonódjon le ez a lépés
            }
            if (Input.GetMouseButtonUp(0)) // Ha felengedte a felhasználó az egérgombot...
                Grabbed = -1; // ...álljon le a mozgás
        }
        ////////////
        // Alsó menü
        ////////////
        int Left = Screen.width / 2 + 50; // Bal oldali behúzás (a szerkesztőfelület közepe)
        TextAnchor OldLabelAlign = GUI.skin.label.alignment; // Régi címkeigazítás
        GUI.skin.label.alignment = TextAnchor.MiddleCenter; // A címkéket írja közép-középre
        GUI.Label(new Rect(Left, ScreenHeight += 20, 200, 25), "Right click to add enemy."); // Információ közlése a jobb klikkes menü létezéséről
        GUI.skin.label.alignment = OldLabelAlign; // Címkeigazítás visszaállítása
        if (GUI.Button(new Rect(Left, ScreenHeight += 25, 200, 25), "Revert")) { // Visszaállítás gomb
            int OldLength = Original.Length; // Eredeti ellenségek száma
            Enemies = new Enemy[OldLength]; // Új ellenségtömb, az eredeti méretben
            Array.Copy(Original, Enemies, OldLength); // Eredeti ellenségek visszamásolása
        }
        if (GUI.Button(new Rect(Left, ScreenHeight += 25, 200, 25), "Save")) // Mentés gomb
            Save(Path); // Eredetileg megnyitott pálya felülírása
        if (GUI.Button(new Rect(Left, ScreenHeight += 25, 200, 25), "Save as")) // Mentés másként gomb
            SaveAs = true; // Mentés másként felület megjelenítése
        if (SaveAs) // Ha meg van nyitva a mentés másként felület...
            SaveAsDialog(); // ...jelenítse meg
    }
}