using UnityEngine;
using System.Collections.Generic;

namespace FallingSand.Physics.DouglasPeucker
{
    public class DouglasPeucker
    {
        // Douglas-Peucker line simplification algorithm
        public static List<Vector2> Simplify(List<Vector2> points, float epsilon)
        {
            if (points.Count < 3)
                return new List<Vector2>(points);
            
            // Find the point with the maximum distance
            float dmax = 0;
            int index = 0;
            int end = points.Count - 1;
            
            for (int i = 1; i < end; i++)
            {
                float d = PerpendicularDistance(points[i], points[0], points[end]);
                if (d > dmax)
                {
                    index = i;
                    dmax = d;
                }
            }
            
            List<Vector2> result = new List<Vector2>();
            
            // If max distance is greater than epsilon, recursively simplify
            if (dmax > epsilon)
            {
                // Recursive call
                List<Vector2> recResults1 = Simplify(points.GetRange(0, index + 1), epsilon);
                List<Vector2> recResults2 = Simplify(points.GetRange(index, end - index + 1), epsilon);
                
                // Build result list
                result.AddRange(recResults1.GetRange(0, recResults1.Count - 1));
                result.AddRange(recResults2);
            }
            else
            {
                // Just keep start and end points
                result.Add(points[0]);
                result.Add(points[end]);
            }
            
            return result;
        }
        
        private static float PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            float dx = lineEnd.x - lineStart.x;
            float dy = lineEnd.y - lineStart.y;
            
            // Normalize
            float mag = Mathf.Sqrt(dx * dx + dy * dy);
            if (mag > 0.0f)
            {
                dx /= mag;
                dy /= mag;
            }
            
            float pvx = point.x - lineStart.x;
            float pvy = point.y - lineStart.y;
            
            // Get dot product (project point onto line)
            float pvdot = dx * pvx + dy * pvy;
            
            // Scale line direction vector
            float dsx = pvdot * dx;
            float dsy = pvdot * dy;
            
            // Subtract this from pv
            float ax = pvx - dsx;
            float ay = pvy - dsy;
            
            return Mathf.Sqrt(ax * ax + ay * ay);
        }
    }
}

