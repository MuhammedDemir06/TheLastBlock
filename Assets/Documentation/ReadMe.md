🎮 [The Last Block] – Open Source Unity Platformer

Welcome to the source code of this open-source Unity platform game.  
This project is a fully functional and extensible platform game system built with a custom level editor, save/load system, and dynamic level loading.  
This README will guide you through the architecture and how to contribute, maintain, or learn from it.

---

## 📁 Project Structure
Assets/
├── Scripts/ # Gameplay, Player, UI, Manager scripts
├── Resources/
│ └── Levels/ # Chapters and levels as ScriptableObjects
├── Editor/
│ └── LevelEditorWindow.cs # Custom editor tool for creating levels
├── Prefabs/ # UI buttons, tile objects, etc.
├── Scenes/
│ └── Game.unity # Main gameplay scene
└── ...


---

## 🧱 Level System

### 1. Level Data

- Levels are stored as `LevelData` ScriptableObjects.
- Path: `Resources/Levels/ChapterName/Level_X.asset`
- Each chapter is a folder, and each level inside it is numbered (e.g., `Level_1.asset`, `Level_2.asset`).

### 2. Custom Level Editor

- File: `Editor/LevelEditorWindow.cs`
- Open via: **`Tools > Level Editor`**
- ⚠️ **Only works inside the `Game` scene**
- Features:
  - Grid-based tile placement
  - Level saving and clearing
  - Editor reset options
  - Drag-and-drop tile types

### 3. Dynamic Chapter and Level Loading

- Chapters are scanned from `Resources/Levels/`
- Each folder = 1 chapter
- Levels are sorted numerically and instantiated as UI buttons
- Scroll & Snap system is available for horizontal chapter browsing

---

## 🎮 Player System

### `PlayerController.cs`

- Handles:
  - Movement input
  - Collision with obstacles
  - Reset/Retry system
- Visual effects & transitions use DOTween

---

## 💾 Save System

### `PlayerDataManager.cs`

- Saves are stored as a JSON file at:


- Data saved:
- Current chapter
- Unlocked levels per chapter
- Sound settings

### Reset Save Data

- Delete all progress easily from Editor:  
➤ **`Tools > Game > Delete Data`**

---

## 🧪 Creating New Levels

To create a new level:

1. Open the `Game` scene
2. Navigate to **`Tools > Level Editor`**
3. Design your level using tiles
4. Set `PlayerStartPos` and `NextLevel` objects
5. Click **Save Level**
6. New levels are saved in:  
 `Assets/Resources/Levels/{Chapter}/Level_X.asset`
7. After any change, reset save data:  
 ➤ `Tools > Game > Delete Data`

---

## 🔊 Audio

- Controlled by `AudioListener.volume`
- Smooth fade-in via DOTween
- Volume slider UI updates saved settings
- Sounds are defined in `SoundData` ScriptableObject

---

## 🌟 Features Summary

- ✅ Dynamic chapter/level UI generation
- ✅ In-Editor level builder
- ✅ JSON save system
- ✅ Audio transitions & effects
- ✅ Open-source and extensible design

---

## 📣 Contributing

This project is open-source and welcomes contributions.  
You can:
- ⭐️ Star the repo
- 🛠️ Fork it and build your own game
- 🐞 Report bugs or suggestions

---

## 🙏 Thanks

Thank you for using or exploring this project.  
If it helps you in learning or building something fun — that's a success.

> Happy coding & game building!
