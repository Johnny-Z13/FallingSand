namespace FallingSand.Elements
{
    public class Oil : Liquid
    {
        public Oil(int x, int y) : base(x, y)
        {
            mass = 18;
            dispersionRate = 4;
            flammabilityResistance = 80;
        }
        
        public override ElementType GetEnumType() => ElementType.OIL;
    }
}

