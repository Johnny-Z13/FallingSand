namespace FallingSand.Elements
{
    public class Dirt : MovableSolid
    {
        public Dirt(int x, int y) : base(x, y)
        {
            mass = 55;
        }
        
        public override ElementType GetEnumType() => ElementType.DIRT;
    }
}

