# Escape Room VR Project

This is a Virtual Reality (VR) "Escape Room" game developed using Unity. Players can interact with the environment, collect items, and solve various puzzles to escape the room using VR hardware.

## Project Overview

The game is based on a classic escape room scenario where the player is trapped in a asylum and must complete specific tasks to get out.

### Core Mechanics

*   **VR Interaction:** Players can grab objects, press buttons, and interact with the environment.
*   **Inventory System:** Collected items are stored in an inventory system.
*   **Save and Load System:** The `GameCache` system persistently saves and loads player progress (collected items, completed missions) using `PlayerPrefs`.

## Project Structure

```text
EscapeRoom/
├── Assets/
│   ├── MyScripts/            # Core game logic and scripts
│   ├── MyScenes/             # Game levels and scenes
│   ├── Abandoned_Asylum/      # Main environment assets
│   ├── Free Wood Door Pack/   # Door models and textures
│   ├── HorrorPuzzleItems/    # Puzzle-related 3D models
│   ├── XR/                   # XR Interaction Toolkit assets
│   └── ... 
├── ProjectSettings/          # Unity project configurations
└── README.md                 # Project documentation
```

## Important Scripts (`Assets/MyScripts`)

- **`GameManager.cs`**: Manages game state (Pause/Resume) and global settings.
- **`GameCache.cs`**: Handles persistent storage of inventory and mission states.
- **`MissionTerminal.cs`**: Logic for the PC terminal missions and rewards.
- **`Door.cs`**: Manages door animations, locking logic, and item requirements.
- **`VRItemPickup.cs`**: Facilitates picking up items and adding them to the inventory.
- **`VRInventoryToggle.cs`**: Controls the visibility of the VR inventory UI.

## Controls (VR Right Hand)

- **A Button:** Adds the hovered or grabbed item to the inventory.
- **B Button:** Interacts with doors or activates terminals.
- **Grab Trigger:** Pick up and hold objects.

## Assets Used

## Assets Used

This repository does not include the third-party asset files themselves due to licensing restrictions. You can download them directly from the Unity Asset Store using the links below:

- **[Abandoned Asylum](https://assetstore.unity.com/packages/3d/environments/urban/abandoned-asylum-49137):** Main environmental designs and architectural structures (by 3D Urban).
- **[Retro PSX Horror Puzzle Item Pack (Icon+LowPoly)](https://assetstore.unity.com/packages/3d/props/retro-psx-horror-puzzle-item-pack-icon-lowpoly-250188):** 3D props used for puzzles, including keys, keycards, and floppy disks (by Retro).
- **[VR Male Hand (Left & Right)](https://assetstore.unity.com/packages/3d/characters/humanoids/vr-male-hand-left-right-209546):** Humanoid 3D models used for the player's VR hands (by VR).
- **[Free Horror Ambience 2](https://assetstore.unity.com/packages/audio/music/free-horror-ambience-2-215651):** Atmospheric in-game background audio and soundscapes (by Audio Music).
- **[Drag & Drop Inventory & Hotbar Framework](https://assetstore.unity.com/packages/tools/gui/drag-drop-inventory-hotbar-framework-333604):** Core framework and user interface (UI) infrastructure for the item mechanics (by GUI Tools).
- **Unity XR Interaction Toolkit:** The foundation for VR input and interactions.


## Setup and Usage

1. Open the project in Unity (Recommended: 2021.3+).
2. Ensure `XR Plugin Management` is configured for your VR hardware (Oculus, OpenXR, etc.).
3. Open the main scene located in `Assets/MyScenes` and press Play.

## Demo Gameplay
https://github.com/user-attachments/assets/953e877e-1fd1-4a50-9ce2-00a023377e99

https://github.com/user-attachments/assets/1fa3c38c-6e5e-4360-8fea-bffb7dadbbd6

https://github.com/user-attachments/assets/132b8830-c2c5-4cc2-8f02-45ceee4ab331

---
This project is prepared for educational and development purposes.

## Authors & Contributors

This project was co-developed as a collaborative effort by:

* **[Nida Elvin Mertoğlu](https://github.com/NidaElvinMertoglu)** – Computer Engineering Student
  * *Roles:* Core VR gameplay systems, `GameCache` save/load logic, UI implementation, level design, environment setup, puzzle logic implementation.
* **[Ecem Şimşek](https://github.com/simsekecem)** – Computer Engineering Student
  * *Roles:* Core VR gameplay systems, `GameCache` save/load logic, UI implementation, level design, environment setup, puzzle logic implementation.
