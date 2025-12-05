namespace FallingSand.Elements
{
    public class Coal : MovableSolid
    {
        public Coal(int x, int y) : base(x, y)
        {
            mass = 60;
            flammabilityResistance = 200;
            health = 1000;
            fireDamage = 1;
        }
        
        public override ElementType GetEnumType() => ElementType.COAL;
    }
}

