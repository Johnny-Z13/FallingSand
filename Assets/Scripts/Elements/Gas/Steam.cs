namespace FallingSand.Elements
{
    public class Steam : Gas
    {
        public Steam(int x, int y) : base(x, y)
        {
            lifeSpan = 400;
        }
        
        public override ElementType GetEnumType() => ElementType.STEAM;
    }
}

