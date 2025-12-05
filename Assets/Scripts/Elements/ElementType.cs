using UnityEngine;

namespace FallingSand.Elements
{
    public enum ElementType
    {
        EMPTYCELL,
        
        // Movable Solids
        SAND,
        DIRT,
        SNOW,
        GUNPOWDER,
        COAL,
        EMBER,
        
        // Immovable Solids
        STONE,
        BRICK,
        WOOD,
        GROUND,
        TITANIUM,
        SLIMEMOLD,
        
        // Liquids
        WATER,
        OIL,
        ACID,
        LAVA,
        BLOOD,
        CEMENT,
        
        // Gases
        STEAM,
        SMOKE,
        SPARK,
        EXPLOSIONSPARK,
        FLAMMABLEGAS,
        GAS,
        
        // Special
        PLAYERMEAT,
        PARTICLE
    }
    
    public static class ElementTypeExtensions
    {
        public static Element CreateElementByMatrix(this ElementType type, int x, int y)
        {
            return type switch
            {
                ElementType.EMPTYCELL => EmptyCell.Instance,
                ElementType.SAND => new Sand(x, y),
                ElementType.DIRT => new Dirt(x, y),
                ElementType.SNOW => new Snow(x, y),
                ElementType.GUNPOWDER => new Gunpowder(x, y),
                ElementType.COAL => new Coal(x, y),
                ElementType.EMBER => new Ember(x, y),
                ElementType.STONE => new Stone(x, y),
                ElementType.BRICK => new Brick(x, y),
                ElementType.WOOD => new Wood(x, y),
                ElementType.GROUND => new Ground(x, y),
                ElementType.TITANIUM => new Titanium(x, y),
                ElementType.SLIMEMOLD => new SlimeMold(x, y),
                ElementType.WATER => new Water(x, y),
                ElementType.OIL => new Oil(x, y),
                ElementType.ACID => new Acid(x, y),
                ElementType.LAVA => new Lava(x, y),
                ElementType.BLOOD => new Blood(x, y),
                ElementType.CEMENT => new Cement(x, y),
                ElementType.STEAM => new Steam(x, y),
                ElementType.SMOKE => new Smoke(x, y),
                ElementType.SPARK => new Spark(x, y),
                ElementType.EXPLOSIONSPARK => new ExplosionSpark(x, y),
                ElementType.FLAMMABLEGAS => new FlammableGas(x, y),
                ElementType.GAS => new Gas(x, y),
                ElementType.PLAYERMEAT => new PlayerMeat(x, y),
                _ => EmptyCell.Instance
            };
        }
    }
}

