using UnityEngine;
using System.Collections.Generic;

namespace FallingSand.Util
{
    public class Chunk
    {
        public static int size = 32;
        
        private Vector2Int topLeft;
        private Vector2Int bottomRight;
        private bool shouldStep = true;
        private bool shouldStepNextFrame = false;
        private List<FallingSand.Entities.Boid> boids = new List<FallingSand.Entities.Boid>();
        
        public void SetTopLeft(Vector2Int pos) => topLeft = pos;
        public void SetBottomRight(Vector2Int pos) => bottomRight = pos;
        public Vector2Int GetTopLeft() => topLeft;
        public Vector2Int GetBottomRight() => bottomRight;
        
        public bool GetShouldStep() => shouldStep;
        public void SetShouldStepNextFrame(bool value) => shouldStepNextFrame = value;
        
        public void ShiftShouldStepAndReset()
        {
            shouldStep = shouldStepNextFrame;
            shouldStepNextFrame = false;
        }
        
        public void AddBoid(FallingSand.Entities.Boid boid)
        {
            if (!boids.Contains(boid))
                boids.Add(boid);
        }
        
        public void RemoveBoid(FallingSand.Entities.Boid boid)
        {
            boids.Remove(boid);
        }
        
        public void RemoveAllBoids()
        {
            boids.Clear();
        }
        
        public List<FallingSand.Entities.Boid> GetAllBoids() => boids;
    }
}

