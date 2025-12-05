using UnityEngine;
using FallingSand.Core;
using FallingSand.Elements;

namespace FallingSand.Entities
{
    public class Player
    {
        private int matrixX;
        private int matrixY;
        private int playerWidth = 3;
        private int playerHeight = 5;
        private Vector3 velocity = Vector3.zero;
        private bool isAlive = true;
        
        public Player(int startX, int startY)
        {
            this.matrixX = startX;
            this.matrixY = startY;
        }
        
        public void Step(CellularMatrix matrix)
        {
            if (!isAlive) return;
            
            // Apply simple physics
            velocity += CellularAutomaton.Instance.config.gravity * Time.deltaTime;
            
            // Try to move based on velocity
            int targetY = matrixY + Mathf.RoundToInt(velocity.y * Time.deltaTime);
            
            // Check if can move
            bool canMove = true;
            for (int x = 0; x < playerWidth; x++)
            {
                Element below = matrix.Get(matrixX + x, targetY - 1);
                if (below != null && !(below is EmptyCell) && !(below is Gas))
                {
                    canMove = false;
                    velocity.y = 0;
                    break;
                }
            }
            
            if (canMove && targetY > 0)
            {
                matrixY = targetY;
            }
            
            // Handle input
            if (UnityEngine.Input.GetKey(KeyCode.LeftArrow) && matrixX > 0)
            {
                matrixX--;
            }
            if (UnityEngine.Input.GetKey(KeyCode.RightArrow) && matrixX < matrix.innerArraySize - playerWidth)
            {
                matrixX++;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && velocity.y <= 0)
            {
                velocity.y = 50f;
            }
            
            // Spawn player meat at position
            for (int x = 0; x < playerWidth; x++)
            {
                for (int y = 0; y < playerHeight; y++)
                {
                    Element element = matrix.Get(matrixX + x, matrixY + y);
                    if (element is EmptyCell)
                    {
                        matrix.SpawnElementByMatrix(matrixX + x, matrixY + y, ElementType.PLAYERMEAT);
                    }
                }
            }
        }
        
        public bool IsAlive() => isAlive;
        public void Kill() => isAlive = false;
    }
}

