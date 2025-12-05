using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public abstract class Liquid : Element
    {
        protected int dispersionRate = 4;
        
        protected Liquid(int x, int y) : base(x, y)
        {
            mass = 20;
            frictionFactor = 0.95f;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            if (owningBody != null) return;
            if (matrix.useChunks && !matrix.ShouldElementInChunkStep(this)) return;
            if (stepped) return;
            
            stepped = true;
            
            int myX = GetMatrixX();
            int myY = GetMatrixY();
            Vector3 formerLocation = new Vector3(myX, myY, 0);
            
            // Try to move down
            Element below = matrix.Get(myX, myY - 1);
            if (below != null && below.elementType == ElementType.EMPTYCELL)
            {
                SwapPositions(matrix, below, myX, myY - 1);
            }
            else if (below != null && below is Gas)
            {
                SwapPositions(matrix, below, myX, myY - 1);
            }
            else
            {
                // Try to flow horizontally
                int direction = Random.value > 0.5f ? 1 : -1;
                for (int i = 1; i <= dispersionRate; i++)
                {
                    Element side = matrix.Get(myX + (direction * i), myY);
                    if (side != null && (side.elementType == ElementType.EMPTYCELL || side is Gas))
                    {
                        SwapPositions(matrix, side, myX + (direction * i), myY);
                        break;
                    }
                }
            }
            
            if (!DidNotMove(formerLocation))
            {
                matrix.ReportToChunkActive(this);
            }
            
            // Custom liquid behaviors
            CustomLiquidBehavior(matrix);
        }
        
        protected virtual void CustomLiquidBehavior(CellularMatrix matrix) { }
        
        protected bool DidNotMove(Vector3 formerLocation)
        {
            return formerLocation.x == GetMatrixX() && formerLocation.y == GetMatrixY();
        }
        
        protected override bool ActOnNeighboringElement(Element neighbor, int modifiedMatrixX, int modifiedMatrixY, 
            CellularMatrix matrix, bool isFinal, bool isFirst, Vector3 lastValidLocation, int depth)
        {
            return false;
        }
    }
}

