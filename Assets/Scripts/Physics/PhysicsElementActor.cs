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
        private int minX;
        private int maxY;
        
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
            
            // Update element positions based on physics body
            Vector2 bodyPos = physicsBody.position;
            float rotation = physicsBody.rotation;
            
            // Could update element rendering based on body transform
        }
        
        public void Draw(Texture2D texture, Color32[] pixels, int screenWidth)
        {
            if (physicsBody == null) return;
            
            // Elements in physics bodies are drawn by the physics system
            // This is handled by the renderer checking owningBody
        }
        
        public void ElementDeath(Element element, Element newElement)
        {
            // Handle element death in physics body
            // Could trigger body destruction if too many elements are removed
        }
        
        public Rigidbody2D GetPhysicsBody() => physicsBody;
    }
}

