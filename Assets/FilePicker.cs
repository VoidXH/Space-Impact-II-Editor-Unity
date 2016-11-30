/// ***************************************************************
/// Fájlválasztó szkript
/// Forrás: Loopy (http://voidx.tk/)
/// ***************************************************************
using UnityEngine;
using System.IO;

public class FilePicker {
    public bool Open = false, FoldersOnly = false, Loaded = false, EnableMovement = true, EnableFolderSwitch = true;
    public Rect Position;
    public string Location = "\\", FullLocation = "\\";
    public FileInfo Picked;

    public string Folder { get { return FullLocation; } set { FullLocation = value; CacheFolder(value); } }
    public DirectoryInfo[] Folders;
    public FileInfo[] Files;

    bool TreeTop;
    Vector2 PickerScroll;

    void CacheFolder(string Path) {
        if (!Directory.Exists(Path))
            Path = "\\";
        TreeTop = Path == "\\";
        if (TreeTop) {
            Location = "Drives";
            string[] Drives = Directory.GetLogicalDrives();
            Folders = new DirectoryInfo[Drives.Length];
            for (int i = 0; i < Drives.Length; i++)
                Folders[i] = new DirectoryInfo(Drives[i]);
        } else {
            DirectoryInfo Info = new DirectoryInfo(Path);
            Location = Info.Name;
            FullLocation = Info.FullName;
            Folders = Info.GetDirectories();
            if (!FoldersOnly)
                Files = Info.GetFiles();
        }
        PickerScroll = new Vector2(0, 0);
    }

    public void Show(bool KeepLocation = true) {
        Position = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 200, 300, 400);
        CacheFolder(KeepLocation ? FullLocation : "\\");
        Open = true;
    }

    public void Toggle(bool KeepLocation = true) {
        if (Open)
            Open = false;
        else
            Show(KeepLocation);
    }

    void WindowTick(int ID) {
        GUI.color = Color.red;
        if (GUI.Button(new Rect(285, 5, 10, 10), ""))
            Open = false;
        GUI.color = Color.white;
        int Top = -20, ListHeight = Folders.Length + (!EnableFolderSwitch || TreeTop ? 0 : 2);
        if (!FoldersOnly)
            ListHeight += Files.Length + (EnableFolderSwitch ? 1 : 0);
        ListHeight *= 20;
        int WindowHeight = (int)Position.height - 25, ButtonWidth = (int)Position.width - (ListHeight > WindowHeight ? 28 : 10);
        TextAnchor OldAlign = GUI.skin.button.alignment;
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        PickerScroll = GUI.BeginScrollView(new Rect(5, 20, 290, WindowHeight), PickerScroll, new Rect(0, 0, ButtonWidth, ListHeight));
        if (EnableFolderSwitch && !TreeTop && GUI.Button(new Rect(0, Top += 20, ButtonWidth, 20), "Top"))
            CacheFolder("\\");
        if (EnableFolderSwitch && !TreeTop && GUI.Button(new Rect(0, Top += 20, ButtonWidth, 20), "Up"))
            CacheFolder(Location.Length == 3 ? "\\" : Directory.GetParent(FullLocation).FullName);
        if (EnableFolderSwitch)
            for (int i = 0; i < Folders.Length; i++)
                if (GUI.Button(new Rect(0, Top += 20, ButtonWidth, 20), Folders[i].Name))
                    CacheFolder(Folders[i].FullName);
        if (!FoldersOnly) {
            if (Top != -20)
                Top += 20;
            for (int i = 0; i < Files.Length; i++)
                if (GUI.Button(new Rect(0, Top += 20, ButtonWidth, 20), Files[i].Name))
                    Picked = Files[i];
        }
        GUI.skin.button.alignment = OldAlign;
        GUI.EndScrollView();
        GUI.DragWindow();
    }

    public void OnGUI(int WindowID) {
        if (Open) {
            if (EnableMovement) {
                Position = GUI.Window(WindowID, Position, WindowTick, Location);
                if (Position.x < 0)
                    Position.x = 0;
                if (Position.y < 0)
                    Position.y = 0;
                if (Position.xMax >= Screen.width)
                    Position.x = Screen.width - Position.width;
                if (Position.yMax >= Screen.height)
                    Position.y = Screen.height - Position.height;
            } else
                GUI.Window(WindowID, Position, WindowTick, Location);
        }
    }
}