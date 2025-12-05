namespace FallingSand.Elements
{
    public class Stone : ImmovableSolid
    {
        public Stone(int x, int y) : base(x, y)
        {
            health = 2000;
            explosionResistance = 15;
        }
        
        public override ElementType GetEnumType() => ElementType.STONE;
    }
}

