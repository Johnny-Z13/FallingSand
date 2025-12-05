using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public class ExplosionSpark : Gas
    {
        public ExplosionSpark(int x, int y) : base(x, y)
        {
            lifeSpan = 30;
            heatFactor = 25;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);
            
            // Apply heat to neighbors
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
                        }
                    }
                }
            }
        }
        
        public override ElementType GetEnumType() => ElementType.EXPLOSIONSPARK;
    }
}

