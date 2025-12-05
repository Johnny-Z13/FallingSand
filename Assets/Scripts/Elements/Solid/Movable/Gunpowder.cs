using FallingSand.Core;
using UnityEngine;

namespace FallingSand.Elements
{
    public class Gunpowder : MovableSolid
    {
        public Gunpowder(int x, int y) : base(x, y)
        {
            mass = 40;
            flammabilityResistance = 50;
            explosionRadius = 15;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);
            
            if (isIgnited && Random.value > 0.8f)
            {
                matrix.AddExplosion(explosionRadius, 5, this);
                Die(matrix);
            }
        }
        
        public override ElementType GetEnumType() => ElementType.GUNPOWDER;
    }
}

