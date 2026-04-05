# 🎮 Cannon Merge — Tower Defence Game

![Unity](https://img.shields.io/badge/Unity-6-black?logo=unity)
![Platform](https://img.shields.io/badge/Platform-Android-green?logo=android)
![Language](https://img.shields.io/badge/Language-C%23-purple?logo=csharp)
![Status](https://img.shields.io/badge/Status-Complete-brightgreen)

> A strategic merge tower-defence game built in Unity 6 for Android.  
> Summon cannons, merge them to upgrade, and survive increasingly difficult waves of enemies.

---

## 📱 Screenshots

<!-- Add your screenshots here after uploading them to the repo -->
<!-- Example: -->
<!-- ![Gameplay](screenshots/gameplay.png) -->
<!-- ![Wave](screenshots/wave.png) -->

*Add screenshots by creating a `screenshots/` folder in the repo and uploading your images there.*

---

## 🕹️ Gameplay

### Core Loop
1. **Summon** a cannon by spending 10 coins — it appears on a random empty grid cell
2. **Drag and drop** the cannon to any empty cell on the grid
3. **Merge** two same-level cannons by dragging one onto the other — they combine into a higher level cannon with more damage and fire rate
4. Press **SEND** to start the wave — enemies spawn from the top and move downward
5. Cannons **automatically shoot bullets** straight up their column
6. **Survive** the wave — earn coins as reward and prepare for the next wave

### Grid System
- 5×4 grid positioned at the bottom of the screen
- Each cell can hold one cannon
- Cannons only protect their own column — placement strategy matters

### Coin Economy
| Action | Cost / Reward |
|--------|--------------|
| Summon Cannon | 10 coins |
| Rapid Fire Power-Up | 5 coins |
| Shield Power-Up | 5 coins |
| Fire Power Power-Up | 5 coins |
| Wave Clear Reward | 8 + (wave × 2) coins |

---

## ⚔️ Enemy Types

| Type | HP | Speed | Color | Appears |
|------|----|-------|-------|---------|
| Normal | 50 | Slow | 🔴 Red | Wave 1+ |
| Fast | 80 | Fast | 🟡 Yellow | Wave 2+ |
| Tank | 200 | Very slow | 🟠 Orange | Wave 3+ |
| Boss | 500 | Slow | 🟣 Purple | Every 5th wave |

- Each enemy displays its **current HP** in the center
- Enemies **wobble** as they move down for visual feedback
- Enemies that reach the base **stop and attack** every second
- Enemy **speed increases 20% per wave** after wave 3

---

## 📈 Difficulty Curve

| Wave | Normal | Fast | Tank | Boss | Notes |
|------|--------|------|------|------|-------|
| 1 | 2 | 0 | 0 | 0 | Easy intro |
| 2 | 3 | 1 | 0 | 0 | First Fast enemy |
| 3 | 4 | 2 | 1 | 0 | First Tank enemy |
| 4 | 3 | 2 | 1 | 0 | Speed starts increasing |
| 5 | 3 | 3 | 2 | 1 | First Boss! |
| 6+ | 3 | 4+ | 3+ | 1-2 | Intense — scales every wave |

---

## ⚡ Power-Ups

| Power-Up | Effect | Cost |
|----------|--------|------|
| ⚡ Rapid Fire | Doubles fire rate of all cannons | 5 coins |
| 🛡️ Shield | Protects cannon base from damage | 5 coins |
| 🔥 Fire Power | +20 damage to all cannons | 5 coins |

---

## 🔧 Technical Details

### Built With
- **Engine:** Unity 6 (2D URP)
- **Language:** C# 
- **Platform:** Android (APK)
- **UI:** TextMeshPro, Unity UI Canvas

### Architecture
The game uses a **Singleton pattern** for all major managers so any script can access them directly without passing references:

```
GameManager      — coins, wave number, rewards
GridManager      — cell grid, column positions
BattleManager    — wave spawning, enemy tracking
UIManager        — coin display, wave display, messages
CannonBase       — player base HP, game over
SummonManager    — cannon spawning with coin check
PowerUpManager   — power-up activation with coin check
```

### Key Scripts

| Script | Responsibility |
|--------|---------------|
| `GridManager.cs` | Builds NxM grid, auto-scales to screen width, provides column positions |
| `Cell.cs` | Tracks occupancy, places/clears cannons |
| `Cannon.cs` | Level, fire rate, damage, auto-shoot coroutine, level label |
| `CannonDragDrop.cs` | IPointer drag-and-drop, closest-cell detection, merge logic |
| `SummonManager.cs` | Coin check, spawn on random empty cell |
| `BattleManager.cs` | Wave composition, enemy spawning, wave completion tracking |
| `Enemy.cs` | 4 enemy types, HP label, wobble animation, base attack |
| `Bullet.cs` | Vertical movement, OnTriggerEnter2D damage |
| `PowerUpManager.cs` | Coin-gated power-up activation |
| `GameManager.cs` | Coin economy, wave progression, rewards |
| `UIManager.cs` | Event-driven UI updates, temporary messages |
| `CannonBase.cs` | Player base HP bar, game over on destruction |

### Interesting Implementation Details

**Drag and Drop** — Uses `IPointerDownHandler`, `IDragHandler`, `IPointerUpHandler` interfaces instead of `OnMouseDown` for reliable Unity 6 compatibility. The cannon's own collider is disabled during drag so it doesn't block cell detection. Cell snapping uses closest-cell-by-distance rather than raycasting.

**Vertical Bullets** — Bullets travel straight up and use `OnTriggerEnter2D` to hit any enemy in their path, adding strategic depth to cannon placement.

**Wave System** — Tracks `enemiesSpawned` and `enemiesKilled` as separate counters. Wave completes only when both equal the total, handling all enemy exit paths (killed, escaped, attacking base).

**Enemy Wobble** — Uses `Mathf.Sin(Time.time + randomOffset)` for smooth oscillating rotation. Random offset ensures enemies don't all wobble in sync.

**Auto Screen Fit** — Grid cell size is calculated from `Camera.orthographicSize * aspect` so the grid fits perfectly on any screen resolution.

---

## 🚀 How to Run

### Play the APK (Android)
1. Download `CannonMerge.apk` from the [Releases](../../releases) section
2. On your Android device go to **Settings → Security → Unknown Sources** and enable it
3. Open the downloaded APK and tap **Install**
4. Launch **Cannon Merge** from your app drawer

### Open in Unity
1. Install **Unity Hub** from [unity.com](https://unity.com)
2. Install **Unity 6** via Unity Hub
3. Clone this repository:
   ```bash
   git clone https://github.com/YOUR_USERNAME/CannonMergeGame.git
   ```
4. In Unity Hub click **Open** and select the cloned folder
5. Open the scene at `Assets/Scenes/GameScene.unity`
6. Press **Play** to run in editor

---

## 📁 Project Structure

```
Assets/
├── Scenes/
│   └── GameScene.unity
├── Scripts/
│   ├── GameManager.cs
│   ├── UIManager.cs
│   ├── GridManager.cs
│   ├── Cell.cs
│   ├── Cannon.cs
│   ├── CannonDragDrop.cs
│   ├── SummonManager.cs
│   ├── BattleManager.cs
│   ├── Enemy.cs
│   ├── Bullet.cs
│   ├── PowerUpManager.cs
│   └── CannonBase.cs
├── Prefabs/
│   ├── Cell.prefab
│   ├── Cannon.prefab
│   ├── Enemy.prefab
│   └── Bullet.prefab
└── Sprites/
```

---

## 🎯 Future Improvements

- [ ] Sprite artwork to replace placeholder shapes
- [ ] Particle effects on merge and enemy death
- [ ] Sound effects and background music
- [ ] High score system using PlayerPrefs
- [ ] Special cannon abilities at high merge levels
- [ ] Animated merge effect
- [ ] Game over screen with wave reached stats
- [ ] Multiple grid sizes as difficulty option

---

## 👨‍💻 Developer

Built as a technical assessment for the **AIRIDEV Game Developer Internship**.

---

## 📄 License

This project is for assessment and portfolio purposes.