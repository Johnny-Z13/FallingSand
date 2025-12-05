using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public class SlimeMold : ImmovableSolid
    {
        public SlimeMold(int x, int y) : base(x, y)
        {
            health = 200;
            explosionResistance = 2;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);
            
            // Spread to neighboring cells occasionally
            if (Random.value > 0.99f)
            {
                int dx = Random.Range(-1, 2);
                int dy = Random.Range(-1, 2);
                Element neighbor = matrix.Get(GetMatrixX() + dx, GetMatrixY() + dy);
                if (neighbor != null && !(neighbor is EmptyCell) && !(neighbor is SlimeMold))
                {
                    neighbor.DieAndReplace(matrix, ElementType.SLIMEMOLD);
                }
            }
        }
        
        public override ElementType GetEnumType() => ElementType.SLIMEMOLD;
    }
}

