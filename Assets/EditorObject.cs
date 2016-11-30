using UnityEngine;
using System;
using System.IO;
using System.Text;

/// <summary>
/// Objektumszerkesztő felület
/// </summary>
public class EditorObject : Editor {
    /// <summary>
    /// Szerkesztés alatt lévő kép
    /// </summary>
    public Objects.Obj Image = new Objects.Obj();
    /// <summary>
    /// Eredeti kép
    /// </summary>
    public Objects.Obj OriginalImage = new Objects.Obj();

    /// <summary>
    /// Mentés másként felület megjelenítése
    /// </summary>
    bool SaveAs = false;
    /// <summary>
    /// Hardcode felület megjelenítése
    /// </summary>
    bool Hardcode = false;
    /// <summary>
    /// Mentési azonosító
    /// </summary>
    string SaveAsID;
    /// <summary>
    /// Hardcode
    /// </summary>
    string HardcodeOut;
    /// <summary>
    /// Mentés másként menü háttere
    /// </summary>
    Texture2D SaveBG;

    /// <summary>
    /// Induláskor azonnal lefut
    /// </summary>
    void Start() {
        int AfterBackslash = Path.LastIndexOf('\\') + 1; // Fájlnév kezdetének helye a fájl elérési útvonalában
        SaveAsID = Path.Substring(AfterBackslash, Path.LastIndexOf('.') - AfterBackslash); // Fájlnév, azaz mentési azonosító
        Image = Objects.LoadObject(Path); // Kép betöltése a megadott helyről
        OriginalImage = Objects.CopyObject(Image); // Eredeti állapot eltárolása
        SaveBG = new Texture2D(1, 1); // Új 1x1-es textúra a mentés másként menü hátterének
        SaveBG.SetPixel(0, 0, new Color(0, 0, 0, .8f)); // Enyhén átlátszó fekete szín beállítása
        SaveBG.Apply(); // Színezés alkalmazása
    }

    /// <summary>
    /// Mentés
    /// </summary>
    /// <param name="SavePath">Célfájlnév</param>
    void Save(string SavePath) {
        int Pixels = Image.X * Image.Y; // Pixelek száma
        int FileLength = Pixels / 8 + (Pixels % 8 != 0 ? 3 : 2); // Tömörített pixelek száma + bájtok a méretnek
        byte[] Bytes = new byte[FileLength]; // Fájlba írandó tartalom helye
        Bytes[0] = (byte)Image.X; // 0. bájt: szélesség
        Bytes[1] = (byte)Image.Y; // 1. bájt: magasság
        int Byte = 1; // Jelenleg írás alatt lévő bájt (a következő ciklus első lépésként 2-re löki)
        for (int Pixel = 0; Pixel < Pixels; ++Pixel) { // Minden pixel tömörítése
            if (Pixel % 8 == 0) // Ha 8 pixellel végzett...
                ++Byte; // ...nyisson új bájtot
            Bytes[Byte] <<= 1; // Hely szorítása a jelenlegi bájtban a következő pixelnek
            if (Image.Image[Pixel] == 1) // Ha aktív a vizsgált pixel...
                ++Bytes[Byte]; // ...a jelenlegi bájtba írja bele
        }
        File.WriteAllBytes(SavePath, Bytes); // Fájlba írás
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
    /// Hardcode felület
    /// </summary>
    void HardcodeDialog() {
        GUI.DrawTexture(new Rect(300, 0, Screen.width, Screen.height), SaveBG); // A teljes háttér elsötétítése
        int HalfWidth = Screen.width / 2 + 150, HalfHeight = Screen.height / 2; // A fájlválasztókon kívüli rész szélességének és magasságának középpontja
        GUI.Box(new Rect(HalfWidth - 150, HalfHeight - 50, 300, 100), "Hardcode"); // Hardcode doboz
        GUI.TextArea(new Rect(HalfWidth - 145, HalfHeight - 25, 290, 40), HardcodeOut); // Hardcode megjelenítése egy szövegdobozban, hogy másolható legyen
        if (GUI.Button(new Rect(HalfWidth + 45, HalfHeight + 20, 100, 25), "OK")) // OK gomb
            Hardcode = false; // Hardcode felület eltüntetése
    }

    /// <summary>
    /// GUI-kezelő függvény
    /// </summary>
    void OnGUI() {
        //////////////////////////
        // Pixelenkénti szerkesztő
        //////////////////////////
        // Ugyanazok vannak itt kiszámolva, mint a Graphics.Draw-n belül, hogy a keretet köré lehessen rajzolni
        int MaxWidth = Screen.width - 300, MaxHeight = Screen.height - 100, WScale = MaxWidth / Image.X, HScale = MaxHeight / Image.Y, Scale = Math.Min(WScale, HScale),
            Left = 300 + (MaxWidth - Image.X * Scale) / 2, Top = (MaxHeight - Image.Y * Scale) / 2, Width = Image.X * Scale, Height = Image.Y * Scale;
        Graphics.Draw(ref Image, new Rect(300, 0, MaxWidth, MaxHeight), !SaveAs && !Hardcode); // Szerkesztő megjelenítése
        GUI.DrawTexture(new Rect(Left, Top, Width, 1), Graphics.Foreground); // Felső keret
        GUI.DrawTexture(new Rect(Left, Top, 1, Height), Graphics.Foreground); // Bal keret
        GUI.DrawTexture(new Rect(Left + Width - 1, Top, 1, Height), Graphics.Foreground); // Jobb keret
        GUI.DrawTexture(new Rect(Left, Top + Height - 1, Width, 1), Graphics.Foreground); // Alsó keret
        //////////////
        // Információk
        //////////////
        Top = Screen.height - 100; // Alsó behúzás
        GUI.color = Graphics.ForegroundColor; // Előtérszín alkalmazása a GUI-ra
        StringBuilder Info = new StringBuilder(); // Információ szövegének tárolója
        Info.Append(SaveAsID).Append(" (").Append(Image.X).Append('x').Append(Image.Y).Append(')'); // Információ összeírása
        GUI.Label(new Rect(305, Top + 5, 200, 20), Info.ToString()); // Információ megjelenítése
        //////////////
        // Átméretezés
        //////////////
        int OldX = Image.X, OldY = Image.Y; // Régi méretek
        GUI.Label(new Rect(305, Top + 25, 55, 20), "Width:"); // Szélesség címke
        Image.X = (int)Graphics.Slider(new Rect(360, Top + 29, 200, 20), Image.X, 1, 84); // Szélesség csúszka
        GUI.Label(new Rect(305, Top + 45, 55, 20), "Height:"); // Magasság címke
        Image.Y = (int)Graphics.Slider(new Rect(360, Top + 49, 200, 20), Image.Y, 1, 48); // Magasság csúszka
        if (Image.X != OldX) { // Ha változott a szélesség
            int OldPixel = 0; // Adott pixel régi helye
            int Pixel = 0; // Adott pixel jelenlegi helye
            int WidthDiff = OldX - Image.X; // Szélességkülönbség
            byte[] OldImage = new byte[84 * 48]; // Régi pixeltérkép helye
            Array.Copy(Image.Image, OldImage, 84 * 48); // Másolat létrehozása
            for (int Row = 0; Row < Image.Y; ++Row) { // A jelnlegi méret sorain...
                for (int Column = 0; Column < Image.X; ++Column) { // ...majd oszlopain (tehát a pixelek sorrendjében) haladva...
                    Image.Image[Pixel++] = Column >= OldX ? (byte)0 : OldImage[OldPixel]; // ...az odaillő pixel beillesztése, ami a régi szélességen túl biztos nem volt aktív
                    ++OldPixel; // A régi pixeltömb következő elemére ugrás
                }
                OldPixel += WidthDiff; // A régi pixeltömbben is a következő sor elejére kerüljön a vizsgálat
            }
            int LastPixel = Image.X * Image.Y, OldLastPixel = OldX * Image.Y; // Új, illetve régi utolsó pixel
            for (Pixel = LastPixel - 1; ++Pixel < OldLastPixel; Image.Image[Pixel] = 0) ; // Az előző sor pontjai közti részt ürítse ki, ha esetleg újra nőne a szélesség
        }
        if (Image.Y != OldY) { // Ha változott a magasság
            int LastPixel = Image.X * Image.Y, OldLastPixel = Image.X * OldY; // Új, illetve régi utolsó pixel
            for (int Pixel = LastPixel - 1; ++Pixel < OldLastPixel; Image.Image[Pixel] = 0) ; // Az előző sor pontjai közti részt ürítse ki, ha esetleg újra nőne a magasság
        }
        /////////
        // Opciók
        /////////
        int WidthJump = (Screen.width - 300) / 2 - 100; // Ugrás mérete az alsó oszlopokkal
        Left = 300 + WidthJump; // Behúzás középre
        if (GUI.Button(new Rect(Left, Top + 10, 200, 25), "Clear")) // Ürítés gomb
            Array.Clear(Image.Image, 0, Image.X * Image.Y); // Tömb nullázása
        if (GUI.Button(new Rect(Left, Top + 40, 200, 25), "Revert")) // Visszaállítás gomb
            Image = Objects.CopyObject(OriginalImage); // Eredeti objektum visszaállítása
        if (GUI.Button(new Rect(Left, Top + 70, 200, 25), "Crop")) { // Levágás gomb
            int CropTop = 48, CropLeft = 84, CropRight = 0, CropBottom = 0; // Levágási határok
            int Pixels = Image.X * Image.Y; // Pixelek száma
            bool Work = false; // Legyen-e vágás
            for (int Pixel = 0; Pixel < Pixels; ++Pixel) { // Minden pixelt ellenőrizzen
                if (Image.Image[Pixel] == 1) { // Ha egy pixel aktív
                    int Row = Pixel / Image.X, Column = Pixel % Image.X; // Sor és oszlop meghatározása
                    if (CropTop > Row) // Ha feljebb van, mint eddig bármi...
                        CropTop = Row; // ...terjessze ki addig a levágást
                    if (CropLeft > Column) // Ha balrább van, mint eddig bármi...
                        CropLeft = Column; // ...terjessze ki addig a levágást
                    if (CropRight < Column) // Ha jobbrább van, mint eddig bármi...
                        CropRight = Column; // ...terjessze ki addig a levágást
                    if (CropBottom < Row) // Ha lejjebb van, mint eddig bármi...
                        CropBottom = Row; // ...terjessze ki addig a levágást
                    Work = true; // Legyen vágás
                }
            }
            if (Work) { // Ha van aktív pixel
                byte[] NewMap = new byte[84 * 48]; // Új pixeltérkép
                int NewWidth = CropRight - CropLeft + 1, NewHeight = CropBottom - CropTop + 1; // Új szélesség és magasság
                int NewPixel = 0; // Adott pixel új helye
                for (int Row = 0; Row < NewHeight; ++Row) // Előbb soronként...
                    for (int Column = 0; Column < NewWidth; ++Column) // ...majd oszloponként bejárás (ez a bájtsorrend)
                        NewMap[NewPixel++] = Image.Image[(CropTop + Row) * Image.X + CropLeft + Column]; // Pixelek új helyükre mozgatása
                Image.X = NewWidth; // Szélesség frissítése
                Image.Y = NewHeight; // Magasság frissítése
                Image.Image = NewMap; // Pixeltérkép frissítése
            }
        }
        /////////
        // Mentés
        /////////
        Left += WidthJump - 5; // Behúzás a jobb szélre
        if (GUI.Button(new Rect(Left, Top + 10, 200, 25), "Save")) // Mentés gomb
            Save(Path); // Eredetileg megnyitott objektum felülírása
        if (GUI.Button(new Rect(Left, Top + 40, 200, 25), "Save as")) // Mentés másként gomb
            SaveAs = true; // Mentés másként felület megjelenítése
        if (GUI.Button(new Rect(Left, Top + 70, 200, 25), "Hardcode")) { // Hardcode gomb
            int Size = Image.X * Image.Y; // Képméret
            StringBuilder Out = new StringBuilder("[").Append(Size).Append("] = {"); // Kimenet, tömbformátumban
            for (int Pixel = 0; Pixel < Size; ++Pixel) // Minden pixelt...
                Out.Append(Image.Image[Pixel] == 0 ? '0' : '1').Append(','); // ...adjon hozzá, vesszővel elválasztva
            HardcodeOut = Out.Remove(Out.Length - 1, 1).Append('}').ToString(); // Utolsó vessző levágása, tömb lezárása
            Hardcode = true; // Hardcode felület megjelenítése
        }
        GUI.color = Color.white; // GUI színének visszaállítása
        ////////////
        // Felületek
        ////////////
        if (SaveAs) // Ha meg van nyitva a mentés másként felület...
            SaveAsDialog(); // ...jelenítse meg
        if (Hardcode) // Ha meg van nyitva a hardcode felület...
            HardcodeDialog(); // ...jelenítse meg
    }
}