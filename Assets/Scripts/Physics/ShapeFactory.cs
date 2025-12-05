using UnityEngine;
using System.Collections.Generic;
using FallingSand.Elements;
using FallingSand.Physics.MarchingSquares;
using FallingSand.Physics.DouglasPeucker;

namespace FallingSand.Physics
{
    public static class ShapeFactory
    {
        public static Rigidbody2D CreatePolygonFromElementArray(int minX, int minY, 
            List<List<Element>> elementArray, RigidbodyType2D bodyType)
        {
            // Convert element array to boolean grid
            bool[,] grid = new bool[elementArray.Count, elementArray[0].Count];
            for (int y = 0; y < elementArray.Count; y++)
            {
                for (int x = 0; x < elementArray[y].Count; x++)
                {
                    grid[y, x] = !(elementArray[y][x] is EmptyCell);
                }
            }
            
            // Get contour using marching squares
            List<Vector2> contour = MarchingSquares.MarchingSquares.GetContour(grid);
            
            if (contour.Count < 3)
                return null;
            
            // Simplify using Douglas-Peucker
            List<Vector2> simplified = DouglasPeucker.DouglasPeucker.Simplify(contour, 2.0f);
            
            if (simplified.Count < 3)
                return null;
            
            // Create GameObject for physics body
            GameObject go = new GameObject("PhysicsElementActor");
            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = bodyType;
            rb.gravityScale = 1f;
            
            // Create polygon collider
            // Note: Unity's PolygonCollider2D has some limitations
            // May need multiple colliders for complex shapes
            PolygonCollider2D collider = go.AddComponent<PolygonCollider2D>();
            
            // Convert simplified points to Vector2 array
            Vector2[] points = new Vector2[Mathf.Min(simplified.Count, 8)]; // Unity limit
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector2(
                    simplified[i].x + minX,
                    simplified[i].y + minY
                );
            }
            
            collider.points = points;
            
            // Position the rigidbody
            Vector2 center = Vector2.zero;
            foreach (var point in points)
            {
                center += point;
            }
            center /= points.Length;
            rb.position = center;
            
            return rb;
        }
        
        public static Rigidbody2D CreateBox(Vector3 center, Vector3 size, RigidbodyType2D bodyType)
        {
            GameObject go = new GameObject("PhysicsBox");
            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = bodyType;
            rb.position = center;
            rb.gravityScale = 1f;
            
            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
            collider.size = size;
            
            return rb;
        }
    }
}

