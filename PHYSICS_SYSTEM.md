# Physics System Reference

> **Technical documentation on physics integration, Marching Squares algorithm, Douglas-Peucker simplification, and Rigidbody2D generation from particle groups.**

---

## 📖 Table of Contents

1. [Physics Overview](#physics-overview)
2. [Marching Squares Algorithm](#marching-squares-algorithm)
3. [Douglas-Peucker Algorithm](#douglas-peucker-algorithm)
4. [Shape Factory](#shape-factory)
5. [PhysicsElementActor](#physicselementactor)
6. [Unity Physics2D Integration](#unity-physics2d-integration)
7. [Workflow Examples](#workflow-examples)
8. [Performance Considerations](#performance-considerations)

---

## Physics Overview

### Why Physics Integration?

**Problem:** Cellular automata simulate individual particles, but real-world objects are **rigid bodies**.

**Solution:** Convert groups of static elements into Unity **Rigidbody2D** objects.

**Benefits:**
- Realistic collision physics
- Falling structures (e.g., houses made of brick)
- Interaction with Unity's physics system
- Player can collide with element groups

### Architecture

```
Element Groups (CA)
     ↓
Marching Squares (contour extraction)
     ↓
Douglas-Peucker (simplification)
     ↓
ShapeFactory (collider creation)
     ↓
PhysicsElementActor (Rigidbody2D manager)
     ↓
Unity Physics2D (collision detection)
```

---

## Marching Squares Algorithm

### What It Does

**Marching Squares** extracts contours (outlines) from a 2D grid of elements.

**Input:** 2D array of elements (solid vs empty)
**Output:** List of connected edge points forming the boundary

### Algorithm Overview

**Core Concept:** For each 2×2 cell in the grid, determine which edges are part of the boundary.

### The 16 Cases

Each 2×2 cell has **4 corners** (solid or empty), giving **2^4 = 16 possible configurations**:

```
Corner Numbering:
  0───1
  │   │
  3───2

Solid = 1, Empty = 0
```

**Case Examples:**

```
Case 0: All empty          Case 15: All solid
  0───0                      1───1
  │   │                      │   │
  0───0                      1───1
  (No edge)                  (No edge)

Case 1: Bottom-left solid  Case 7: Bottom row solid
  0───0                      0───0
  │ ╱ │                      ├───┤
  1───0                      1───1
  (Bottom-left edge)         (Top horizontal edge)
```

**Edge Directions:**
- **Vertical edges**: Left, Right
- **Horizontal edges**: Top, Bottom
- **Diagonal edges** (ambiguous cases): Need disambiguation

### Pseudocode

```
FOR each 2×2 cell in grid:
    bitMask = 0
    IF topLeft solid:     bitMask |= 1
    IF topRight solid:    bitMask |= 2
    IF bottomRight solid: bitMask |= 4
    IF bottomLeft solid:  bitMask |= 8
    
    SWITCH bitMask:
        CASE 1, 2, 4, 8, ...:
            Add appropriate edges to contour list
```

### C# Implementation Sketch

```csharp
public class MarchingSquares
{
    public static List<Vector2> ExtractContour(Element[,] grid)
    {
        List<Vector2> contour = new List<Vector2>();
        
        for (int y = 0; y < grid.GetLength(0) - 1; y++)
        {
            for (int x = 0; x < grid.GetLength(1) - 1; x++)
            {
                int bitMask = CalculateBitMask(grid, x, y);
                AddEdgesForCase(bitMask, x, y, contour);
            }
        }
        
        return contour;
    }
    
    private static int CalculateBitMask(Element[,] grid, int x, int y)
    {
        int mask = 0;
        if (IsSolid(grid[y, x])) mask |= 1;        // Top-left
        if (IsSolid(grid[y, x+1])) mask |= 2;      // Top-right
        if (IsSolid(grid[y+1, x+1])) mask |= 4;    // Bottom-right
        if (IsSolid(grid[y+1, x])) mask |= 8;      // Bottom-left
        return mask;
    }
    
    private static void AddEdgesForCase(int bitMask, int x, int y, List<Vector2> contour)
    {
        switch (bitMask)
        {
            case 1: // Top-left corner
                contour.Add(new Vector2(x + 0.5f, y));
                contour.Add(new Vector2(x, y + 0.5f));
                break;
            
            case 2: // Top-right corner
                contour.Add(new Vector2(x + 1, y + 0.5f));
                contour.Add(new Vector2(x + 0.5f, y));
                break;
            
            // ... 14 more cases ...
        }
    }
    
    private static bool IsSolid(Element element)
    {
        return element != null && element is Solid;
    }
}
```

**Output:** List of connected points forming the outline of solid regions.

---

## Douglas-Peucker Algorithm

### What It Does

**Douglas-Peucker** simplifies a polyline by removing unnecessary points while preserving shape.

**Input:** List of points (e.g., from Marching Squares)
**Output:** Simplified list with fewer points

**Why Needed:** Unity's `PolygonCollider2D` has an **8-vertex maximum**. Complex contours need simplification.

### Algorithm Overview

**Recursive Divide-and-Conquer:**

1. **Find furthest point** from line connecting start and end
2. **If distance > threshold**: Split into two segments, recurse
3. **If distance < threshold**: Remove all intermediate points

### Visual Example

```
Original (10 points):
  A─B─C─D─E─F─G─H─I─J
  
Simplified (4 points):
  A───────E───────I─J
  
Points B,C,D,F,G,H removed (distance < threshold)
Points A,E,I,J kept (critical for shape)
```

### Pseudocode

```
FUNCTION DouglasPeucker(points, epsilon):
    IF points.length < 3:
        RETURN points
    
    // Find furthest point
    maxDistance = 0
    maxIndex = 0
    
    FOR i = 1 to points.length - 2:
        distance = PerpendicularDistance(points[i], Line(points[0], points[end]))
        IF distance > maxDistance:
            maxDistance = distance
            maxIndex = i
    
    // Decide whether to split
    IF maxDistance > epsilon:
        // Recurse on both segments
        left = DouglasPeucker(points[0..maxIndex], epsilon)
        right = DouglasPeucker(points[maxIndex..end], epsilon)
        RETURN left + right
    ELSE:
        // Remove all intermediate points
        RETURN [points[0], points[end]]
```

### C# Implementation Sketch

```csharp
public class DouglasPeucker
{
    public static List<Vector2> Simplify(List<Vector2> points, float epsilon)
    {
        if (points.Count < 3)
            return points;
        
        // Find furthest point
        float maxDistance = 0f;
        int maxIndex = 0;
        
        Vector2 start = points[0];
        Vector2 end = points[points.Count - 1];
        
        for (int i = 1; i < points.Count - 1; i++)
        {
            float distance = PerpendicularDistance(points[i], start, end);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
            }
        }
        
        // Decide whether to simplify
        if (maxDistance > epsilon)
        {
            // Keep splitting
            List<Vector2> left = Simplify(points.GetRange(0, maxIndex + 1), epsilon);
            List<Vector2> right = Simplify(points.GetRange(maxIndex, points.Count - maxIndex), epsilon);
            
            // Merge (remove duplicate at split point)
            left.RemoveAt(left.Count - 1);
            left.AddRange(right);
            return left;
        }
        else
        {
            // Simplify to line
            return new List<Vector2> { start, end };
        }
    }
    
    private static float PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 line = lineEnd - lineStart;
        float lineLength = line.magnitude;
        
        if (lineLength == 0)
            return Vector2.Distance(point, lineStart);
        
        float t = Mathf.Clamp01(Vector2.Dot(point - lineStart, line) / (lineLength * lineLength));
        Vector2 projection = lineStart + t * line;
        
        return Vector2.Distance(point, projection);
    }
}
```

**Key Parameter:** `epsilon` (threshold distance)
- **Small epsilon (0.1)**: More points, higher fidelity, slower
- **Large epsilon (2.0)**: Fewer points, blockier, faster

---

## Shape Factory

### What It Does

**ShapeFactory** converts simplified point lists into Unity colliders.

**Input:** List of simplified points
**Output:** `PolygonCollider2D` or `EdgeCollider2D`

### Implementation

```csharp
using UnityEngine;
using System.Collections.Generic;

namespace FallingSand.Physics
{
    public static class ShapeFactory
    {
        public static PolygonCollider2D CreatePolygonCollider(GameObject obj, List<Vector2> points)
        {
            PolygonCollider2D collider = obj.AddComponent<PolygonCollider2D>();
            
            // Unity's PolygonCollider2D needs local coordinates
            Vector2[] localPoints = new Vector2[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                localPoints[i] = points[i] - (Vector2)obj.transform.position;
            }
            
            collider.points = localPoints;
            return collider;
        }
        
        public static EdgeCollider2D CreateEdgeCollider(GameObject obj, List<Vector2> points)
        {
            EdgeCollider2D collider = obj.AddComponent<EdgeCollider2D>();
            
            Vector2[] localPoints = new Vector2[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                localPoints[i] = points[i] - (Vector2)obj.transform.position;
            }
            
            collider.points = localPoints;
            return collider;
        }
        
        public static Rigidbody2D CreateRigidbody(GameObject obj, bool isStatic = true)
        {
            Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
            
            if (isStatic)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1f;
            }
            
            return rb;
        }
    }
}
```

### Usage Example

```csharp
// 1. Extract contour
List<Vector2> contour = MarchingSquares.ExtractContour(elementGrid);

// 2. Simplify
List<Vector2> simplified = DouglasPeucker.Simplify(contour, 1.0f);

// 3. Create GameObject
GameObject physicsObject = new GameObject("PhysicsElementGroup");
physicsObject.transform.position = new Vector3(centerX, centerY, 0);

// 4. Create Rigidbody2D
Rigidbody2D rb = ShapeFactory.CreateRigidbody(physicsObject, isStatic: false);

// 5. Create PolygonCollider2D
if (simplified.Count <= 8)
{
    PolygonCollider2D collider = ShapeFactory.CreatePolygonCollider(physicsObject, simplified);
}
else
{
    // Fallback: Multiple colliders or EdgeCollider2D
    EdgeCollider2D collider = ShapeFactory.CreateEdgeCollider(physicsObject, simplified);
}
```

---

## PhysicsElementActor

### What It Does

**PhysicsElementActor** manages the relationship between element data and physics bodies.

**Location:** `Assets/Scripts/Physics/PhysicsElementActor.cs`

### Responsibilities

1. **Track elements** belonging to this physics body
2. **Synchronize positions** (physics body ↔ elements)
3. **Handle element death** (update body when elements are destroyed)
4. **Render elements** in physics body

### Implementation

```csharp
using UnityEngine;
using System.Collections.Generic;
using FallingSand.Core;
using FallingSand.Elements;

namespace FallingSand.Physics
{
    public class PhysicsElementActor
    {
        private Rigidbody2D physicsBody;
        private List<List<Element>> elementArray;
        private int minX, maxY;
        
        public PhysicsElementActor(Rigidbody2D body, List<List<Element>> elements, int minX, int maxY)
        {
            this.physicsBody = body;
            this.elementArray = elements;
            this.minX = minX;
            this.maxY = maxY;
            
            // Set all elements to reference this actor
            foreach (var row in elementArray)
            {
                foreach (var element in row)
                {
                    if (element != null)
                    {
                        element.owningBody = this;
                    }
                }
            }
        }
        
        public void Step(CellularMatrix matrix)
        {
            if (physicsBody == null) return;
            
            // Elements in physics body don't step individually
            // They move with the Rigidbody2D
        }
        
        public void ElementDeath(Element element, Element newElement)
        {
            // Handle element destruction
            // If too many elements die, destroy the body
            
            int remainingElements = CountLivingElements();
            if (remainingElements < threshold)
            {
                DestroyBody();
            }
        }
        
        private int CountLivingElements()
        {
            int count = 0;
            foreach (var row in elementArray)
            {
                foreach (var element in row)
                {
                    if (element != null && !element.isDead)
                        count++;
                }
            }
            return count;
        }
        
        private void DestroyBody()
        {
            if (physicsBody != null)
            {
                Object.Destroy(physicsBody.gameObject);
            }
        }
        
        public Rigidbody2D GetPhysicsBody() => physicsBody;
    }
}
```

### Element Integration

**In Element.cs:**
```csharp
public class Element
{
    public PhysicsElementActor owningBody = null;
    
    public override void Step(CellularMatrix matrix)
    {
        // Skip stepping if part of physics body
        if (owningBody != null) return;
        
        // Normal CA logic
        TryMove(matrix);
    }
    
    public void Die(CellularMatrix matrix)
    {
        // Notify owning body
        if (owningBody != null)
        {
            owningBody.ElementDeath(this, newElement);
        }
        
        // Normal death logic
        // ...
    }
}
```

---

## Unity Physics2D Integration

### Physics Settings

**Location:** `Edit → Project Settings → Physics 2D`

**Key Settings:**
- **Gravity**: (0, -9.81) - Matches GameConfig gravity direction
- **Layers**: Configure collision layers for element groups
- **Solver Iterations**: Higher = more stable, slower

### Collision Layers

**Recommended Setup:**
```
Layer 8: ElementPhysics
Layer 9: Player
Layer 10: Projectiles

Collision Matrix:
  ElementPhysics ↔ Player: ✓
  ElementPhysics ↔ Projectiles: ✓
  ElementPhysics ↔ ElementPhysics: ✓
```

### Material Settings

**PhysicsMaterial2D for elements:**
```
Friction: 0.4
Bounciness: 0.1
```

**Assign to colliders:**
```csharp
PolygonCollider2D collider = ShapeFactory.CreatePolygonCollider(obj, points);
collider.sharedMaterial = elementPhysicsMaterial;
```

---

## Workflow Examples

### Example 1: Convert Static Stone Wall to Physics Body

**Scenario:** Player builds a stone wall, then removes bottom blocks. Wall should fall.

**Step 1: Detect disconnected groups**
```csharp
// Flood-fill algorithm to find connected element groups
List<List<Element>> groups = FindConnectedGroups(matrix);
```

**Step 2: For each group, extract contour**
```csharp
foreach (var group in groups)
{
    // Convert group to 2D grid
    Element[,] grid = ConvertToGrid(group);
    
    // Extract contour
    List<Vector2> contour = MarchingSquares.ExtractContour(grid);
}
```

**Step 3: Simplify contour**
```csharp
List<Vector2> simplified = DouglasPeucker.Simplify(contour, epsilon: 1.5f);
```

**Step 4: Create physics body**
```csharp
GameObject obj = new GameObject("StoneWallPhysics");
Rigidbody2D rb = ShapeFactory.CreateRigidbody(obj, isStatic: false);
PolygonCollider2D collider = ShapeFactory.CreatePolygonCollider(obj, simplified);
```

**Step 5: Create PhysicsElementActor**
```csharp
PhysicsElementActor actor = new PhysicsElementActor(rb, group, minX, maxY);
matrix.physicsElementActors.Add(actor);
```

**Step 6: Let Unity Physics take over**
- Rigidbody2D falls due to gravity
- Collides with ground and player
- Elements visual position synced with body

---

### Example 2: Destroy Physics Body by Acid

**Scenario:** Acid corrodes elements in a physics body.

**Workflow:**
1. Acid damages elements (normal CA logic)
2. Element health → 0, element dies
3. `Element.Die()` calls `owningBody.ElementDeath()`
4. `PhysicsElementActor` counts remaining elements
5. If < threshold, destroy entire body

```csharp
public void ElementDeath(Element element, Element newElement)
{
    int remaining = CountLivingElements();
    
    if (remaining < totalElements * 0.3f)  // 70% destroyed
    {
        // Body integrity compromised
        DestroyBody();
        
        // Convert remaining elements back to CA
        foreach (var elem in elementArray)
        {
            elem.owningBody = null;  // Resume CA stepping
        }
    }
}
```

---

## Performance Considerations

### Computational Cost

**Marching Squares:**
- **Complexity:** O(width × height)
- **Cost:** Low (simple bitmasking)
- **Typical time:** ~1-2ms for 100×100 grid

**Douglas-Peucker:**
- **Complexity:** O(n log n) average, O(n²) worst case
- **Cost:** Medium (recursive)
- **Typical time:** ~1-5ms for 1000 points → 8 points

**Unity Physics:**
- **Complexity:** O(n²) for n colliders (broad phase optimization)
- **Cost:** High if many physics bodies
- **Recommendation:** Limit to <50 active physics bodies

### Optimization Strategies

**1. Batch Conversion**
- Don't convert every frame
- Trigger conversion only when elements are destroyed/added
- Example: After explosion, check for disconnected groups

**2. Simplify Aggressively**
- Use higher epsilon (1.5-2.0) for distant objects
- Use lower epsilon (0.5-1.0) for player-visible objects

**3. Caching**
- Cache contours if element group doesn't change
- Invalidate cache on element death/spawn

**4. LOD (Level of Detail)**
- Far objects: Simple box colliders
- Near objects: Detailed PolygonCollider2D

**5. Static Bodies When Possible**
- Use `RigidbodyType2D.Static` for immovable structures
- Only convert to `Dynamic` when needed (e.g., supports destroyed)

---

### Memory Usage

**Per PhysicsElementActor:**
```
Rigidbody2D: ~200 bytes (Unity internal)
PolygonCollider2D: ~100 bytes + (points × 8 bytes)
ElementArray: elements × ~200 bytes

Example (20×20 stone wall):
  400 elements × 200 bytes = 80 KB
  Collider (8 points) = 164 bytes
  Total: ~80 KB
```

**Recommendation:** Limit total elements in physics bodies to <10,000.

---

### Unity Physics2D Settings for Performance

**Physics2D Settings (Project Settings):**
```
Auto Simulation: ✓ (enabled)
Simulation Mode: Update
Solver Iterations: 6 (default)
Velocity Iterations: 8 (default)
Max Linear Correction: 0.2
Max Rotation Correction: 8.0
Baumgarte Scale: 0.2
Baumgarte Time of Impact Scale: 0.75
Time to Sleep: 0.5
Linear Sleep Tolerance: 0.01
Angular Sleep Tolerance: 2.0
```

**For better performance:**
- Lower `Solver Iterations` to 4
- Lower `Velocity Iterations` to 6
- Increase `Time to Sleep` to 1.0

---

## Related Documentation

- **Architecture overview:** [ARCHITECTURE.md](ARCHITECTURE.md)
- **Element behaviors:** [ELEMENTS_REFERENCE.md](ELEMENTS_REFERENCE.md)
- **System settings:** [SYSTEM_SETTINGS.md](SYSTEM_SETTINGS.md)
- **Performance tuning:** [PERFORMANCE_OPTIMIZATIONS.md](PERFORMANCE_OPTIMIZATIONS.md)

---

**Summary:** The physics system bridges cellular automata (particle simulation) with Unity's Rigidbody2D (rigid body simulation). Marching Squares extracts contours, Douglas-Peucker simplifies them, and ShapeFactory creates colliders. PhysicsElementActor manages the element-body relationship. This enables realistic physics interactions while maintaining CA flexibility.
