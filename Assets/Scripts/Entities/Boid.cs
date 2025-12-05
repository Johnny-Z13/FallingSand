using UnityEngine;
using FallingSand.Core;
using FallingSand.Elements;
using FallingSand.Util;
using System.Collections.Generic;

namespace FallingSand.Entities
{
    public class Boid
    {
        public static int neighborDistance = 20;
        public static int maxNeighbors = 7;
        
        private int matrixX;
        private int matrixY;
        private Vector3 velocity;
        private float maxSpeed = 2f;
        private float maxForce = 0.5f;
        
        private Chunk currentChunk;
        
        public Boid(int x, int y, Vector3 velocity)
        {
            this.matrixX = x;
            this.matrixY = y;
            this.velocity = velocity;
        }
        
        public void Step(CellularMatrix matrix)
        {
            List<Boid> neighbors = GetNeighbors(matrix);
            
            // Apply flocking behaviors
            Vector3 separation = Separate(neighbors);
            Vector3 alignment = Align(neighbors);
            Vector3 cohesion = Cohere(neighbors);
            
            // Weight the behaviors
            separation *= 1.5f;
            alignment *= 1.0f;
            cohesion *= 1.0f;
            
            // Apply forces
            velocity += separation + alignment + cohesion;
            
            // Limit speed
            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }
            
            // Update position
            matrixX += Mathf.RoundToInt(velocity.x);
            matrixY += Mathf.RoundToInt(velocity.y);
            
            // Keep within bounds
            if (matrixX < 0) matrixX = 0;
            if (matrixX >= matrix.innerArraySize) matrixX = matrix.innerArraySize - 1;
            if (matrixY < 0) matrixY = 0;
            if (matrixY >= matrix.outerArraySize) matrixY = matrix.outerArraySize - 1;
            
            // Update chunk
            currentChunk = matrix.GetChunkForCoordinates(matrixX, matrixY);
        }
        
        private List<Boid> GetNeighbors(CellularMatrix matrix)
        {
            // Get nearby boids from chunk system
            return currentChunk?.GetAllBoids() ?? new List<Boid>();
        }
        
        private Vector3 Separate(List<Boid> neighbors)
        {
            Vector3 steer = Vector3.zero;
            int count = 0;
            
            foreach (Boid other in neighbors)
            {
                float d = Vector3.Distance(new Vector3(matrixX, matrixY, 0), new Vector3(other.matrixX, other.matrixY, 0));
                if (d > 0 && d < neighborDistance / 2)
                {
                    Vector3 diff = new Vector3(matrixX - other.matrixX, matrixY - other.matrixY, 0);
                    diff.Normalize();
                    diff /= d;
                    steer += diff;
                    count++;
                }
            }
            
            if (count > 0)
            {
                steer /= count;
            }
            
            if (steer.magnitude > 0)
            {
                steer.Normalize();
                steer *= maxSpeed;
                steer -= velocity;
                if (steer.magnitude > maxForce)
                {
                    steer = steer.normalized * maxForce;
                }
            }
            
            return steer;
        }
        
        private Vector3 Align(List<Boid> neighbors)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            
            foreach (Boid other in neighbors)
            {
                sum += other.velocity;
                count++;
            }
            
            if (count > 0)
            {
                sum /= count;
                sum.Normalize();
                sum *= maxSpeed;
                Vector3 steer = sum - velocity;
                if (steer.magnitude > maxForce)
                {
                    steer = steer.normalized * maxForce;
                }
                return steer;
            }
            
            return Vector3.zero;
        }
        
        private Vector3 Cohere(List<Boid> neighbors)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            
            foreach (Boid other in neighbors)
            {
                sum += new Vector3(other.matrixX, other.matrixY, 0);
                count++;
            }
            
            if (count > 0)
            {
                sum /= count;
                return Seek(sum);
            }
            
            return Vector3.zero;
        }
        
        private Vector3 Seek(Vector3 target)
        {
            Vector3 desired = target - new Vector3(matrixX, matrixY, 0);
            desired.Normalize();
            desired *= maxSpeed;
            
            Vector3 steer = desired - velocity;
            if (steer.magnitude > maxForce)
            {
                steer = steer.normalized * maxForce;
            }
            
            return steer;
        }
        
        public int GetMatrixX() => matrixX;
        public int GetMatrixY() => matrixY;
    }
}

