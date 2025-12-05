namespace FallingSand.Elements
{
    public class Smoke : Gas
    {
        public Smoke(int x, int y) : base(x, y)
        {
            lifeSpan = 300;
        }
        
        public override ElementType GetEnumType() => ElementType.SMOKE;
    }
}

