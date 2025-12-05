# Architecture Reference

> **Complete code structure, design patterns, namespace organization, and development guidelines for the Falling Sand simulation.**

---

## 📖 Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Namespace Organization](#namespace-organization)
3. [Core Systems](#core-systems)
4. [Design Patterns](#design-patterns)
5. [Data Flow](#data-flow)
6. [Class Hierarchy](#class-hierarchy)
7. [Key Interfaces](#key-interfaces)
8. [Extension Points](#extension-points)
9. [Development Guidelines](#development-guidelines)

---

## Architecture Overview

### High-Level Architecture

```
┌─────────────────────────────────────────────────┐
│           Unity MonoBehaviour Layer             │
│  (CellularAutomaton, InputManager, UIManager)  │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│              Core Layer                         │
│  (CellularMatrix, GameConfig, MatrixRenderer)  │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│           Element System Layer                  │
│  (Element base + 30+ concrete implementations)  │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│        Support Systems Layer                    │
│  (Physics, Rendering, Utilities, Entities)     │
└─────────────────────────────────────────────────┘
```

### Design Philosophy

**Principles:**
1. **Modularity**: Small, focused classes with single responsibilities
2. **Inheritance**: Clear class hierarchies (Element → Solid → MovableSolid → Sand)
3. **Composition**: Systems composed of smaller components
4. **Data-Driven**: Configuration through ScriptableObject (GameConfig)
5. **Performance-Conscious**: Optimization without sacrificing readability

---

## Namespace Organization

### Namespace Tree

```
FallingSand
├── FallingSand.Core
│   ├── CellularAutomaton.cs        # Main simulation controller
│   ├── CellularMatrix.cs           # 2D element grid
│   └── GameConfig.cs                # Configuration ScriptableObject
│
├── FallingSand.Elements
│   ├── Element.cs                   # Abstract base class
│   ├── ElementType.cs               # Enum factory pattern
│   ├── EmptyCell.cs                 # Singleton empty cell
│   ├── ColorConstants.cs            # Element color definitions
│   ├── EffectColors.cs              # Fire/effect colors
│   │
│   ├── Solid.cs                     # Abstract solid base
│   │   ├── MovableSolid.cs          # Falling solids
│   │   │   ├── Sand.cs, Dirt.cs, Snow.cs, Gunpowder.cs, Coal.cs, Ember.cs
│   │   └── ImmovableSolid.cs        # Static solids
│   │       ├── Stone.cs, Brick.cs, Wood.cs, Ground.cs, Titanium.cs, SlimeMold.cs
│   │
│   ├── Liquid.cs                    # Abstract liquid base
│   │   ├── Water.cs, Oil.cs, Acid.cs, Lava.cs, Blood.cs, Cement.cs
│   │
│   ├── Gas.cs                       # Base gas class
│   │   ├── Steam.cs, Smoke.cs, Spark.cs, ExplosionSpark.cs, FlammableGas.cs
│   │
│   └── Player/
│       └── PlayerMeat.cs            # Player element
│
├── FallingSand.Rendering
│   └── MatrixRenderer.cs            # Texture2D rendering
│
├── FallingSand.Physics
│   ├── PhysicsElementActor.cs       # Rigidbody2D manager
│   ├── ShapeFactory.cs              # Collider generation
│   ├── MarchingSquares/             # Contour extraction
│   └── DouglasPeucker/              # Line simplification
│
├── FallingSand.InputSystem
│   └── InputManager.cs              # Mouse/keyboard input
│
├── FallingSand.UI
│   └── UIManager.cs                 # On-screen UI
│
├── FallingSand.Systems
│   ├── WeatherSystem.cs             # Sky spawning
│   └── SpoutSystem.cs               # Particle emitters
│
├── FallingSand.Entities
│   ├── Player.cs                    # Player character
│   └── Boid.cs                      # Flocking AI
│
└── FallingSand.Util
    └── Chunk.cs                     # Spatial partitioning
```

---

## Core Systems

### 1. CellularAutomaton (Master Controller)

**Responsibility:** Main game loop and system orchestration.

**Location:** `Assets/Scripts/Core/CellularAutomaton.cs`

**Key Methods:**
```csharp
void Start()
    ├─ Initialize Camera
    ├─ Create CellularMatrix
    ├─ Initialize MatrixRenderer
    └─ Setup initial elements (ground)

void Update()
    ├─ HandleInput()
    ├─ frameCount = (frameCount + 1) % 4  // Frame modulation
    ├─ matrix.ReshuffleXIndexes()         // Every 4 frames
    ├─ matrix.StepAll()                   // Element simulation
    ├─ matrix.ExecuteExplosions()         // Deferred explosions
    └─ matrixRenderer.RenderMatrix()      // Draw to screen
```

**Singleton Pattern:**
```csharp
public static CellularAutomaton Instance { get; private set; }

void Awake()
{
    if (Instance == null)
        Instance = this;
    else
        Destroy(gameObject);
}
```

**Purpose:** Global access for elements (e.g., `CellularAutomaton.frameCount`).

---

### 2. CellularMatrix (Data Model)

**Responsibility:** Manages 2D element grid, chunk system, and element operations.

**Location:** `Assets/Scripts/Core/CellularMatrix.cs`

**Core Data:**
```csharp
private Element[,] matrix;              // 2D element grid
private Chunk[,] chunks;                // Spatial partitioning
private List<int> shuffledXIndexes;     // Randomized step order
public List<Explosion> explosionArray;  // Deferred explosions
```

**Key Methods:**
```csharp
Element Get(int x, int y)
    └─ Returns element at matrix coordinates

void SetElementAtIndex(int x, int y, Element element)
    └─ Places element in grid

Element SpawnElementByMatrix(int x, int y, ElementType type)
    ├─ Kill existing element
    ├─ Create new element
    └─ Report to chunk system

void StepAll()
    ├─ Loop through all cells (shuffled X order)
    ├─ Call element.Step(matrix)
    └─ Reset stepped flags

void ReshuffleXIndexes()
    └─ Fisher-Yates shuffle (randomize X order)

Chunk GetChunkForCoordinates(int x, int y)
    └─ Find chunk containing coordinates
```

**Coordinate Systems:**
```csharp
int ToMatrix(int pixelVal)   // Pixel → Matrix
int ToPixel(int matrixVal)   // Matrix → Pixel
```

---

### 3. GameConfig (Configuration)

**Responsibility:** ScriptableObject holding all simulation settings.

**Location:** `Assets/Scripts/Core/GameConfig.cs`

**Properties:**
```csharp
// Screen
int screenWidth, screenHeight
int pixelSizeModifier

// Physics
Vector3 gravity

// Optimization
bool useChunks

// Computed
int MatrixWidth => screenWidth / pixelSizeModifier
int MatrixHeight => screenHeight / pixelSizeModifier
```

**Usage:**
```csharp
// In any script:
int width = CellularAutomaton.Instance.config.MatrixWidth;
```

See [SYSTEM_SETTINGS.md](SYSTEM_SETTINGS.md) for detailed configuration.

---

### 4. Element System (Polymorphic Hierarchy)

**Responsibility:** Define element behaviors through inheritance.

**Base Class:** `Element` (abstract)

**Location:** `Assets/Scripts/Elements/Element.cs`

**Key Abstract Methods:**
```csharp
public abstract void Step(CellularMatrix matrix);
protected abstract bool ActOnNeighboringElement(...);
public abstract ElementType GetEnumType();
```

**Shared Properties:**
```csharp
// Position
int matrixX, matrixY
int pixelX, pixelY

// Physics
int mass
Vector3 vel
float frictionFactor

// Health
int health
int explosionResistance

// Fire
int flammabilityResistance
bool isIgnited
int temperature

// Visual
Color32 color

// Lifecycle
bool stepped
bool isDead
int? lifeSpan
```

**Shared Methods:**
```csharp
void SwapPositions(CellularMatrix matrix, Element toSwap)
void Die(CellularMatrix matrix)
bool ReceiveHeat(CellularMatrix matrix, int heat)
bool ReceiveCooling(CellularMatrix matrix, int cooling)
bool Explode(CellularMatrix matrix, int strength)
```

See [ELEMENTS_REFERENCE.md](ELEMENTS_REFERENCE.md) for all elements.

---

## Design Patterns

### 1. Factory Pattern (ElementType Enum)

**Purpose:** Create elements without hard-coding constructors everywhere.

**Location:** `Assets/Scripts/Elements/ElementType.cs`

**Usage:**
```csharp
public enum ElementType
{
    EMPTYCELL,
    SAND,
    WATER,
    STONE,
    // ... etc
}

public static class ElementTypeExtensions
{
    public static Element CreateElementByMatrix(this ElementType type, int x, int y)
    {
        switch (type)
        {
            case ElementType.SAND:
                return new Sand(x, y);
            case ElementType.WATER:
                return new Water(x, y);
            // ... etc
        }
    }
    
    public static System.Type GetType(this ElementType type)
    {
        // Returns C# Type for type comparisons
    }
}
```

**Benefits:**
- Single place to add new elements
- Type-safe element creation
- Easy serialization (enum instead of class references)

---

### 2. Singleton Pattern (EmptyCell)

**Purpose:** Only one instance of EmptyCell needed (memory efficiency).

**Location:** `Assets/Scripts/Elements/EmptyCell.cs`

**Implementation:**
```csharp
public class EmptyCell : Element
{
    private static EmptyCell instance;
    
    public static EmptyCell Instance
    {
        get
        {
            if (instance == null)
                instance = new EmptyCell(0, 0);
            return instance;
        }
    }
    
    private EmptyCell(int x, int y) : base(x, y) { }
    
    public override void Step(CellularMatrix matrix) 
    {
        // Do nothing
    }
}
```

**Usage:**
```csharp
// Instead of: new EmptyCell(x, y)
Element empty = EmptyCell.Instance;
```

**Why?** Thousands of EmptyCell instances would waste memory—use one shared instance.

---

### 3. Strategy Pattern (Element Behaviors)

**Purpose:** Different movement strategies via inheritance.

**Example:**
```csharp
// MovableSolid strategy: Try down, then diagonals
protected override void TryMove(CellularMatrix matrix)
{
    if (belowIsEmpty) moveDown();
    else if (diagonalEmpty) moveDiagonal();
}

// Liquid strategy: Try down, then horizontal dispersion
public override void Step(CellularMatrix matrix)
{
    if (belowIsEmpty) moveDown();
    else disperseHorizontally();
}

// Gas strategy: Try up, then horizontal dispersion
public override void Step(CellularMatrix matrix)
{
    if (aboveIsEmpty) moveUp();
    else disperseHorizontally();
}
```

---

### 4. Template Method Pattern (Step Lifecycle)

**Purpose:** Define algorithm structure, let subclasses override steps.

**Example:**
```csharp
// In Element base class
public abstract void Step(CellularMatrix matrix);

// In Solid class
public override void Step(CellularMatrix matrix)
{
    if (ChunkOptimizationSaysSkip()) return;
    if (AlreadyStepped()) return;
    
    stepped = true;
    
    TryMove(matrix);  // ← Abstract, subclasses define
    
    ReportMovementToChunks();
}

// In MovableSolid class
protected override void TryMove(CellularMatrix matrix)
{
    // Specific falling logic
}
```

---

### 5. Observer Pattern (Chunk Activation)

**Purpose:** Notify chunk system when elements move.

**Implementation:**
```csharp
// In Element movement
public void SwapPositions(CellularMatrix matrix, Element toSwap, int x, int y)
{
    matrix.SetElementAtIndex(this.GetMatrixX(), this.GetMatrixY(), toSwap);
    matrix.SetElementAtIndex(x, y, this);
    
    // Notify observer (chunk system)
    matrix.ReportToChunkActive(this);
}

// In CellularMatrix
public void ReportToChunkActive(Element element)
{
    Chunk chunk = GetChunkForElement(element);
    chunk.SetShouldStepNextFrame(true);
    
    // Activate neighboring chunks too
    ActivateNeighborChunks(chunk);
}
```

---

## Data Flow

### Main Update Loop

```
┌─ Frame Start ────────────────────────────────────┐
│                                                   │
│  CellularAutomaton.Update()                      │
│  │                                                │
│  ├─ HandleInput()                                │
│  │   └─ InputManager.ProcessInput()             │
│  │       ├─ Mouse input → SpawnElement()        │
│  │       └─ Keyboard → Toggle settings          │
│  │                                                │
│  ├─ frameCount = (frameCount + 1) % 4           │
│  │                                                │
│  ├─ matrix.ResetChunks()                         │
│  │   └─ chunks[r,c].ShiftShouldStepAndReset()  │
│  │                                                │
│  ├─ if (frameCount == 0) ReshuffleXIndexes()   │
│  │   └─ Fisher-Yates shuffle                    │
│  │                                                │
│  ├─ matrix.StepAll()                             │
│  │   └─ FOR each element:                       │
│  │       ├─ element.Step(matrix)                │
│  │       │   ├─ Movement logic                  │
│  │       │   ├─ Neighbor interactions           │
│  │       │   └─ Custom behaviors                │
│  │       └─ element.stepped = false             │
│  │                                                │
│  ├─ matrix.ExecuteExplosions()                   │
│  │   └─ FOR each explosion:                     │
│  │       └─ Destroy elements in radius          │
│  │                                                │
│  └─ matrixRenderer.RenderMatrix()                │
│      ├─ FOR each element:                       │
│      │   └─ Fill pixel block with color         │
│      ├─ texture.SetPixels32(pixels)             │
│      └─ texture.Apply()                          │
│                                                   │
│  Frame End                                       │
└───────────────────────────────────────────────────┘
```

---

## Class Hierarchy

### Element Inheritance Tree

```
Element (abstract)
│
├─ EmptyCell (singleton)
│
├─ Solid (abstract)
│   │
│   ├─ MovableSolid (abstract)
│   │   ├─ Sand
│   │   ├─ Dirt
│   │   ├─ Snow
│   │   ├─ Gunpowder
│   │   ├─ Coal
│   │   └─ Ember
│   │
│   └─ ImmovableSolid (abstract)
│       ├─ Stone
│       ├─ Brick
│       ├─ Wood
│       ├─ Ground
│       ├─ Titanium
│       └─ SlimeMold
│
├─ Liquid (abstract)
│   ├─ Water
│   ├─ Oil
│   ├─ Acid
│   ├─ Lava
│   ├─ Blood
│   └─ Cement
│
└─ Gas
    ├─ Steam
    ├─ Smoke
    ├─ Spark
    ├─ ExplosionSpark
    └─ FlammableGas
```

**Special:** `PlayerMeat` inherits directly from `Element` (not Gas/Liquid/Solid).

---

## Key Interfaces

### Element "Interface" (Abstract Methods)

While C# interfaces aren't explicitly used, the abstract `Element` class defines a contract:

```csharp
public abstract class Element
{
    // Must implement:
    public abstract void Step(CellularMatrix matrix);
    protected abstract bool ActOnNeighboringElement(...);
    public abstract ElementType GetEnumType();
    
    // Can override:
    public virtual void CustomElementFunctions(CellularMatrix matrix) { }
    public virtual bool ActOnOther(Element other, CellularMatrix matrix) { return false; }
    public virtual void ModifyColor() { }
}
```

---

## Extension Points

### Adding a New Element

**Step 1:** Create element class

```csharp
using FallingSand.Core;
using FallingSand.Elements;

namespace FallingSand.Elements
{
    public class MyNewElement : MovableSolid  // Or Liquid, Gas, etc.
    {
        public MyNewElement(int x, int y) : base(x, y)
        {
            mass = 45;
            health = 600;
            flammabilityResistance = 70;
            explosionResistance = 3;
        }
        
        // Optional: Custom behavior
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);  // Inherit base movement
            
            // Add custom CA rules here
            if (IsEffectsFrame())
            {
                // Custom interaction logic
            }
        }
        
        public override ElementType GetEnumType()
        {
            return ElementType.MYNEW;
        }
    }
}
```

**Step 2:** Add to ElementType enum

```csharp
// In ElementType.cs
public enum ElementType
{
    // ... existing types ...
    MYNEW,
}
```

**Step 3:** Register in factory

```csharp
// In ElementTypeExtensions.CreateElementByMatrix()
case ElementType.MYNEW:
    return new MyNewElement(x, y);
```

**Step 4:** Add color

```csharp
// In ColorConstants.cs
case ElementType.MYNEW:
    return new Color32(128, 64, 200, 255);  // Purple
```

**Step 5:** (Optional) Bind to input key

```csharp
// In InputManager.cs
if (Input.GetKeyDown(KeyCode.Alpha9))
{
    currentElementType = ElementType.MYNEW;
}
```

---

### Adding a New System

**Example:** Add a "Electricity" system

**Step 1:** Create system class

```csharp
using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Systems
{
    public class ElectricitySystem : MonoBehaviour
    {
        private CellularMatrix matrix;
        
        public void Initialize(CellularMatrix matrix)
        {
            this.matrix = matrix;
        }
        
        public void Update()
        {
            // Propagate electricity through conductive elements
        }
    }
}
```

**Step 2:** Integrate with CellularAutomaton

```csharp
// In CellularAutomaton.cs
[Header("Systems")]
public ElectricitySystem electricitySystem;

void Start()
{
    // ... existing initialization ...
    electricitySystem = gameObject.AddComponent<ElectricitySystem>();
    electricitySystem.Initialize(matrix);
}

void Update()
{
    // ... existing updates ...
    electricitySystem.Update();
}
```

---

## Development Guidelines

### Coding Standards

**Naming Conventions:**
- Classes: `PascalCase` (e.g., `CellularMatrix`)
- Methods: `PascalCase` (e.g., `StepAll()`)
- Fields: `camelCase` (e.g., `matrixX`)
- Constants: `UPPER_SNAKE_CASE` (e.g., `ElementType.EMPTYCELL`)

**File Organization:**
```csharp
// 1. Using statements
using UnityEngine;
using FallingSand.Core;

// 2. Namespace
namespace FallingSand.Elements
{
    // 3. Class documentation (optional)
    /// <summary>
    /// Falling sand element with mass-based displacement.
    /// </summary>
    public class Sand : MovableSolid
    {
        // 4. Constructor
        public Sand(int x, int y) : base(x, y)
        {
            mass = 50;
        }
        
        // 5. Methods
        public override ElementType GetEnumType() => ElementType.SAND;
    }
}
```

---

### Performance Guidelines

**DO:**
- ✅ Use `element.elementType == ElementType.X` (enum comparison)
- ✅ Pre-calculate values outside loops
- ✅ Use `Color32` instead of `Color`
- ✅ Early return when possible
- ✅ Profile before optimizing

**DON'T:**
- ❌ Use `element is Sand` (slow reflection)
- ❌ Allocate new objects in Update() loops
- ❌ Use string comparisons
- ❌ Deep nested loops without optimization
- ❌ Premature optimization

---

### Testing Strategy

**Manual Testing:**
1. Empty screen test (FPS baseline)
2. Full screen active test (worst case)
3. Settled simulation test (chunk optimization)
4. Element interaction test (reactions work)
5. Explosion stress test (performance under load)

**Profiling:**
```csharp
// In Unity Profiler
CellularAutomaton.Update
├─ StepAll()           [Target: <12ms]
├─ RenderMatrix()      [Target: <6ms]
└─ ExecuteExplosions() [Spikes OK]
```

---

### Git Workflow

**Branch Naming:**
- `feature/element-ice` - New element
- `fix/explosion-crash` - Bug fix
- `refactor/element-base` - Code restructure
- `docs/architecture-guide` - Documentation

**Commit Messages:**
```
feat(elements): add Ice element with freezing behavior
fix(rendering): prevent null texture crash
refactor(core): simplify chunk activation logic
docs(readme): update quick start guide
perf(matrix): optimize shuffle to every 4 frames
```

---

## Related Documentation

- **Element CA rules:** [ELEMENTS_REFERENCE.md](ELEMENTS_REFERENCE.md)
- **Configuration settings:** [SYSTEM_SETTINGS.md](SYSTEM_SETTINGS.md)
- **Rendering details:** [RENDER_PIPELINE.md](RENDER_PIPELINE.md)
- **Physics algorithms:** [PHYSICS_SYSTEM.md](PHYSICS_SYSTEM.md)

---

**Summary:** The architecture follows object-oriented principles with clear hierarchies, modular systems, and performance-conscious design. New elements and systems can be added through well-defined extension points. Code is organized into logical namespaces for maintainability.
