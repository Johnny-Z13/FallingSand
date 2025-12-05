using UnityEngine;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public class EmptyCell : Element
    {
        private static EmptyCell instance;
        public static EmptyCell Instance
        {
            get
            {
                if (instance == null)
                    instance = new EmptyCell(0, 0);
                return instance;
            }
        }
        
        private EmptyCell(int x, int y) : base(x, y)
        {
            mass = 0;
            health = int.MaxValue;
        }
        
        public override void Step(CellularMatrix matrix)
        {
            // Empty cells don't do anything
        }
        
        protected override bool ActOnNeighboringElement(Element neighbor, int modifiedMatrixX, int modifiedMatrixY, 
            CellularMatrix matrix, bool isFinal, bool isFirst, Vector3 lastValidLocation, int depth)
        {
            return false;
        }
        
        public override ElementType GetEnumType()
        {
            return ElementType.EMPTYCELL;
        }
    }
}

