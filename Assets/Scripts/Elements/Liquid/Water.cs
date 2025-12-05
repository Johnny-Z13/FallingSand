using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public class Water : Liquid
    {
        public Water(int x, int y) : base(x, y)
        {
            mass = 20;
            dispersionRate = 5;
            coolingFactor = 10;
        }
        
        protected override void CustomLiquidBehavior(CellularMatrix matrix)
        {
            // Cool nearby elements
            if (IsEffectsFrame())
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        Element neighbor = matrix.Get(GetMatrixX() + dx, GetMatrixY() + dy);
                        if (neighbor != null && neighbor.isIgnited)
                        {
                            neighbor.ReceiveCooling(matrix, coolingFactor);
                        }
                    }
                }
            }
        }
        
        public override ElementType GetEnumType() => ElementType.WATER;
    }
}

