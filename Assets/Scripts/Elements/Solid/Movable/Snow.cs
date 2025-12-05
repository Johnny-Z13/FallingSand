using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public class Snow : MovableSolid
    {
        public Snow(int x, int y) : base(x, y)
        {
            mass = 30;
            temperature = -20;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);
            
            // Melt snow when heated
            if (temperature > 0)
            {
                DieAndReplace(matrix, ElementType.WATER);
            }
        }
        
        public override ElementType GetEnumType() => ElementType.SNOW;
    }
}

