using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public class Gas : Element
    {
        protected int dispersionRate = 3;
        
        public Gas(int x, int y) : base(x, y)
        {
            mass = 1;
            lifeSpan = 600;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            if (matrix.useChunks && !matrix.ShouldElementInChunkStep(this)) return;
            if (stepped) return;
            
            stepped = true;
            
            int myX = GetMatrixX();
            int myY = GetMatrixY();
            Vector3 formerLocation = new Vector3(myX, myY, 0);
            
            // Gases rise
            Element above = matrix.Get(myX, myY + 1);
            if (above != null && above.elementType == ElementType.EMPTYCELL)
            {
                SwapPositions(matrix, above, myX, myY + 1);
            }
            else
            {
                // Disperse horizontally
                int direction = Random.value > 0.5f ? 1 : -1;
                for (int i = 1; i <= dispersionRate; i++)
                {
                    Element side = matrix.Get(myX + (direction * i), myY);
                    if (side != null && side.elementType == ElementType.EMPTYCELL)
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
            
            // Check lifespan
            if (lifeSpan.HasValue)
            {
                lifeSpan--;
                if (lifeSpan <= 0)
                {
                    Die(matrix);
                }
            }
        }
        
        protected bool DidNotMove(Vector3 formerLocation)
        {
            return formerLocation.x == GetMatrixX() && formerLocation.y == GetMatrixY();
        }
        
        protected override bool ActOnNeighboringElement(Element neighbor, int modifiedMatrixX, int modifiedMatrixY, 
            CellularMatrix matrix, bool isFinal, bool isFirst, Vector3 lastValidLocation, int depth)
        {
            return false;
        }
        
        public override ElementType GetEnumType()
        {
            return ElementType.GAS;
        }
    }
}

