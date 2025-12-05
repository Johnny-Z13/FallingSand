using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public class PlayerMeat : MovableSolid
    {
        public PlayerMeat(int x, int y) : base(x, y)
        {
            mass = 70;
            health = 100;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);
            
            // Player meat specific behavior can be added here
        }
        
        public override ElementType GetEnumType() => ElementType.PLAYERMEAT;
    }
}

