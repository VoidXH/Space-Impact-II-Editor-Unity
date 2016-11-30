using UnityEngine;
using System;
using System.IO;

/// <summary>
/// A játék adatmappájának megkeresését segítő komponens
/// </summary>
public class Loader : MonoBehaviour {
#region Indítási tömörítés
#if UNITY_EDITOR // Csak szerkesztőben legyen ilyen
    [Tooltip("Indításkor beolvasott és tömörítendő pixeltérkép, ami 0 vagy 1 elemekből állhat. " +
        "Mindenképp annyi eleme (vagy 8-cal osztható számmal kevesebb) legyen, ahány pixele van a bementi térképnek (struktúrakövetelmény).")]
    public string Compress;

    /// <summary>
    /// A Compress string-ben megadott tömörítetlen tömböt tömöríti
    /// </summary>
    void DoCompress() {
        if (Compress == "") // Ha nincs mit tömöríteni...
            return; // ...ne fusson le
        Compress = Compress.Replace(",", "").Replace(" ", "").Replace("{", "").Replace("}", ""); // Körbeölelő karakterekkel való bemásolás ne okozzon gondot
        int Pixels = Compress.Length; // Pixelek száma a bemásolt szövegben = a bemásolt szöveg hossza
        int FileLength = Pixels / 8 + (Pixels % 8 != 0 ? 3 : 2); // Tömörített adattömb bájtjainak száma
        byte[] Bytes = new byte[FileLength]; // Tömörített adattömb
        int Byte = -1; // A tömörített adattömb vizsgált eleme (azért -1, mert a következő for első lépése 0-ra löki)
        for (int Pixel = 0; Pixel < Pixels; ++Pixel) { // Minden pixelre
            if (Pixel % 8 == 0) // Ha a jelenlegi bájt megtelt...
                ++Byte; // ...lépjen új bájtra
            Bytes[Byte] <<= 1; // A jelenlegi bájtban szorítson új helyet az utolsó biten
            if (Compress[Pixel] == '1') // Ha a vizsgált pixel aktív...
                ++Bytes[Byte]; // ...a helyére igaz bitet rakjon
        }
        string o = "[" + (Byte + 1) + "->" + Pixels + "] "; // A kimenet szemléltesse a következőket, a megadott módon: [tömörített méret -> kibontott méret]
        for (int i = 0; i <= Byte; ++i) // Minden elemet írjon ki...
            o += Bytes[i].ToString() + ','; // ...vesszővel elválasztva...
        Debug.Log(o.Substring(0, o.Length - 1)); // ...ez viszont az utolsó elem után is írna egy vesszőt, azt vágja le, majd írja ki
    }
#endif // UNITY_EDITOR
#endregion // Indítási tömörítés

    /// <summary>
    /// Fájlválasztó, amiben megkeresendőek a játék adatfájlai
    /// </summary>
    FilePicker fp = new FilePicker();

    /// <summary>
    /// Induláskor azonnal lefut
    /// </summary>
    void Start() {
#if UNITY_EDITOR // Ez csak szerkesztőben van implementálva
        DoCompress(); // Tömörítse, ha van megadott tömörítendő pixeltérkép
#endif // UNITY_EDITOR
        if (PlayerPrefs.HasKey("Path")) // Ha volt elmentett hely...
            fp.FullLocation = PlayerPrefs.GetString("Path"); // ...indítsa onnan a fájlválasztót
        fp.Show(false); // Nyissa meg, de ha eseleg nem találtható a mentett helyen játékfájl, a rendszer gyökerében
        fp.EnableMovement = false; // Ablak elmozgatásának tiltása
        fp.FoldersOnly = true; // Fájlok kilistázásának tiltása
        fp.Position = new Rect(0, 0, 300, Screen.height); // A kép balsó 300 pixele legyen a fájlválasztó
        Graphics.ForegroundColor = new Color(.698f, .741f, .031f); // Az aktív pixelek legyenek a Nokia 3310 színei
	}

    /// <summary>
    /// GUI-kezelő függvény
    /// </summary>
    void OnGUI() {
        fp.OnGUI(1); // Fájlválasztó megjelenítése
        int DrawWidth = Screen.width - 300; // Rajz szélessége (ablak szélessége - fájlválasztó)
        int WScale = DrawWidth / 84; // Szélességet pontosan kitöltő pixelméret
        int HScale = Screen.height / 48; // Magasságot pontosan kitöltő pixelméret
        int Scale = Math.Min(WScale, HScale); // Az legyen a végső méret, amelyik kisebb (sehol ne lógjon ki, de legyen maximális méretű, és négyzetekből álljon)
        int Left = 300 + (DrawWidth - 84 * Scale) / 2; // Bal oldali behúzás
        int Top = (Screen.height - 48 * Scale) / 2; // Felső behúzás
        Graphics.Draw(ref Objects.Space, new Rect(Left + 8 * Scale, Top + 11 * Scale, 67 * Scale, 12 * Scale)); // A logó Space részének megjelenítése
        Graphics.Draw(ref Objects.Impact, new Rect(Left + 4 * Scale, Top + 25 * Scale, 76 * Scale, 12 * Scale)); // A logó Impact részének megjelenítése
    }

    /// <summary>
    /// Minden képkuckára egyszer lefutó függvény
    /// </summary>
	void Update() {
	    if (Menu.DataFolder != fp.FullLocation) { // Ha változott az előző képkocka óta a vizsgált mappa (már azon a helyen tárolja az előzőt, ahol később szükség lesz rá)
            Menu.DataFolder = fp.FullLocation; // Jelenlegi mappa eltárolása (ne vizsgálja minden képkockában a következőket)
            // Ha olyan mappa nyílt meg, ahol létezik a játéknak megfelelő adatfájl-struktúra, az valószínűleg az lesz
            if (File.Exists(Menu.DataFolder + "\\data\\enemies\\0.dat")
                && File.Exists(Menu.DataFolder + "\\data\\levels\\0.dat")
                && File.Exists(Menu.DataFolder + "\\data\\objects\\0.dat")) {
                gameObject.AddComponent<Menu>(); // Menü megnyitása...
                Destroy(this); // ... = cserélje le ezt a komponenst a menüre
            }
        }
	}
}