using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public class Ember : MovableSolid
    {
        public Ember(int x, int y) : base(x, y)
        {
            mass = 40;
            isIgnited = true;
            lifeSpan = 300;
            heatFactor = 20;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);
            
            if (IsEffectsFrame())
            {
                // Apply heat to neighbors
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        Element neighbor = matrix.Get(GetMatrixX() + dx, GetMatrixY() + dy);
                        if (neighbor != null)
                            neighbor.ReceiveHeat(matrix, heatFactor);
                    }
                }
            }
            
            if (lifeSpan.HasValue)
            {
                lifeSpan--;
                if (lifeSpan <= 0)
                    Die(matrix);
            }
        }
        
        public override ElementType GetEnumType() => ElementType.EMBER;
    }
}

