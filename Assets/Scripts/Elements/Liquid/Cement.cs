using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public class Cement : Liquid
    {
        private int solidifyTime = 600;
        
        public Cement(int x, int y) : base(x, y)
        {
            mass = 30;
            dispersionRate = 2;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            base.Step(matrix);
            
            solidifyTime--;
            if (solidifyTime <= 0)
            {
                DieAndReplace(matrix, ElementType.STONE);
            }
        }
        
        public override ElementType GetEnumType() => ElementType.CEMENT;
    }
}

