using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public abstract class Solid : Element
    {
        protected Solid(int x, int y) : base(x, y)
        {
            frictionFactor = 0.8f;
            inertialResistance = 0.7f;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            if (owningBody != null) return;
            if (matrix.useChunks && !matrix.ShouldElementInChunkStep(this)) return;
            if (stepped) return;
            
            stepped = true;
            
            Vector3 formerLocation = new Vector3(GetMatrixX(), GetMatrixY(), 0);
            
            // Try to move
            TryMove(matrix);
            
            if (!DidNotMove(formerLocation))
            {
                matrix.ReportToChunkActive(this);
            }
        }
        
        protected abstract void TryMove(CellularMatrix matrix);
        
        protected bool DidNotMove(Vector3 formerLocation)
        {
            return formerLocation.x == GetMatrixX() && formerLocation.y == GetMatrixY();
        }
    }
}

