using UnityEngine;

/// <summary>
/// Szerkesztő őskomponens
/// </summary>
public class Editor : MonoBehaviour {
    /// <summary>
    /// Szerkesztésre megnyitott fájl elérési útvonala
    /// </summary>
    public string Path;

    /// <summary>
    /// A jelenleg megnyitott szerkesztő
    /// </summary>
    public static Editor Instance = null;

    /// <summary>
    /// A szerkesztő inicializálása előtt lefutó függvény
    /// </summary>
    void Awake() {
        if (Instance) // Ha van megnyitott szerkesztő...
            Destroy(Instance); // ...zárja be
        Instance = this; // Legyen az új szerkesztő a jelenleg megnyitott
    }
}