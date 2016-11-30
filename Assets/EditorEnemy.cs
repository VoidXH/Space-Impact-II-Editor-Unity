using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Ellenségszerkesztő felület
/// </summary>
public class EditorEnemy : Editor {
    float AnimTimer = .4f; // A következő animációs fázisig hátralévő idő
    int ModelID, X, Y, Anims, Lives, Floats, ShotTime, MoveUp, MoveDown, MoveAnyway, MoveMin, MoveMax, AnimPhase; // Az ellenség tulajdonságai
    Objects.Obj[] AnimPhases; // Animációs fázisok grafikái
    string SaveAsID; // Mentési ID szövegként (hogy a felhasználó átírhassa)
    string ModelID_str; // Első animációs fázis objektumának azonosítója szövegként (hogy a felhasználó átírhassa)

    /// <summary>
    /// Animációk újratöltése
    /// </summary>
    void ReloadAnims() {
        ModelID = Convert.ToInt32(ModelID_str); // Első animációs fázis objektumának azonosítója
        AnimPhases = new Objects.Obj[Anims]; // Az összes animációs fázis grafikáinak helye
        string ObjectFolder = Path.Substring(0, Path.LastIndexOf('\\')); // Objektummappa kezdete (levágott fájlnév)
        ObjectFolder = ObjectFolder.Substring(0, ObjectFolder.LastIndexOf('\\')) /* Levágott jelenlegi mappanév */ + "\\objects\\"; // Elérési út végének cseréje
        for (int Phase = 0; Phase < Anims; ++Phase) // Minden fázisra...
            AnimPhases[Phase] = Objects.LoadObject(ObjectFolder + (ModelID + Phase).ToString() + ".dat"); // ...töltse be a modellt
        AnimPhase = X = Y = 0; // Animációs fázis és méretek nullázása
        for (int Anim = 0; Anim < Anims; ++Anim) { // Ellenség méretének meghatározása: a legnagyobb az animációk közt
            if (X < AnimPhases[Anim].X)
                X = AnimPhases[Anim].X;
            if (Y < AnimPhases[Anim].Y)
                Y = AnimPhases[Anim].Y;
        }
    }

    /// <summary>
    /// Újratöltés
    /// </summary>
    void Reload() {
        int AfterBackslash = Path.LastIndexOf('\\') + 1; // Fájlnév kezdetének helye a fájl elérési útvonalában
        SaveAsID = Path.Substring(AfterBackslash, Path.LastIndexOf('.') - AfterBackslash); // Fájlnév, azaz mentési azonosító
        byte[] Data = File.ReadAllBytes(Path); // Fájl beolvasása
        ModelID = Data[0]; // Első animációs fázis objektumának azonosítója
        ModelID_str = ModelID.ToString(); // Első animációs fázis objektumának azonosítója szövegként
        Anims = Data[1]; // Animációs fázisok száma
        Lives = Data[2]; // Életek
        Floats = Data[3]; // Beúszva a pályára helyben marad-e
        ShotTime = Data[4]; // Lövések közti idő
        MoveUp = Data[5]; // Mozog-e felfelé
        MoveDown = Data[6]; // Mozog-e lefelé
        MoveAnyway = Data[7]; // Mozog-e pályán kívül
        MoveMin = Data[8]; // Felső mozgáshatár
        MoveMax = Data[9]; // Alsó mozgáshatár
        ReloadAnims(); // Animációs fázisok betöltése
    }

    /// <summary>
    /// Induláskor azonnal lefut
    /// </summary>
    void Start() {
        Reload(); // Újratöltés
    }

    /// <summary>
    /// Mentés
    /// </summary>
    /// <param name="Path">Célfájlnév</param>
    void Save(string Path) {
        File.WriteAllBytes(Path, new byte[] {Convert.ToByte(ModelID_str), (byte)Anims, (byte)Lives, (byte)Floats, (byte)ShotTime, (byte)MoveUp,
            (byte)MoveDown, (byte)MoveAnyway, (byte)MoveMin, (byte)MoveMax}); // Tulajdonságok kiírása a megfelelő sorrendben a célfájlba
        Menu.ReloadMenu(); // Menü újramegnyitása, hogy egy esetleges új fájl megjelenjen
    }

    /// <summary>
    /// GUI-kezelő függvény
    /// </summary>
    void OnGUI() {
        ///////////////
        // Konfiguráció
        ///////////////
        bool ID_OK = Menu.IsNumeric(SaveAsID); // Érvényes szám van-e az azonosító helyére írva
        bool Model_OK = ModelID_str.Length != 0 && Menu.IsNumeric(ModelID_str); // Érvényes szám van-e az objektumazonosító helyére írva
        int Left = (Screen.width - 300) / 4 + 155; // Bal oldali behúzás
        int Top = (Screen.height - 275) / 2; // Jobb oldali behúzás
        GUI.color = ID_OK ? Color.white : Color.red; // Ha nincs rendben az azonosító mezeje, színezze vörösre
        Graphics.NamedField(new Rect(Left, Top, 200, 20), "ID:", ref SaveAsID); // Beviteli mező az azonosítónak
        GUI.color = Model_OK ? Color.white : Color.red; // Ha nincs rendben az objektumazonosító mezeje, színezze vörösre
        Graphics.NamedField(new Rect(Left, Top += 25, 200, 20), "Model ID:", ref ModelID_str); // Beiviteli mező az objektumazonosítónak
        GUI.color = Color.white; // Tovább biztosan ne tartson az esetleges vörös színezés
        GUI.Label(new Rect(Left, Top += 25, 500, 20), "Size: " + X.ToString() + "x" + Y.ToString() + " (automatically calculated)"); // Kiszámolt méret kiírása
        Anims = (int)Graphics.NamedSlider(new Rect(Left, Top += 20, 250, 20), "Animation phases", Anims, 1, 15); // Animációs fázisok csúszkája
        if (ModelID_str.Length != 0 && Menu.IsNumeric(ModelID_str) && (Anims != AnimPhases.Length || Convert.ToInt32(ModelID_str) != ModelID))
            ReloadAnims(); // Ha az előző csúszka értéke vagy az első animációs fázis azonosítója változott, és helyes értékre, töltse újra az animációs fázisokat
        Lives = (int)Graphics.NamedSlider(new Rect(Left, Top += 20, 250, 20), "Lives", Lives, 1, 127); // Életek csúszkája
        Floats = GUI.Toggle(new Rect(Left, Top += 20, 200, 20), Floats == 1, " Floats") ? 1 : 0; // Lebegés állítása
        ShotTime = (int)Graphics.NamedSlider(new Rect(Left, Top += 20, 250, 20), "Shot cooldown", ShotTime, 0, 80); // Lövések közti idő csúszkája
        MoveUp = GUI.Toggle(new Rect(Left, Top += 20, 200, 20), MoveUp == 1, " Move upwards") ? 1 : 0; // Felfele mozgás állítása
        MoveDown = GUI.Toggle(new Rect(Left, Top += 20, 200, 20), MoveDown == 1, " Move downwards") ? 1 : 0; // Lefele mozgás állítása
        MoveAnyway = GUI.Toggle(new Rect(Left, Top += 20, 200, 20), MoveAnyway == 0, " Only move on screen") ? 0 : 1; // Csak képernyőn mozgás állítása
        MoveMin = (int)Graphics.NamedSlider(new Rect(Left, Top += 20, 250, 20), "Top position", MoveMin, 0, Math.Min(48 - Y, MoveMax)); // Felső mozgáshatár csúszkája
        MoveMax = (int)Graphics.NamedSlider(new Rect(Left, Top += 20, 250, 20), "Bottom position", MoveMax, MoveMin, 48 - Y); // Alsó mozgáshatár csúszkája
        GUI.enabled = ID_OK && Model_OK; // Ha a két szöveges mező rendben, engedélyezze a mentés gomb megnyomását
        if (GUI.Button(new Rect(Left, Top += 20, 100, 25), "Save")) // Mentés gomb
            Save(Path.Substring(0, Path.LastIndexOf('\\') + 1) + SaveAsID + ".dat"); // Mentés a célfájlnéven
        GUI.enabled = true; // Mindenképp engedélyezze a visszaállítás gomb megnyomását
        if (GUI.Button(new Rect(Left + 105, Top, 100, 25), "Revert")) // Visszaállítás gomb
            Reload(); // Töltse újra teljesen a fájlt
        ///////////////////////
        // Ellenség kirajzolása
        ///////////////////////
        int HalfWidth = Screen.width / 2; // A képernyő szélességének fele
        Graphics.Draw(ref AnimPhases[AnimPhase], new Rect(HalfWidth + 150, 0, HalfWidth - 150, Screen.height)); // A fájlkeresők által nem elfoglalt rész jobb felén legyen az objektum
        AnimTimer -= Time.deltaTime; // Az animáció jelenlegi fázisának megjelenítéséből hátralévő idő csökkentése a képkockaidővel
        if (AnimTimer <= 0) { // Ha letelt a jelenlegi fázis ideje:
            AnimTimer = .4f; // Időzítő alaphelyzetbe állítása
            AnimPhase++; // Következő animációs fázisba lépés
            if (AnimPhase == Anims) // Ha az utolsó fázison túlugrott...
                AnimPhase = 0; // ...kezdje elölről
        }
    }
}