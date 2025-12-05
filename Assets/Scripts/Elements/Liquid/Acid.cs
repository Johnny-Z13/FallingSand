using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public class Acid : Liquid
    {
        public Acid(int x, int y) : base(x, y)
        {
            mass = 22;
            dispersionRate = 3;
        }
        
        protected override void CustomLiquidBehavior(CellularMatrix matrix)
        {
            // Corrode neighbors
            if (IsReactionFrame())
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        
                        Element neighbor = matrix.Get(GetMatrixX() + dx, GetMatrixY() + dy);
                        if (neighbor != null && !(neighbor is Acid) && !(neighbor is EmptyCell) && Random.value > 0.9f)
                        {
                            neighbor.health -= 170;
                            neighbor.CheckIfDead(matrix);
                        }
                    }
                }
            }
        }
        
        public override ElementType GetEnumType() => ElementType.ACID;
    }
}

