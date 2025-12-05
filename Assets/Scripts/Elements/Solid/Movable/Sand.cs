namespace FallingSand.Elements
{
    public class Sand : MovableSolid
    {
        public Sand(int x, int y) : base(x, y)
        {
            mass = 50;
        }
        
        public override ElementType GetEnumType() => ElementType.SAND;
    }
}

