using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Objektumok tárolására és kezelésére szolgáló osztály
/// </summary>
public static class Objects {
    /// <summary>
    /// Objektumtároló
    /// </summary>
    public struct Obj {
        public int X, Y; // Méretek
        public byte[] Image; // Pixeltérkép
    };

    /// <summary>
    /// A logó Space része
    /// </summary>
    public static Obj Space = new Obj() {
        X = 67, Y = 12,
        Image = new byte[804] {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,
                               0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,
                               0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,1,1,1,0,0,1,1,1,0,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,
                               0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,1,1,0,0,1,1,1,0,0,0,0,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,
                               0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,1,0,0,0,1,1,1,0,0,0,0,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,
                               0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,
                               0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,0,0,
                               0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,
                               0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,
                               0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,
                               0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,
                               1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0}
    };
    /// <summary>
    /// A logó Impact része
    /// </summary>
    public static Obj Impact = new Obj() {
        X = 76, Y = 12,
        Image = new byte[912] {0,0,0,1,1,1,1,1,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,
                               0,0,0,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,
                               0,0,0,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,0,0,0,0,0,1,1,1,1,0,1,1,1,0,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,
                               0,0,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,0,0,0,0,0,1,1,1,0,0,1,1,1,0,0,0,0,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,
                               0,0,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,0,0,0,0,0,1,1,0,0,0,1,1,1,0,0,0,0,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,
                               0,0,1,1,1,1,0,0,0,0,1,1,1,1,0,0,1,1,1,0,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,
                               0,0,1,1,1,1,0,0,0,0,1,1,1,1,0,0,1,1,0,0,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,
                               0,0,1,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,
                               0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,
                               0,1,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,
                               0,1,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,0,0,0,0,0,
                               1,1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0}
    };

    /// <summary>
    /// Objektum betöltése
    /// </summary>
    /// <param name="Path">Elérési útvonal</param>
    /// <returns>A betöltendő objektum</returns>
    public static Obj LoadObject(string Path) {
        Obj Image; // Tároló létrehozása
        Image.Image = new byte[84 * 48]; // Pixeltérkép azonnal max méretre, elfér, és nem kell átméretezéskor újrafoglalni
        try { // Ha nem lehet beolvasni a fájlt, ne omoljon össze
            byte[] Data = File.ReadAllBytes(Path); // Fájl bájtjainak beolvasása
            Image.X = Data[0]; // Az első bájt a szélesség
            Image.Y = Data[1]; // A második bájt a magasság
            int Pixels = Data[0] * Data[1]; // Pixelek száma
            int Bytes = Pixels / 8 + (Pixels % 8 != 0 ? 1 : 0); // Tömörített tömb mérete
            int Bits = Pixels % 8; // A vizsgált bájtból hátralévő bitek
            if (Bits == 0) // Ha a pixeltérkép pontosan kitölti a bájtokat...
                Bits = 8; // ...az egész vizsgált bájt hátra van
            while (Bytes-- != 0) { // Amíg van tömörített adat
                while (Bits-- != 0) { // Amíg a vizsgált bájtban van feldolgozatlan pixel
                    Image.Image[Bytes * 8 + Bits] = (byte)(Data[Bytes + 2] % 2); // Pixel elhelyezése a helyén
                    Data[Bytes + 2] >>= 1; // Ugrás a következő bitre a vizsgált bájton
                }
                Bits = 8; // A következő bájt feldolgozandó pixelei
            }
            return Image; // Adja vissza a sikeresen betöltött objektumot
        } catch { // Fájlolvasási hibák kezelése
            return new Obj() { X = 0, Y = 0, Image = new byte[84 * 48] }; // Ezesetben adjon vissza egy semmit
        }
    }

    /// <summary>
    /// Másolatot hoz létre egy objektumból
    /// </summary>
    /// <param name="Source">Forrás</param>
    /// <returns>Másolat</returns>
    public static Obj CopyObject(Obj Source) {
        Obj Copy; // Másolat tárolója
        Copy.X = Source.X; // Szélesség másolása
        Copy.Y = Source.Y; // Magasság másolása
        Copy.Image = new byte[84 * 48]; // Új, maximális méretű pixeltérkép (elfér, és nem kell átméretezéskor újrafoglalni)
        Array.Copy(Source.Image, Copy.Image, Source.X * Source.Y); // Pixelek átmásolása
        return Copy; // Adja vissza a másolatot
    }
}

/// <summary>
/// Rajzoló osztály
/// </summary>
public static class Graphics {
    /// <summary>
    /// Előtér színe (tároló)
    /// </summary>
    static Color _ForegroundColor;
    /// <summary>
    /// Előtérszínű 1x1-es textúra (tároló)
    /// </summary>
    static Texture2D _Foreground;

    /// <summary>
    /// Előtér színe
    /// </summary>
    public static Color ForegroundColor {
        get { return _ForegroundColor; } // Lekérni a tárolóból lehet
        set {
            _ForegroundColor = value; // Érték kiírása a tárolóba
            if (_Foreground != null) // Ha van létrehozott textúra
                MonoBehaviour.Destroy(_Foreground); // Oldja fel a memóriafoglalását (nem kezeli a GC)
            _Foreground = new Texture2D(1, 1); // Új előtértextúra létrehozása 1x1-es méretben
            _Foreground.SetPixel(0, 0, value); // A pixel kiszínezése
            _Foreground.Apply(); // Színezés alkalmazása
        }
    }

    /// <summary>
    /// Előtérszínű 1x1-es textúra
    /// </summary>
    public static Texture2D Foreground {
        get { return _Foreground; } // Csak lekérni lehessen, mert a szín átállítása hozza létre újra
    }

    /// <summary>
    /// Rajzolás
    /// </summary>
    /// <param name="obj">Kirajzolandó objektum</param>
    /// <param name="rect">Pozíció a képernyőn</param>
    /// <param name="Clickable">Kattintással átrajzolható legyen-e</param>
    public static void Draw(ref Objects.Obj obj, Rect rect, bool Clickable = false) {
        int WScale = (int)(rect.width / obj.X); // Szélességet pontosan kitöltő pixelméret
        int HScale = (int)(rect.height / obj.Y); // Magasságot pontosan kitöltő pixelméret
        int Scale = Math.Min(WScale, HScale); // Az legyen a végső méret, amelyik kisebb (sehol ne lógjon ki, de legyen maximális méretű, és négyzetekből álljon)
        int Left = (int)(rect.x + (rect.width - obj.X * Scale) / 2); // Bal oldali behúzás
        int Top = (int)(rect.y + (rect.height - obj.Y * Scale) / 2); // Felső behúzás
        bool Clicked = Clickable && Input.GetMouseButtonDown(0); // Módosítandó-e a pixel az egér helyén
        Vector2 MousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y); // Korrigált egérpozíció
        for (int xPos = 0; xPos < obj.X; ++xPos) { // Oszloponként
            for (int yPos = 0; yPos < obj.Y; ++yPos) { // Soronként
                int Pixel = xPos + yPos * obj.X; // Pixel helye a pixeltömbben
                Rect Position = new Rect(Left + xPos * Scale, Top + yPos * Scale, Scale, Scale); // Pixel helye a képernyőn
                if (obj.Image[Pixel] % 2 == 1) // Aktív pixel esetén...
                    GUI.DrawTexture(Position, _Foreground); // ...rajzoljon oda egy előtérszínű négyzetet
                if (Clicked && Position.Contains(MousePosition)) { // Ha a pixelre kattintott a felhasználó, és engedélyezve van a módosítás
                    if (obj.Image[Pixel] < 2) // Első OnGUI futás (képkockánként kétszer fut végig)
                        obj.Image[Pixel] = obj.Image[Pixel] == 0 ? (byte)3 : (byte)2; // A pixel értéke legyen ellentétes, és adjon hozzá kettőt
                                                                                      // E módon tudjuk, ha második OnGUI lefutás jött, és ne invertáljuk újra a pixelt
                    else // Második OnGUI futás
                        obj.Image[Pixel] -= 2; // Az előbb hozzáadott kettő kivonása, érvényes állapotba állítás
                }
            }
        }
    }

    /// <summary>
    /// Csúszka
    /// </summary>
    /// <param name="rect">Pozíció a kijelzőn</param>
    /// <param name="value">Jelenlegi érték</param>
    /// <param name="min">Minimális érték</param>
    /// <param name="max">Maximális érték</param>
    /// <returns>A felhasználó által választott érték</returns>
    public static float Slider(Rect rect, float value, float min, float max) {
        GUI.DrawTexture(new Rect(rect.x, rect.y + 5, rect.width, 2), Foreground); // Előtérszínű csúszkavonal
        return GUI.HorizontalSlider(rect, value, min, max); // Unity-féle csúszka fölé
    }

    /// <summary>
    /// Csúszka, előtte mezőnévvel és jelenlegi értékkel
    /// </summary>
    /// <param name="rect">Pozíció a kijelzőn</param>
    /// <param name="Name">Mezőnév</param>
    /// <param name="value">Jelenlegi érték</param>
    /// <param name="min">Minimális érték</param>
    /// <param name="max">Maximális érték</param>
    /// <returns>A felhasználó által választott érték</returns>
    public static float NamedSlider(Rect rect, string Name, float value, float min, float max) {
        int FieldWidth = (int)(rect.width * .5f); // A két komponens (szöveg/csúszka) szélessége
        GUI.Label(new Rect(rect.xMin, rect.yMin, FieldWidth, rect.height), Name + ": " + value.ToString()); // Mezőnév és jelenlegi érték megjelenítése
        return Slider(new Rect(rect.xMin + FieldWidth, rect.yMin + (rect.height - 12) / 2, FieldWidth, 12), value, min, max); // Csúszka megjelenítése és kezelése
    }

    /// <summary>
    /// Szövegbeviteli mező, előtte mezőnévvel
    /// </summary>
    /// <param name="rect">Pozíció a kijelzőn</param>
    /// <param name="Name">Mezőnév</param>
    /// <param name="Text">Bevitt szöveg</param>
    public static void NamedField(Rect rect, string Name, ref string Text) {
        int FieldWidth = (int)(rect.width * .5f); // A két komponens (szöveg/mező) szélessége
        GUI.Label(new Rect(rect.xMin, rect.yMin, FieldWidth, rect.height), Name); // Mezőnév megjelenítése
        Text = GUI.TextField(new Rect(rect.xMin + FieldWidth, rect.yMin, FieldWidth, rect.height), Text); // Beviteli mező megjelenítése és kezelése
    }
}
