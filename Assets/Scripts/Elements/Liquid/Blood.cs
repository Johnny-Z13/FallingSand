namespace FallingSand.Elements
{
    public class Blood : Liquid
    {
        public Blood(int x, int y) : base(x, y)
        {
            mass = 21;
            dispersionRate = 4;
        }
        
        public override ElementType GetEnumType() => ElementType.BLOOD;
    }
}

