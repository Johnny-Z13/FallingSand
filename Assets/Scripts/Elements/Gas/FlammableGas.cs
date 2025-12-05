using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public class FlammableGas : Gas
    {
        public FlammableGas(int x, int y) : base(x, y)
        {
            lifeSpan = 500;
            flammabilityResistance = 50;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);
            
            if (isIgnited && Random.value > 0.7f)
            {
                matrix.AddExplosion(10, 3, this);
                Die(matrix);
            }
        }
        
        public override ElementType GetEnumType() => ElementType.FLAMMABLEGAS;
    }
}

