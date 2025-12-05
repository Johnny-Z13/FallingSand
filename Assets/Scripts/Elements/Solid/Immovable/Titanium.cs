namespace FallingSand.Elements
{
    public class Titanium : ImmovableSolid
    {
        public Titanium(int x, int y) : base(x, y)
        {
            health = 5000;
            explosionResistance = 50;
            flammabilityResistance = int.MaxValue;
        }
        
        public override ElementType GetEnumType() => ElementType.TITANIUM;
    }
}

