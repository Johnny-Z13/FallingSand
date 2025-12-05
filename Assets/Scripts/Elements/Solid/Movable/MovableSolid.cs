using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public abstract class MovableSolid : Solid
    {
        protected MovableSolid(int x, int y) : base(x, y)
        {
            mass = 50;
        }
        
        protected override void TryMove(CellularMatrix matrix)
        {
            int myX = GetMatrixX();
            int myY = GetMatrixY();
            
            // Check directly below
            Element below = matrix.Get(myX, myY - 1);
            if (below != null)
            {
                if (below.elementType == ElementType.EMPTYCELL)
                {
                    SwapPositions(matrix, below, myX, myY - 1);
                    return;
                }
                
                // Try to displace lighter elements below
                if (below.mass < this.mass && (below is Liquid || below is Gas))
                {
                    SwapPositions(matrix, below, myX, myY - 1);
                    return;
                }
            }
            
            // Check diagonals (random direction first)
            int direction = Random.value > 0.5f ? 1 : -1;
            
            Element diagonal1 = matrix.Get(myX + direction, myY - 1);
            if (diagonal1 != null && diagonal1.elementType == ElementType.EMPTYCELL)
            {
                SwapPositions(matrix, diagonal1, myX + direction, myY - 1);
                return;
            }
            
            // Try other diagonal
            Element diagonal2 = matrix.Get(myX - direction, myY - 1);
            if (diagonal2 != null && diagonal2.elementType == ElementType.EMPTYCELL)
            {
                SwapPositions(matrix, diagonal2, myX - direction, myY - 1);
                return;
            }
        }
        
        protected override bool ActOnNeighboringElement(Element neighbor, int modifiedMatrixX, int modifiedMatrixY, 
            CellularMatrix matrix, bool isFinal, bool isFirst, Vector3 lastValidLocation, int depth)
        {
            return false;
        }
    }
}

