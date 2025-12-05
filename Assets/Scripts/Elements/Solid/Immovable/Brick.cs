namespace FallingSand.Elements
{
    public class Brick : ImmovableSolid
    {
        public Brick(int x, int y) : base(x, y)
        {
            health = 1500;
            explosionResistance = 12;
            flammabilityResistance = 300;
        }
        
        public override ElementType GetEnumType() => ElementType.BRICK;
    }
}

