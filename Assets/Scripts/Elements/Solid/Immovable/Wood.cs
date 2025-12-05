namespace FallingSand.Elements
{
    public class Wood : ImmovableSolid
    {
        public Wood(int x, int y) : base(x, y)
        {
            health = 800;
            explosionResistance = 5;
            flammabilityResistance = 100;
            fireDamage = 5;
        }
        
        public override ElementType GetEnumType() => ElementType.WOOD;
    }
}

