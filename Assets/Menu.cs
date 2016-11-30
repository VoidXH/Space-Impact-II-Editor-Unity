using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Főmenü és szerkesztőfelület-kezelő komponens
/// </summary>
public class Menu : MonoBehaviour {
    /// <summary>
    /// A játék adatmappája
    /// </summary>
    public static string DataFolder;

    /// <summary>
    /// Ellenségválasztó
    /// </summary>
    FilePicker enemies = new FilePicker();
    /// <summary>
    /// Szintválasztó
    /// </summary>
    FilePicker levels = new FilePicker();
    /// <summary>
    /// Objektumválasztó
    /// </summary>
    FilePicker objects = new FilePicker();

    /// <summary>
    /// Induláskor azonnal lefut
    /// </summary>
	void Start() {
        int BlockSize = Screen.height / 3; // Egy fájlválasztó mérete
        // A következő mappák valójában kisbetűsek, de a kisbetűs ablakcím gány, és a fájlrendszernek nem baj
        enemies.FullLocation = DataFolder + "\\data\\Enemies"; // Ellenségek mappája
        enemies.Show(); // Töltse be
        enemies.Position = new Rect(0, 0, 300, BlockSize); // A bal felső sarokban jelenjen meg
        enemies.EnableFolderSwitch = false; // Ne lehessen mappát váltani benne
        ProcessFolder(enemies); // Esetleges nem oda való fájlok eltüntetése a listáról
        levels.FullLocation = DataFolder + "\\data\\Levels"; // Szintek mappája
        levels.Show(); // Töltse be
        levels.Position = new Rect(0, BlockSize, 300, BlockSize); // Az előző alján jelenjen meg
        levels.EnableFolderSwitch = false; // Ne lehessen mappát váltani benne
        ProcessFolder(levels); // Esetleges nem oda való fájlok eltüntetése a listáról
        BlockSize *= 2; // Két blokk alatt jelenjen meg
        objects.FullLocation = DataFolder + "\\data\\Objects"; // Objektumok mappája
        objects.Show(); // Töltse be
        objects.Position = new Rect(0, BlockSize, 300, Screen.height - BlockSize); // Legalul legyen meg, és ha az ablakmagasság nem osztható hárommal, a maradék pixelt pótolja ki
        objects.EnableFolderSwitch = false; // Ne lehessen mappát váltani benne
        ProcessFolder(objects); // Esetleges nem oda való fájlok eltüntetése a listáról
    }

    /// <summary>
    /// Menü újramegnyitása (pl. frissítés indokával)
    /// </summary>
    public static void ReloadMenu() {
        Menu menu = FindObjectOfType<Menu>(); // A jelenlegi menü
        GameObject MenuObject = menu.gameObject; // A menüt hordozó objektum
        Destroy(menu); // A jelenlegi menü bezárása
        MenuObject.AddComponent<Menu>(); // Új menü nyitása a menühordozón
    }

    /// <summary>
    /// Egy szöveg ellenőrzése, hogy pozitív egész számot tartalmaz-e
    /// </summary>
    /// <param name="s">Ellenőrizendő szöveg</param>
    /// <returns>Igaz, ha a szöveg pozitív egész számot tartalmaz</returns>
    public static bool IsNumeric(string s) {
        int l = s.Length; // Bemeneti szöveg hossza
        for (int i = 0; i < l; ++i) // A szöveg minden karakterére ellenőrizze...
            if (s[i] < '0' || s[i] > '9') // ...hogy szám-e...
                return false; // ...mert ha bármi más, a vizsgálandó állítás megdőlt
        return true; // Ha nem dőlt meg az állítás, igaz
    }

    /// <summary>
    /// A mappák számnevű fájlainak kiválasztását segítő struktúra
    /// </summary>
    struct FileProcessor {
        public int Number; // A fájl neve számként, hogy a rendezés ne bolonduljon meg
        public FileInfo FileEntry; // Fájlbejegyzés
    }

    /// <summary>
    /// Nem számnevű fájlok eltávolítása egy fájlválasztóból
    /// </summary>
    /// <param name="fp">Fájlválasztó</param>
    void ProcessFolder(FilePicker fp) {
        FileProcessor[] Files = new FileProcessor[fp.Files.Length]; // A megjelenítendő fájlokat tartalmazó tömb, elég hellyel
        int l = fp.Files.Length; // Fájlok száma
        int i = 0; // Vizsgált elem sorszáma
        while (l-- != 0) { // Amíg van szám
            string NumOnly = fp.Files[l].Name.Substring(0, fp.Files[l].Name.Length - 4); // A fájl nevének kiterjesztésmentes része
            if (IsNumeric(NumOnly)) { // Amennyiben ez szám...
                Files[i++] = new FileProcessor() { // ...vegye hozzá a megjelenítendő fájlokhoz
                    Number = Convert.ToInt32(NumOnly), // Fájlnév, számmá konvertálva
                    FileEntry = fp.Files[l] // Fájlbejegyzés
                };
            }
        }
        Array.Resize(ref Files, i); // Vágja le az utolsó, üres elemeket
        Array.Sort(Files, (a, b) => { return a.Number.CompareTo(b.Number); }); // Rendezze azonosító alapján a fájlokat
        fp.Files = new FileInfo[i]; // Cserélje le a fájlválasztó által betöltött fájlokat...
        for (int c = 0; c < i; c++)
            fp.Files[c] = Files[c].FileEntry; // ...az előbb kigyűjtött és rendezett fájlokra
    }

    /// <summary>
    /// GUI-kezelő függvény
    /// </summary>
	void OnGUI() {
        enemies.OnGUI(1); // Ellenségválasztó megjelenítése
        levels.OnGUI(2); // Szintválasztó megjelenítése
        objects.OnGUI(3); // Objektumválasztó megjelenítése
	}

    /// <summary>
    /// Minden képkuckára egyszer lefutó függvény
    /// </summary>
    void Update() {
        if (enemies.Picked != null) { // Ha van kiválasztott ellenség:
            gameObject.AddComponent<EditorEnemy>().Path = enemies.Picked.FullName; // Hozza létre a kiválasztott fájlt megnyitva az új szerkesztőt
            enemies.Picked = null; // A választás törlése, hogy a következő képkockában ne fusson ez le
        }
        if (levels.Picked != null) { // Ha van kiválasztott szint:
            gameObject.AddComponent<EditorMap>().Path = levels.Picked.FullName; // Hozza létre a kiválasztott fájlt megnyitva az új szerkesztőt
            levels.Picked = null; // A választás törlése, hogy a következő képkockában ne fusson ez le
        }
        if (objects.Picked != null) { // Ha van kiválasztott objektum:
            gameObject.AddComponent<EditorObject>().Path = objects.Picked.FullName; // Hozza létre a kiválasztott fájlt megnyitva az új szerkesztőt
            objects.Picked = null; // A választás törlése, hogy a következő képkockában ne fusson ez le
        }
    }

    /// <summary>
    /// Kilépéskor meghívódó függvény
    /// </summary>
    void OnApplicationQuit() {
        PlayerPrefs.SetString("Path", DataFolder); // Mentse el a jelenlegi mappát, hogy innen nyíljon meg a szerkesztő legközelebb
        PlayerPrefs.Save(); // Bizonyosodjon meg róla, hogy el van mentve
    }
}