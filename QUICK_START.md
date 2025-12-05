# 🚀 Quick Start Guide

## ✅ All Compilation Errors Are Fixed!

All the issues have been resolved. Follow these steps:

## Step 1: Clean Reopen

1. **Close Unity** completely if it's open
2. **Delete the `Library` folder** (if it exists in the project root)
3. **Reopen Unity 6** with this project
4. **Wait** for all scripts to compile
5. **Verify**: Console should show **0 errors**

> ⚠️ If Unity offers "Safe Mode", click **Ignore** and let it compile

## Step 2: Create Game Config

1. In Project window, right-click in **Assets** folder
2. **Create → FallingSand → Game Config**
3. Name it `GameConfig`
4. Leave default settings (they're already correct)

## Step 3: Create Scene

1. **File → New Scene → 2D (Core)**
2. **Save** as `Assets/Scenes/MainScene.unity`

### Setup Camera:
- Select "Main Camera" in Hierarchy
- Tag: "MainCamera" (should already be set)
- Position: (640, 400, -10)
- Orthographic Size: 4
- Background: Black

### Create Game Object:
1. **Right-click in Hierarchy → Create Empty**
2. Rename to "CellularAutomaton"
3. **Add Component → CellularAutomaton**
4. **Add Component → InputManager**
5. **Add Component → UIManager**

### Wire Up References:
**CellularAutomaton component:**
- Config: Drag `GameConfig` asset here

**InputManager component:**
- Cellular Automaton: Drag CellularAutomaton GameObject (self)
- Main Camera: Drag Main Camera from Hierarchy
- Brush Size: 5
- Max Brush Size: 50
- Min Brush Size: 1

**UIManager component:**
- Cellular Automaton: Drag CellularAutomaton GameObject (self)

## Step 4: Play!

Press **▶ Play** and you should see:
- Black screen with stone floor at bottom
- FPS counter
- Help window

**Controls:**
- **Left-click**: Draw
- **1-9**: Select elements
- **Scroll**: Brush size
- **Space**: Pause
- **C**: Clear

## 🎮 Controls Reference

| Key | Element |
|-----|---------|
| 1 | Sand |
| 2 | Water |
| 3 | Stone |
| 4 | Wood |
| 5 | Lava |
| 6 | Gunpowder |
| 7 | Oil |
| 8 | Acid |
| 9 | Snow |
| 0 | Eraser |

| Key | Action |
|-----|--------|
| Space | Pause/Resume |
| C | Clear All |
| B | Toggle Brush (Circle/Square) |
| W | Toggle Weather |
| Scroll | Brush Size |

## 🔧 Troubleshooting

### "Script is missing" or yellow warnings
- Make sure compilation finished (0 errors in Console)
- Try: Right-click Assets → Reimport All

### Can't find "Create → FallingSand" menu
- Ensure zero compilation errors
- Check that `GameConfig.cs` has `[CreateAssetMenu]` attribute
- Restart Unity

### Black screen, nothing happens
- Verify GameConfig is assigned to CellularAutomaton
- Check InputManager has Camera reference
- Look for errors in Console

### Still seeing compilation errors
- See `COMPILATION_FIXES.md` for detailed fix information
- Ensure all files were updated (pull latest changes)
- Delete Library folder and reimport

## 📚 Additional Documentation

- **COMPILATION_FIXES.md** - What was fixed and why
- **SETUP_INSTRUCTIONS.md** - Detailed step-by-step setup
- **README.md** - Full project documentation

Enjoy your falling sand simulation! 🎨

