# System Settings Reference

> **Complete guide to all configuration parameters, gravity settings, performance tuning, and system behavior.**

---

## 📖 Table of Contents

1. [GameConfig Asset](#gameconfig-asset)
2. [Screen & Matrix Settings](#screen--matrix-settings)
3. [Gravity & Physics](#gravity--physics)
4. [Threading Configuration](#threading-configuration)
5. [Chunk System](#chunk-system)
6. [Performance Tuning](#performance-tuning)
7. [Element Properties](#element-properties)
8. [Frame Timing](#frame-timing)
9. [Best Practices](#best-practices)

---

## GameConfig Asset

The `GameConfig` ScriptableObject is the **single source of truth** for all simulation settings.

**Location:** `Assets/GameConfig.asset`
**Script:** `Assets/Scripts/Core/GameConfig.cs`

### Creating GameConfig

1. **Right-click** in Project window (Assets folder)
2. **Create → FallingSand → Game Config**
3. **Name it** `GameConfig`
4. **Assign** to `CellularAutomaton` component in scene

### Inspector View

```
┌─ Game Config ─────────────────────────────┐
│ Screen Settings                           │
│   Screen Width: 1280                      │
│   Screen Height: 800                      │
│   Pixel Size Modifier: 6                  │
│   Box2d Size Modifier: 10                 │
│                                            │
│ Physics                                   │
│   Gravity: (0, -5, 0)                    │
│                                            │
│ Threading                                 │
│   Num Threads: 12                         │
│   Use Multi Threading: ☑                  │
│                                            │
│ Optimization                              │
│   Use Chunks: ☑                           │
│                                            │
│ Performance Tuning                        │
│   Performance Pixel Size: 6 ━━━━━━━━○─   │
│                           (range: 4-12)   │
└───────────────────────────────────────────┘
```

---

## Screen & Matrix Settings

### Screen Width & Height

**Purpose:** Defines the pixel dimensions of the simulation canvas.

```csharp
public int screenWidth = 1280;  // Pixels
public int screenHeight = 800;  // Pixels
```

**Usage:**
- Determines Texture2D size
- Affects memory usage (screenWidth × screenHeight × 4 bytes)
- Does NOT affect Unity Game View size (use Camera instead)

**Typical Values:**
- **1280x800** - Default (16:10 aspect ratio)
- **1920x1080** - Full HD
- **800x600** - Low-end/testing

**Performance Impact:**
- Larger screen = more pixels to render
- **Minimal impact** with current rendering system (batch updates)

---

### Pixel Size Modifier

**Purpose:** Scale factor that determines matrix cell size.

```csharp
public int pixelSizeModifier = 6;  // Pixels per cell
```

**Formula:**
```
Matrix Width = Screen Width / Pixel Size Modifier
Matrix Height = Screen Height / Pixel Size Modifier
```

**Example (default):**
```
1280 / 6 = 213 cells wide
800 / 6 = 133 cells tall
Total cells: 213 × 133 = 28,329 elements
```

**Visual Impact:**
- **Lower value (4)** = Smaller cells = More detail = More elements = **SLOWER**
- **Higher value (10)** = Larger cells = Chunky look = Fewer elements = **FASTER**

**Performance Table:**

| Pixel Size | Matrix Size | Total Cells | Visual Quality | FPS Multiplier |
|------------|-------------|-------------|----------------|----------------|
| 4 | 320×200 | 64,000 | Very detailed | 1x (baseline) |
| 6 | 213×133 | 28,329 | Good (default) | ~2x |
| 8 | 160×100 | 16,000 | Moderate | ~4x |
| 10 | 128×80 | 10,240 | Chunky | ~6x |
| 12 | 106×66 | 6,996 | Very chunky | ~10x |

**Recommendation:**
- **Learning/Development**: 6-8 (balanced)
- **High Performance**: 10-12
- **Visual Showcase**: 4-6

**How to Change:**
1. Select `GameConfig` asset
2. Adjust **Pixel Size Modifier** slider
3. **Restart scene** (Library regenerates matrix)

---

### Box2D Size Modifier

**Purpose:** Scale factor for physics body generation (deprecated/unused in current version).

```csharp
public int box2dSizeModifier = 10;
```

**Status:** ⚠️ Not actively used in current physics system
**Legacy:** From Java/libGDX port (Box2D coordinate scaling)

---

## Gravity & Physics

### Gravity Vector

**Purpose:** Defines downward force direction and strength for all elements.

```csharp
public Vector3 gravity = new Vector3(0f, -5f, 0f);
```

**Components:**
- **X (default: 0)** - Horizontal gravity (normally zero)
- **Y (default: -5)** - Vertical gravity (negative = downward)
- **Z (default: 0)** - Depth gravity (unused in 2D)

**How Gravity Works:**
- Elements with mass > 0 are affected
- **MovableSolids** and **Liquids** fall downward
- **Gases** rise (negative mass effect)
- **ImmovableSolids** ignore gravity

**Adjusting Gravity:**

```csharp
// Normal Earth-like
gravity = (0, -5, 0)

// Low gravity (moon-like)
gravity = (0, -2, 0)

// High gravity
gravity = (0, -10, 0)

// Sideways gravity (experimental)
gravity = (-5, 0, 0)  // Falls left

// Zero gravity (float)
gravity = (0, 0, 0)
```

**Performance Note:** Gravity strength does NOT affect performance—only visual behavior.

---

## Threading Configuration

### Num Threads

**Purpose:** Number of threads for parallel element stepping.

```csharp
public int numThreads = 12;
```

**Status:** ⚠️ **NOT YET IMPLEMENTED**
**Current Behavior:** Single-threaded (main thread only)

**Future Use:**
- When Unity Jobs System is implemented
- Will divide matrix into `numThreads` chunks
- Each thread processes its chunk in parallel

**Recommendation:** Set to CPU core count (typically 4-16).

---

### Use Multi Threading

**Purpose:** Enable/disable multi-threading.

```csharp
public bool useMultiThreading = true;
```

**Status:** ⚠️ **NOT YET IMPLEMENTED**
**Current Behavior:** Ignored (always single-threaded)

**Future Use:**
- Toggle for Jobs System
- Useful for debugging (single-thread = easier to debug)

---

## Chunk System

### Use Chunks

**Purpose:** Enable/disable spatial partitioning optimization.

```csharp
public bool useChunks = true;
```

**How It Works:**
1. Matrix divided into **16×16 cell chunks**
2. Each chunk tracks if it has **active elements**
3. Only active chunks step each frame
4. Chunks deactivate when all elements settle

**Performance Impact:**

| State | Without Chunks | With Chunks |
|-------|----------------|-------------|
| **Empty screen** | 60 FPS | 200+ FPS |
| **Full active screen** | 30 FPS | 30 FPS (no difference) |
| **Settled sand pile** | 30 FPS | 120+ FPS |

**When to Disable:**
- Debugging (see all elements step)
- Very small matrices (< 50×50 cells)
- Testing worst-case performance

**Chunk Math:**
```
Matrix: 213×133 cells
Chunk size: 16×16 cells

Chunks needed:
  Columns: ⌈213/16⌉ = 14 chunks
  Rows: ⌈133/16⌉ = 9 chunks
  Total: 14 × 9 = 126 chunks
```

**Active Chunk Tracking:**
- Element moves → mark chunk active + neighbors
- Chunk active this frame → step next frame
- All elements settled → chunk goes inactive

**Code Location:** `Assets/Scripts/Util/Chunk.cs`

---

## Performance Tuning

### Performance Pixel Size

**Purpose:** Quick slider for adjusting `pixelSizeModifier` with visual feedback.

```csharp
[Range(4, 12)]
public int performancePixelSize = 6;
```

**Functionality:** Duplicate of `pixelSizeModifier` with inspector tooltip.

**Tooltip:** "Higher = smaller matrix = better FPS"

---

### Computed Properties

These are **read-only** calculated values:

```csharp
public int MatrixWidth => screenWidth / pixelSizeModifier;
public int MatrixHeight => screenHeight / pixelSizeModifier;
```

**Usage in Code:**
```csharp
int width = config.MatrixWidth;   // 213 (default)
int height = config.MatrixHeight; // 133 (default)
```

---

## Element Properties

While not in `GameConfig`, these are **global element behaviors** defined in `Element.cs`:

### Default Element Values

```csharp
// Health & Damage
health = 500;
explosionResistance = 1;
explosionRadius = 0;

// Fire & Heat
flammabilityResistance = 100;
resetFlammabilityResistance = 50;  // Half of flammability
isIgnited = false;
heatFactor = 10;
fireDamage = 3;
temperature = 0;
coolingFactor = 5;

// Physics
mass = 50;  // Varies by element type
frictionFactor = 0.8f;  // Solids
inertialResistance = 0.7f;

// Lifecycle
lifeSpan = null;  // Immortal by default
isDead = false;
```

### Element-Specific Overrides

Each element class overrides these in its constructor:

**Example - Sand:**
```csharp
public Sand(int x, int y) : base(x, y)
{
    mass = 50;
    // All other properties use base defaults
}
```

**Example - Lava:**
```csharp
public Lava(int x, int y) : base(x, y)
{
    mass = 50;
    dispersionRate = 2;
    temperature = 1000;
    heatFactor = 50;
}
```

See [ELEMENTS_REFERENCE.md](ELEMENTS_REFERENCE.md) for all element properties.

---

## Frame Timing

### Frame Modulation

The simulation uses a **4-frame cycle** for performance:

```csharp
// In CellularAutomaton.cs
frameCount = (frameCount + 1) % 4;  // 0, 1, 2, 3, repeat
```

**Frame 0:**
- Shuffle X-indexes (randomize step order)
- Normal element stepping

**Frame 1 (Effects Frame):**
- Fire spread
- Heat transfer
- Visual effects
```csharp
bool IsEffectsFrame() => frameCount == 1;
```

**Frame 2:**
- Normal element stepping

**Frame 3 (Reaction Frame):**
- Chemical reactions (acid corrosion)
- Heavy interactions
```csharp
bool IsReactionFrame() => frameCount == 3;
```

**Why Modulation?**
- Not all effects need every-frame updates
- Spreads CPU load evenly
- 10-15% FPS improvement

---

## Best Practices

### Performance Optimization Workflow

**Step 1: Identify Bottleneck**
Use Unity Profiler (Window → Analysis → Profiler):
```
CellularAutomaton.Update
├─ StepAll              ← Usually 60-70% of time
├─ RenderMatrix         ← Usually 20-30% of time
└─ ExecuteExplosions    ← Spikes during blasts
```

**Step 2: Tune Settings**

Priority order for FPS gains:

1. **Increase Pixel Size** (biggest impact)
   - 6 → 8 = ~2x FPS
   - 6 → 10 = ~3x FPS

2. **Reduce Screen Size** (if visual size allows)
   - 1280×800 → 800×600 = ~1.5x FPS

3. **Ensure Chunks Enabled**
   - Critical for settled simulations
   - 2-10x FPS for static scenes

4. **Reduce Element Count**
   - Clear screen regularly with 'C' key
   - Limit continuous element spawning

**Step 3: Measure Results**
- Target: **60 FPS** minimum
- Good: **120+ FPS** for settled scenes
- Excellent: **200+ FPS** for empty/static

---

### Configuration Presets

#### Preset 1: High Quality (Showcase)
```yaml
Screen Width: 1920
Screen Height: 1080
Pixel Size Modifier: 4
Use Chunks: true
Expected FPS: 30-60 (full screen), 60-120 (settled)
```

#### Preset 2: Balanced (Default)
```yaml
Screen Width: 1280
Screen Height: 800
Pixel Size Modifier: 6
Use Chunks: true
Expected FPS: 60-120 (full screen), 120+ (settled)
```

#### Preset 3: High Performance
```yaml
Screen Width: 800
Screen Height: 600
Pixel Size Modifier: 10
Use Chunks: true
Expected FPS: 120-200 (full screen), 300+ (settled)
```

#### Preset 4: Mobile/Low-End
```yaml
Screen Width: 640
Screen Height: 480
Pixel Size Modifier: 12
Use Chunks: true
Expected FPS: 200+ (all scenarios)
```

---

### Memory Considerations

**Memory Usage Formula:**
```
Matrix Memory = (Width/PixelSize) × (Height/PixelSize) × ElementSize
Texture Memory = Width × Height × 4 bytes (RGBA32)
```

**Example (default 1280×800, pixelSize=6):**
```
Matrix: 213 × 133 = 28,329 cells
  Each Element: ~200 bytes (C# object overhead)
  Total: 28,329 × 200 = ~5.6 MB

Texture: 1280 × 800 × 4 = 4.096 MB

Total: ~10 MB (plus Unity overhead)
```

**Memory is NOT a bottleneck** for this project—CPU is the limiting factor.

---

### Debugging Settings

**For Development:**
```yaml
Pixel Size Modifier: 8       # Larger = easier to see individual cells
Use Chunks: false            # See all elements step (helpful for debugging)
Screen Size: 800×600         # Smaller window = easier to monitor
```

**For Production:**
```yaml
Pixel Size Modifier: 6-8     # Balance quality/performance
Use Chunks: true             # Always enable for performance
Screen Size: 1280×800+       # Full resolution
```

---

### Testing Different Scenarios

**Empty Screen Test:**
- Press 'C' to clear
- Expected: 200+ FPS

**Full Active Screen Test:**
- Draw sand across entire screen
- Expected: 30-60 FPS (worst case)

**Settled Physics Test:**
- Create large sand pile
- Wait for settling
- Expected: 120+ FPS (chunks deactivate)

**Explosion Stress Test:**
- Fill screen with gunpowder
- Ignite with lava
- Expected: Frame drops during explosion (normal), recovery after

---

## Configuration Code Reference

### GameConfig.cs Full Source

```csharp
using UnityEngine;

namespace FallingSand.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "FallingSand/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Screen Settings")]
        public int screenWidth = 1280;
        public int screenHeight = 800;
        public int pixelSizeModifier = 6;
        public int box2dSizeModifier = 10;
        
        [Header("Physics")]
        public Vector3 gravity = new Vector3(0f, -5f, 0f);
        
        [Header("Threading")]
        public int numThreads = 12;
        public bool useMultiThreading = true;
        
        [Header("Optimization")]
        public bool useChunks = true;
        
        [Header("Performance Tuning")]
        [Tooltip("Higher = smaller matrix = better FPS")]
        [Range(4, 12)]
        public int performancePixelSize = 6;
        
        public int MatrixWidth => screenWidth / pixelSizeModifier;
        public int MatrixHeight => screenHeight / pixelSizeModifier;
    }
}
```

**Code Location:** `Assets/Scripts/Core/GameConfig.cs`

---

## Related Documentation

- **Element behaviors:** [ELEMENTS_REFERENCE.md](ELEMENTS_REFERENCE.md)
- **Code architecture:** [ARCHITECTURE.md](ARCHITECTURE.md)
- **Rendering details:** [RENDER_PIPELINE.md](RENDER_PIPELINE.md)
- **FPS optimization:** [PERFORMANCE_OPTIMIZATIONS.md](PERFORMANCE_OPTIMIZATIONS.md)

---

**Summary:** The `GameConfig` asset centralizes all simulation settings. Adjust `pixelSizeModifier` for the biggest performance impact. Enable `useChunks` for optimal FPS. Gravity is purely visual—tune for desired fall speed.
