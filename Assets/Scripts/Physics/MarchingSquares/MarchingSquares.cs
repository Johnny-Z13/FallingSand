using UnityEngine;
using System.Collections.Generic;

namespace FallingSand.Physics.MarchingSquares
{
    public class MarchingSquares
    {
        // Marching squares algorithm to extract contours from a 2D grid
        public static List<Vector2> GetContour(bool[,] grid)
        {
            List<Vector2> contour = new List<Vector2>();
            int width = grid.GetLength(1);
            int height = grid.GetLength(0);
            
            // Find starting point
            Vector2Int start = FindStartPoint(grid);
            if (start.x == -1) return contour;
            
            // Trace contour
            Vector2Int current = start;
            Vector2Int direction = new Vector2Int(0, -1); // Start going up
            
            do
            {
                contour.Add(new Vector2(current.x, current.y));
                
                // Get marching squares case
                int caseValue = GetMarchingSquaresCase(grid, current.x, current.y);
                
                // Determine next direction based on case
                direction = GetNextDirection(caseValue, direction);
                
                // Move to next position
                current += direction;
                
                // Safety check to prevent infinite loops
                if (contour.Count > width * height)
                    break;
                    
            } while (current != start);
            
            return contour;
        }
        
        private static Vector2Int FindStartPoint(bool[,] grid)
        {
            int width = grid.GetLength(1);
            int height = grid.GetLength(0);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (grid[y, x])
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }
            
            return new Vector2Int(-1, -1);
        }
        
        private static int GetMarchingSquaresCase(bool[,] grid, int x, int y)
        {
            int width = grid.GetLength(1);
            int height = grid.GetLength(0);
            
            int caseValue = 0;
            
            // Check 4 corners (clockwise from top-left)
            if (x > 0 && y > 0 && grid[y - 1, x - 1]) caseValue |= 1;
            if (y > 0 && x < width - 1 && grid[y - 1, x]) caseValue |= 2;
            if (x < width - 1 && y < height - 1 && grid[y, x]) caseValue |= 4;
            if (x > 0 && y < height - 1 && grid[y, x - 1]) caseValue |= 8;
            
            return caseValue;
        }
        
        private static Vector2Int GetNextDirection(int caseValue, Vector2Int currentDirection)
        {
            // Simplified marching squares direction lookup
            // Based on the case value, determine where to go next
            switch (caseValue)
            {
                case 1: return new Vector2Int(0, -1);  // Up
                case 2: return new Vector2Int(1, 0);   // Right
                case 3: return new Vector2Int(1, 0);   // Right
                case 4: return new Vector2Int(0, 1);   // Down
                case 5: return new Vector2Int(0, -1);  // Up
                case 6: return new Vector2Int(0, 1);   // Down
                case 7: return new Vector2Int(1, 0);   // Right
                case 8: return new Vector2Int(-1, 0);  // Left
                case 9: return new Vector2Int(0, -1);  // Up
                case 10: return new Vector2Int(-1, 0); // Left
                case 11: return new Vector2Int(0, -1); // Up
                case 12: return new Vector2Int(-1, 0); // Left
                case 13: return new Vector2Int(-1, 0); // Left
                case 14: return new Vector2Int(0, 1);  // Down
                case 15: return currentDirection;       // Continue current direction
                default: return new Vector2Int(1, 0);  // Default right
            }
        }
    }
}

