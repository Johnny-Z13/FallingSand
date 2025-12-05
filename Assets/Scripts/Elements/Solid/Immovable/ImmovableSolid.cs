using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public abstract class ImmovableSolid : Solid
    {
        protected ImmovableSolid(int x, int y) : base(x, y)
        {
            mass = 1000;
            explosionResistance = 10;
        }
        
        protected override void TryMove(CellularMatrix matrix)
        {
            // Immovable solids don't move
        }
        
        public override void Step(CellularMatrix matrix)
        {
            if (owningBody != null) return;
            
            // Check for fire effects
            if (isIgnited && IsEffectsFrame())
            {
                TakeFireDamage(matrix);
                ApplyHeatToNeighbors(matrix);
            }
            
            CheckIfDead(matrix);
        }
        
        protected void TakeFireDamage(CellularMatrix matrix)
        {
            health -= fireDamage;
        }
        
        protected void ApplyHeatToNeighbors(CellularMatrix matrix)
        {
            for (int x = GetMatrixX() - 1; x <= GetMatrixX() + 1; x++)
            {
                for (int y = GetMatrixY() - 1; y <= GetMatrixY() + 1; y++)
                {
                    if (x == GetMatrixX() && y == GetMatrixY()) continue;
                    
                    Element neighbor = matrix.Get(x, y);
                    if (neighbor != null)
                    {
                        neighbor.ReceiveHeat(matrix, heatFactor);
                    }
                }
            }
        }
        
        protected override bool ActOnNeighboringElement(Element neighbor, int modifiedMatrixX, int modifiedMatrixY, 
            CellularMatrix matrix, bool isFinal, bool isFirst, Vector3 lastValidLocation, int depth)
        {
            return false;
        }
    }
}

