using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public class Lava : Liquid
    {
        public Lava(int x, int y) : base(x, y)
        {
            mass = 50;
            dispersionRate = 2;
            temperature = 1000;
            heatFactor = 50;
        }
        
        protected override void CustomLiquidBehavior(CellularMatrix matrix)
        {
            // Burn neighbors
            if (IsEffectsFrame())
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        Element neighbor = matrix.Get(GetMatrixX() + dx, GetMatrixY() + dy);
                        if (neighbor != null)
                        {
                            neighbor.ReceiveHeat(matrix, heatFactor);
                            if (!(neighbor is Lava) && !(neighbor is EmptyCell))
                            {
                                neighbor.health -= 10;
                                neighbor.CheckIfDead(matrix);
                            }
                        }
                    }
                }
            }
        }
        
        public override ElementType GetEnumType() => ElementType.LAVA;
    }
}

