# Elements Reference Guide

> **Complete documentation of all 30+ elements, their cellular automata (CA) rules, physics properties, and chemical interactions.**

---

## 📖 Table of Contents

1. [Understanding Cellular Automata Rules](#understanding-cellular-automata-rules)
2. [Element Categories](#element-categories)
3. [Base Properties](#base-properties)
4. [Movable Solids](#-movable-solids)
5. [Immovable Solids](#-immovable-solids)
6. [Liquids](#-liquids)
7. [Gases](#-gases)
8. [Special Elements](#-special-elements)
9. [Interaction Matrix](#interaction-matrix)

---

## Understanding Cellular Automata Rules

### What is a Cellular Automaton?
A **cellular automaton (CA)** is a grid of cells where each cell follows simple local rules based on its neighbors. Despite simple rules, complex emergent behaviors appear.

**Key Concepts:**
- **State**: Each cell contains an Element (or EmptyCell)
- **Neighborhood**: Each element checks adjacent cells (usually 3x3 or 5x5)
- **Rules**: Simple "if-then" logic executed every frame
- **Emergence**: Complex patterns emerge from simple interactions

### Our CA System

**Step Order** (every frame):
1. **Shuffle X-indexes** (every 4 frames) - prevents left/right bias
2. **Step all elements** (bottom-to-top, random X order)
3. **Custom behaviors** (fire spread, reactions, etc.)
4. **Execute explosions** (deferred until all elements stepped)
5. **Reset "stepped" flags** (prepare for next frame)

**Movement Rules:**
- Elements try to move to **EmptyCell** spaces first
- **Mass-based displacement**: Heavier elements push lighter ones
- **Velocity threshold**: Elements "settle" after minimal movement
- **Chunk optimization**: Only active regions update

---

## Element Categories

### Category Hierarchy

```
Element (abstract base)
├── Solid (abstract)
│   ├── MovableSolid (abstract) - Falls due to gravity
│   │   ├── Sand, Dirt, Snow, Gunpowder, Coal, Ember
│   └── ImmovableSolid (abstract) - Static structures
│       ├── Stone, Brick, Wood, Ground, Titanium, SlimeMold
├── Liquid (abstract) - Flows down and sideways
│   ├── Water, Oil, Acid, Lava, Blood, Cement
├── Gas (abstract) - Rises upward
│   ├── Steam, Smoke, Spark, ExplosionSpark, FlammableGas, Gas
└── Special
    ├── EmptyCell (singleton) - Represents empty space
    └── PlayerMeat - Player character element
```

---

## Base Properties

All elements inherit these properties from the `Element` base class:

### Position & Physics
```csharp
int matrixX, matrixY        // Grid position
Vector3 vel                 // Velocity (for physics integration)
float frictionFactor        // Movement damping (0-1)
float inertialResistance    // Resistance to external forces
int mass                    // Weight (heavier displaces lighter)
```

### Health & Damage
```csharp
int health                  // Hit points (default: 500)
int explosionResistance     // Withstands blast strength (1-20)
int explosionRadius         // Blast radius when this explodes (0-30)
```

### Fire & Heat
```csharp
int flammabilityResistance  // Points before ignition (default: 100)
bool isIgnited              // Currently on fire?
int heatFactor              // Heat transferred to neighbors (0-50)
int coolingFactor           // Cooling applied to ignited elements (0-20)
int temperature             // Internal temperature (0-1000)
```

### Visual
```csharp
Color32 color               // Display color (with variation)
bool discolored             // Has been darkened by damage?
```

### Lifecycle
```csharp
int? lifeSpan               // Frames until death (null = immortal)
bool isDead                 // Marked for removal?
bool stepped                // Already moved this frame?
```

---

## 🟤 Movable Solids

**Base CA Rule (MovableSolid):**
1. Try move **DOWN** (swap with EmptyCell or lighter element)
2. If blocked, try **DIAGONAL-LEFT** or **DIAGONAL-RIGHT** (random order)
3. If fully blocked, stay in place (element "settles")

### Sand
**Description:** Classic falling sand particle. Foundation of the simulation.

**Plain English:**
- Falls straight down if space is empty
- Slides diagonally if blocked below
- Piles up naturally into slopes
- Can displace liquids and gases (heavier than them)

**CA Rule:**
```
Every frame:
  IF cell below is empty → move down
  ELSE IF cell below is lighter (liquid/gas) → displace it
  ELSE IF diagonal cells empty → slide diagonally (random direction)
  ELSE → settle
```

**Properties:**
```csharp
Mass: 50
Health: 500 (default)
Explosion Resistance: 1 (very weak)
Flammability: Non-flammable
```

**Code Location:** `Assets/Scripts/Elements/Solid/Movable/Sand.cs`

---

### Dirt
**Description:** Slightly heavier than sand with darker color.

**Plain English:**
- Behaves like sand but heavier
- Forms more stable piles
- Cannot be displaced by sand

**CA Rule:** Same as Sand

**Properties:**
```csharp
Mass: 55 (heavier than sand)
Health: 500
Explosion Resistance: 1
Flammability: Non-flammable
Color: Brown variations
```

**Code Location:** `Assets/Scripts/Elements/Solid/Movable/Dirt.cs`

---

### Snow
**Description:** Cold solid that melts into water when heated.

**Plain English:**
- Falls like sand
- Has cooling effect on neighbors
- Melts into water if near heat source
- Extinguishes fires

**CA Rule:**
```
Every frame:
  Standard MovableSolid movement
  
  IF temperature > melting_point → Die and become Water
  
  Effects frame (every 4 frames):
    FOR each neighbor (3x3):
      IF neighbor is ignited → apply cooling
```

**Properties:**
```csharp
Mass: 45
Health: 300 (melts easily)
Cooling Factor: 15
Melting Point: 200 temperature
Color: White variations
```

**Code Location:** `Assets/Scripts/Elements/Solid/Movable/Snow.cs`

---

### Gunpowder
**Description:** Explosive solid that detonates when ignited.

**Plain English:**
- Falls like sand until ignited
- When on fire, has 20% chance per frame to explode
- Explosion destroys nearby elements
- Chain reaction: explodes neighboring gunpowder

**CA Rule:**
```
Every frame:
  Standard MovableSolid movement
  
  IF isIgnited AND random(0-1) > 0.8 → 
    Create explosion (radius: 15, strength: 5)
    Die (remove self)
```

**Properties:**
```csharp
Mass: 40
Health: 500
Explosion Radius: 15 cells
Explosion Strength: 5
Flammability Resistance: 50 (ignites easily)
Color: Dark gray
```

**Explosion Math:**
```
Radius: 15 cells
FOR each cell within circular radius:
  Distance = sqrt((x1-x2)² + (y1-y2)²)
  IF distance <= radius:
    IF element.explosionResistance < strength:
      element → Die or become ExplosionSpark
```

**Code Location:** `Assets/Scripts/Elements/Solid/Movable/Gunpowder.cs`

---

### Coal
**Description:** Slow-burning fuel that sustains fire.

**Plain English:**
- Falls like sand
- Burns slowly when ignited
- Doesn't explode (unlike gunpowder)
- Takes fire damage over time

**CA Rule:**
```
Every frame:
  Standard MovableSolid movement
  
  IF isIgnited → 
    Take fire damage (3 HP per frame)
    Spread heat to neighbors
    Eventually dies after burning
```

**Properties:**
```csharp
Mass: 50
Health: 1500 (burns for ~500 frames)
Fire Damage: 3 per frame
Flammability Resistance: 80 (needs heat to ignite)
Color: Black
```

**Code Location:** `Assets/Scripts/Elements/Solid/Movable/Coal.cs`

---

### Ember
**Description:** Hot glowing particles from fire.

**Plain English:**
- Falls slowly
- Glows orange/yellow
- Ignites flammable materials on contact
- Dies after short lifespan

**CA Rule:**
```
Every frame:
  Standard MovableSolid movement
  
  Lifespan countdown (dies after 200 frames)
  
  Effects frame:
    FOR each neighbor:
      IF neighbor is flammable → apply heat
```

**Properties:**
```csharp
Mass: 30 (light)
Health: 100
Lifespan: 200 frames (~3 seconds)
Heat Factor: 30
Temperature: 800
Color: Orange-yellow glow
```

**Code Location:** `Assets/Scripts/Elements/Solid/Movable/Ember.cs`

---

## 🏗️ Immovable Solids

**Base CA Rule (ImmovableSolid):**
- **Never moves** (TryMove() does nothing)
- Acts as terrain/obstacles
- Can be damaged by explosions, acid, etc.
- Some have special behaviors (fire spread, growth)

### Stone
**Description:** Strong, heat-resistant structure block.

**Plain English:**
- Completely immovable
- Very high health and explosion resistance
- Immune to fire
- Basic building material

**CA Rule:**
```
Every frame:
  Do nothing (immovable)
  
  IF health <= 0 → Die
```

**Properties:**
```csharp
Mass: 100
Health: 2000 (very durable)
Explosion Resistance: 15 (resists most blasts)
Flammability: Infinite (fireproof)
Color: Gray variations
```

**Code Location:** `Assets/Scripts/Elements/Solid/Immovable/Stone.cs`

---

### Brick
**Description:** Moderate strength construction material.

**Plain English:**
- Immovable structure
- Moderate health (weaker than stone)
- Resistant to fire but can be destroyed

**CA Rule:** Same as Stone (immovable, takes damage)

**Properties:**
```csharp
Mass: 100
Health: 1000 (moderate)
Explosion Resistance: 8
Flammability: Very high (fireproof)
Color: Red-brown
```

**Code Location:** `Assets/Scripts/Elements/Solid/Immovable/Brick.cs`

---

### Wood
**Description:** Flammable building material.

**Plain English:**
- Immovable structure
- Catches fire easily
- Burns slowly (like coal)
- Spreads fire to adjacent wood

**CA Rule:**
```
Every frame:
  Do nothing (immovable)
  
  IF isIgnited → 
    Take fire damage (2 HP/frame)
    Spread heat to neighbors
    Eventually turns to EmptyCell
```

**Properties:**
```csharp
Mass: 100
Health: 800
Explosion Resistance: 2 (weak to blasts)
Flammability Resistance: 60 (ignites easily)
Fire Damage: 2 per frame
Color: Brown
```

**Code Location:** `Assets/Scripts/Elements/Solid/Immovable/Wood.cs`

---

### Ground
**Description:** Basic terrain element.

**Plain English:**
- Immovable terrain
- Low health (easily destroyed)
- Acts as floor or landscape

**CA Rule:** Same as Stone (immovable, destructible)

**Properties:**
```csharp
Mass: 100
Health: 600 (weak)
Explosion Resistance: 3
Color: Dark brown
```

**Code Location:** `Assets/Scripts/Elements/Solid/Immovable/Ground.cs`

---

### Titanium
**Description:** Nearly indestructible material.

**Plain English:**
- Immovable and nearly immortal
- Extremely high explosion resistance
- Use for permanent barriers

**CA Rule:** Same as Stone (immovable)

**Properties:**
```csharp
Mass: 100
Health: 10000 (nearly indestructible)
Explosion Resistance: 50 (survives massive blasts)
Flammability: Infinite (fireproof)
Color: Metallic gray
```

**Code Location:** `Assets/Scripts/Elements/Solid/Immovable/Titanium.cs`

---

### Slime Mold
**Description:** Spreading organic material.

**Plain English:**
- Immovable but grows
- Spreads to adjacent EmptyCells slowly
- Creates organic patterns
- Can be destroyed by fire/acid

**CA Rule:**
```
Every frame:
  Do nothing (immovable)
  
  Effects frame (every 4 frames):
    IF random(0-1) > 0.97 → 
      Pick random neighbor (3x3)
      IF neighbor is EmptyCell → spawn new SlimeMold there
```

**Properties:**
```csharp
Mass: 100
Health: 400
Growth Rate: 3% per frame
Explosion Resistance: 1
Color: Green variations
```

**Code Location:** `Assets/Scripts/Elements/Solid/Immovable/SlimeMold.cs`

---

## 💧 Liquids

**Base CA Rule (Liquid):**
1. Try move **DOWN** (swap with EmptyCell or Gas)
2. If blocked, try **HORIZONTAL DISPERSION** (left/right up to `dispersionRate` cells)
3. Liquids flow smoothly and settle to lowest points

### Water
**Description:** Cooling liquid that extinguishes fire.

**Plain English:**
- Falls down through empty space
- Spreads horizontally (dispersion rate: 5)
- Flows around obstacles naturally
- Cools and extinguishes ignited elements
- Heavier than most liquids (sinks below oil)

**CA Rule:**
```
Every frame:
  IF cell below is empty/gas → move down
  ELSE → 
    Pick random direction (left/right)
    FOR i = 1 to dispersionRate (5):
      IF cell at [x + direction*i, y] is empty/gas → move there
      BREAK
  
  Effects frame:
    FOR each neighbor (3x3):
      IF neighbor.isIgnited → 
        neighbor.flammabilityResistance += coolingFactor (10)
        neighbor.CheckIfIgnited()
```

**Properties:**
```csharp
Mass: 20
Dispersion Rate: 5 (flows far)
Cooling Factor: 10
Health: 500
Explosion Resistance: 1 (easily destroyed)
Color: Blue variations
```

**Code Location:** `Assets/Scripts/Elements/Liquid/Water.cs`

---

### Oil
**Description:** Flammable liquid fuel.

**Plain English:**
- Flows like water but slower (dispersion rate: 3)
- Lighter than water (floats on top)
- Ignites easily
- Burns with sustained fire

**CA Rule:**
```
Every frame:
  Standard liquid movement (dispersion: 3)
  
  IF isIgnited → 
    Take fire damage
    Spread heat to neighbors
    Eventually burns away
```

**Properties:**
```csharp
Mass: 18 (lighter than water)
Dispersion Rate: 3
Flammability Resistance: 40 (very flammable)
Fire Damage: 2 per frame
Color: Dark brown/black
```

**Code Location:** `Assets/Scripts/Elements/Liquid/Oil.cs`

---

### Acid
**Description:** Corrosive liquid that damages most materials.

**Plain English:**
- Flows like water (dispersion rate: 3)
- Corrodes neighboring elements (deals health damage)
- Doesn't corrode itself or empty cells
- 10% chance per reaction frame to damage neighbors

**CA Rule:**
```
Every frame:
  Standard liquid movement (dispersion: 3)
  
  Reaction frame (every 4 frames):
    FOR each neighbor (3x3, excluding self):
      IF neighbor is not Acid and not EmptyCell:
        IF random(0-1) > 0.9 → 
          neighbor.health -= 170
          neighbor.CheckIfDead()
```

**Properties:**
```csharp
Mass: 22 (slightly heavier than water)
Dispersion Rate: 3
Corrosion Damage: 170 HP
Corrosion Chance: 10% per reaction frame
Color: Green
```

**Code Location:** `Assets/Scripts/Elements/Liquid/Acid.cs`

---

### Lava
**Description:** Extremely hot molten rock that ignites everything.

**Plain English:**
- Flows very slowly (dispersion rate: 2)
- Heaviest liquid
- Ignites all flammable neighbors
- Deals constant damage to most elements
- Very high temperature

**CA Rule:**
```
Every frame:
  Standard liquid movement (dispersion: 2, very slow)
  
  Effects frame:
    FOR each neighbor (3x3):
      IF neighbor exists:
        Apply heat (heatFactor: 50)
        
        IF neighbor is not Lava and not EmptyCell:
          neighbor.health -= 10
          neighbor.CheckIfDead()
```

**Properties:**
```csharp
Mass: 50 (heaviest liquid)
Dispersion Rate: 2 (very slow flow)
Temperature: 1000
Heat Factor: 50 (extreme heat)
Health Damage: 10 per frame
Color: Orange-red glow
```

**Code Location:** `Assets/Scripts/Elements/Liquid/Lava.cs`

---

### Blood
**Description:** Decorative liquid (no special interactions).

**Plain English:**
- Flows like water
- Purely visual (no gameplay effect)
- Created when PlayerMeat dies

**CA Rule:** Standard liquid movement only

**Properties:**
```csharp
Mass: 20
Dispersion Rate: 4
Color: Dark red
```

**Code Location:** `Assets/Scripts/Elements/Liquid/Blood.cs`

---

### Cement
**Description:** Liquid that solidifies over time.

**Plain English:**
- Initially flows like liquid
- After lifespan expires, transforms to Stone
- Used for construction puzzles

**CA Rule:**
```
Every frame:
  Standard liquid movement (dispersion: 2)
  
  Lifespan countdown (300 frames)
  
  IF lifespan <= 0 → 
    Die and become Stone
```

**Properties:**
```csharp
Mass: 25
Dispersion Rate: 2 (thick liquid)
Lifespan: 300 frames (~5 seconds)
Transforms To: Stone
Color: Gray
```

**Code Location:** `Assets/Scripts/Elements/Liquid/Cement.cs`

---

## 💨 Gases

**Base CA Rule (Gas):**
1. Try move **UP** (swap with EmptyCell)
2. If blocked, try **HORIZONTAL DISPERSION** (left/right up to `dispersionRate` cells)
3. Gases have **lifespan** - disappear after time

### Steam
**Description:** Water vapor that rises and dissipates.

**Plain English:**
- Rises upward
- Spreads horizontally if blocked
- Disappears after 400 frames
- Created when water is heated

**CA Rule:**
```
Every frame:
  IF cell above is empty → move up
  ELSE → 
    Pick random direction
    FOR i = 1 to dispersionRate (3):
      IF cell at [x + direction*i, y] is empty → move there
  
  Lifespan countdown (400 frames)
  IF lifespan <= 0 → Die (become EmptyCell)
```

**Properties:**
```csharp
Mass: 1 (lightest)
Dispersion Rate: 3
Lifespan: 400 frames (~7 seconds)
Color: Light gray/white
```

**Code Location:** `Assets/Scripts/Elements/Gas/Steam.cs`

---

### Smoke
**Description:** Byproduct of fire.

**Plain English:**
- Rises and disperses like steam
- Darker color than steam
- Shorter lifespan

**CA Rule:** Same as Steam

**Properties:**
```csharp
Mass: 1
Dispersion Rate: 3
Lifespan: 300 frames (~5 seconds)
Color: Dark gray
```

**Code Location:** `Assets/Scripts/Elements/Gas/Smoke.cs`

---

### Spark
**Description:** Ignition source that ignites flammable materials.

**Plain English:**
- Rises quickly
- Ignites any flammable element it touches
- Very short lifespan
- Created by friction or fire

**CA Rule:**
```
Every frame:
  Standard gas movement (rises)
  
  Effects frame:
    FOR each neighbor:
      IF neighbor is flammable → 
        Apply heat (heatFactor: 40)
  
  Lifespan: 150 frames
```

**Properties:**
```csharp
Mass: 1
Heat Factor: 40
Lifespan: 150 frames
Color: Orange-yellow
```

**Code Location:** `Assets/Scripts/Elements/Gas/Spark.cs`

---

### Explosion Spark
**Description:** High-temperature debris from explosions.

**Plain English:**
- Rises and spreads rapidly
- Very hot (ignites everything nearby)
- Short lifespan
- Created during explosions

**CA Rule:** Similar to Spark but with higher heat

**Properties:**
```csharp
Mass: 1
Heat Factor: 60 (extreme)
Temperature: 900
Lifespan: 100 frames
Color: Bright orange-white
```

**Code Location:** `Assets/Scripts/Elements/Gas/ExplosionSpark.cs`

---

### Flammable Gas
**Description:** Explosive gas cloud.

**Plain English:**
- Rises and spreads like normal gas
- Explodes when ignited
- Creates large blast radius

**CA Rule:**
```
Every frame:
  Standard gas movement
  
  IF isIgnited → 
    Create explosion (radius: 20)
    Die
```

**Properties:**
```csharp
Mass: 1
Explosion Radius: 20
Flammability Resistance: 30 (very flammable)
Lifespan: 600 frames
Color: Light green
```

**Code Location:** `Assets/Scripts/Elements/Gas/FlammableGas.cs`

---

## 🎯 Special Elements

### EmptyCell
**Description:** Singleton representing empty space.

**Plain English:**
- Not a "real" element
- Placeholder for empty cells
- All elements can move into EmptyCell spaces
- Never moves or interacts

**CA Rule:**
```
Every frame:
  Do nothing
```

**Properties:**
```csharp
Mass: 0
Health: Infinite (cannot die)
Color: Black (transparent)
```

**Code Location:** `Assets/Scripts/Elements/EmptyCell.cs`

---

### PlayerMeat
**Description:** Element used for player character body.

**Plain English:**
- Special element for player entity
- Has health and can be damaged
- Dies and spawns Blood
- Controlled by Player.cs script

**CA Rule:** Custom (not CA-based, controlled by Player entity)

**Properties:**
```csharp
Mass: 30
Health: 500
Color: Pink
```

**Code Location:** `Assets/Scripts/Elements/Player/PlayerMeat.cs`

---

## Interaction Matrix

This table shows how different element types interact:

| Element A ↓ / Element B → | Empty | Sand | Water | Lava | Acid | Stone | Wood | Gas |
|----------------------------|-------|------|-------|------|------|-------|------|-----|
| **Sand** | Move | - | Displace | Burn | Corrode | Block | Block | Displace |
| **Water** | Move | Block | - | Evaporate | Mix | Block | Block | Displace |
| **Lava** | Move | Block | Boil | - | Mix | Block | Ignite | Displace |
| **Acid** | Move | Corrode | Mix | Mix | - | Corrode | Corrode | Displace |
| **Wood** | - | Block | Block | Burn | Corrode | - | - | Block |
| **Gunpowder** | Move | - | Block | Ignite | Corrode | Block | Block | Displace |
| **Steam** | Move | Block | Block | Dissipate | Block | Block | Block | Mix |

**Legend:**
- **Move**: Element A moves into Element B's space
- **Displace**: Element A pushes Element B (mass-based)
- **Block**: Element A cannot move through B
- **Burn**: Element B catches fire
- **Corrode**: Element B takes acid damage
- **Mix**: Elements coexist (no displacement)
- **Boil**: Water → Steam
- **-**: No interaction

---

## Advanced: Timing Frames

The simulation uses **frame modulation** for performance:

```csharp
frameCount = (frameCount + 1) % 4;

// Different effects run on different frames:
bool IsReactionFrame() => frameCount == 3;  // Acid corrosion, heavy reactions
bool IsEffectsFrame() => frameCount == 1;   // Fire spread, heat transfer
```

**Why?**
- Not all effects need to run every frame
- Spreads CPU load across frames
- Maintains 60 FPS with thousands of elements

---

## Creating Custom Elements

**Step 1:** Choose base class
```csharp
// Falling particles → MovableSolid
// Terrain → ImmovableSolid
// Flowing → Liquid
// Rising → Gas
```

**Step 2:** Define properties
```csharp
public class MyElement : MovableSolid
{
    public MyElement(int x, int y) : base(x, y)
    {
        mass = 45;
        health = 600;
        flammabilityResistance = 70;
        // ...
    }
}
```

**Step 3:** Add custom behavior
```csharp
public override void Step(CellularMatrix matrix)
{
    base.Step(matrix);  // Inherit base movement
    
    // Add custom CA rules here
    if (IsEffectsFrame())
    {
        // Your interaction logic
    }
}
```

**Step 4:** Register in ElementType enum
```csharp
// In ElementType.cs
public enum ElementType
{
    // ...
    MYELEMENT,
}
```

---

**For more technical details:**
- Code structure: [ARCHITECTURE.md](ARCHITECTURE.md)
- System configuration: [SYSTEM_SETTINGS.md](SYSTEM_SETTINGS.md)
- Performance tuning: [PERFORMANCE_OPTIMIZATIONS.md](PERFORMANCE_OPTIMIZATIONS.md)
