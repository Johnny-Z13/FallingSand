namespace FallingSand.Elements
{
    public class Ground : ImmovableSolid
    {
        public Ground(int x, int y) : base(x, y)
        {
            health = 1200;
            explosionResistance = 8;
        }
        
        public override ElementType GetEnumType() => ElementType.GROUND;
    }
}

