# Unity Setup Instructions

⚠️ **IMPORTANT**: All compilation errors have been fixed! See `COMPILATION_FIXES.md` for details.

## Steps to Complete Setup:

### 1. First Time Opening in Unity

**If Unity is currently open, close it completely first!**

Then:

1. **Delete Library Folder** (if it exists) - This forces Unity to reimport everything fresh
2. **Open Project** in Unity 6
3. **Wait for Compilation**: Scripts will compile (should see 0 errors!)
4. **Verify**: Console should be clear, no compilation errors

**If you see errors**:
- Click "Ignore" if Unity offers Safe Mode
- Let compilation finish
- Clear Console (right-click → Clear)
- Check `COMPILATION_FIXES.md` if issues persist

### 2. Create the Game Configuration

1. In Project window, right-click in `Assets/` folder
2. Select **Create → FallingSand → Game Config**
3. Name it `GameConfig`
4. In Inspector, verify these settings:
   - Screen Width: 1280
   - Screen Height: 800
   - Pixel Size Modifier: 6
   - Box2d Size Modifier: 10
   - Gravity: (0, -5, 0)
   - Num Threads: 12
   - Use Multi Threading: ✓
   - Use Chunks: ✓

### 3. Create the Main Scene

1. **Create New Scene**: File → New Scene → 2D (Core)
2. **Save Scene**: Save it as `Assets/Scenes/MainScene.unity`

#### Setup Camera:
1. Select `Main Camera` in Hierarchy
2. In Inspector:
   - Clear Flags: Solid Color
   - Background: Black (0, 0, 0, 255)
   - Projection: Orthographic
   - Size: 4
   - Position: (640, 400, -10)

#### Create Cellular Automaton GameObject:
1. In Hierarchy, right-click → Create Empty
2. Rename to `CellularAutomaton`
3. **Add Script**: Click "Add Component" → search "CellularAutomaton" → Add
4. **Add Script**: Click "Add Component" → search "InputManager" → Add
5. **Add Script**: Click "Add Component" → search "UIManager" → Add

#### Configure Components:

**CellularAutomaton Component:**
- Drag `GameConfig` asset to the `Config` field
- Matrix Renderer: (will auto-create, leave empty)

**InputManager Component:**
- Max Brush Size: 50
- Min Brush Size: 1
- Brush Size: 5
- Brush Type: Circle
- Cellular Automaton: Drag the same GameObject (self-reference)
- Main Camera: Drag Main Camera from Hierarchy

**UIManager Component:**
- Cellular Automaton: Drag the same GameObject (self-reference)

### 4. Test Run

1. Press **Play** ▶️
2. You should see:
   - Black screen with a stone floor at the bottom
   - FPS counter in top-left
   - Help window showing controls
3. **Left-click** to draw sand
4. Press **2** to switch to water
5. Scroll wheel to change brush size

## Troubleshooting

### "Script is missing" errors
- Make sure all scripts compiled successfully
- Check Console for compilation errors
- Verify namespace usage: `using FallingSand.Core;` etc.

### Black screen, nothing happens
- Check that GameConfig is assigned
- Check Console for runtime errors
- Verify Camera position and orthographic size

### Can't draw elements
- Verify InputManager has references set
- Check that Camera is tagged as "MainCamera"
- Ensure scene has CellularAutomaton with all components

### Performance issues
- Reduce screen size in GameConfig
- Increase Pixel Size Modifier (makes fewer cells)
- Disable chunks temporarily with 'C' key

## Quick Start Summary

After Unity finishes importing:

```
1. Create → FallingSand → Game Config (name it GameConfig)
2. Create New 2D Scene → Save as MainScene
3. Create Empty GameObject → Add CellularAutomaton, InputManager, UIManager
4. Assign GameConfig to CellularAutomaton
5. Assign references in InputManager and UIManager
6. Press Play!
```

## Controls

- **Mouse**: Left-click to draw, scroll for brush size
- **1-9**: Select elements (Sand, Water, Stone, Wood, Lava, Gunpowder, Oil, Acid, Snow)
- **0**: Eraser
- **Space**: Pause
- **C**: Clear all
- **B**: Toggle brush type
- **W**: Toggle weather

Enjoy your falling sand simulation!

