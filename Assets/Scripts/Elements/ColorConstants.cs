using UnityEngine;

namespace FallingSand.Elements
{
    public static class ColorConstants
    {
        // Solids - Movable
        private static readonly Color32 SAND_COLOR = new Color32(194, 178, 128, 255);
        private static readonly Color32 DIRT_COLOR = new Color32(101, 67, 33, 255);
        private static readonly Color32 SNOW_COLOR = new Color32(240, 255, 255, 255);
        private static readonly Color32 GUNPOWDER_COLOR = new Color32(64, 64, 64, 255);
        private static readonly Color32 COAL_COLOR = new Color32(40, 40, 40, 255);
        private static readonly Color32 EMBER_COLOR = new Color32(255, 100, 0, 255);
        
        // Solids - Immovable
        private static readonly Color32 STONE_COLOR = new Color32(128, 128, 128, 255);
        private static readonly Color32 BRICK_COLOR = new Color32(150, 75, 75, 255);
        private static readonly Color32 WOOD_COLOR = new Color32(139, 90, 43, 255);
        private static readonly Color32 GROUND_COLOR = new Color32(90, 60, 30, 255);
        private static readonly Color32 TITANIUM_COLOR = new Color32(192, 192, 192, 255);
        private static readonly Color32 SLIMEMOLD_COLOR = new Color32(100, 255, 100, 255);
        
        // Liquids
        private static readonly Color32 WATER_COLOR = new Color32(28, 163, 236, 255);
        private static readonly Color32 OIL_COLOR = new Color32(80, 60, 40, 255);
        private static readonly Color32 ACID_COLOR = new Color32(100, 255, 100, 255);
        private static readonly Color32 LAVA_COLOR = new Color32(255, 69, 0, 255);
        private static readonly Color32 BLOOD_COLOR = new Color32(139, 0, 0, 255);
        private static readonly Color32 CEMENT_COLOR = new Color32(160, 160, 160, 255);
        
        // Gases
        private static readonly Color32 STEAM_COLOR = new Color32(200, 200, 220, 200);
        private static readonly Color32 SMOKE_COLOR = new Color32(80, 80, 80, 180);
        private static readonly Color32 SPARK_COLOR = new Color32(255, 200, 50, 255);
        private static readonly Color32 EXPLOSIONSPARK_COLOR = new Color32(255, 150, 0, 255);
        private static readonly Color32 FLAMMABLEGAS_COLOR = new Color32(255, 100, 100, 150);
        
        // Special
        private static readonly Color32 EMPTY_COLOR = new Color32(0, 0, 0, 255);
        private static readonly Color32 PLAYERMEAT_COLOR = new Color32(255, 192, 203, 255);
        
        public static Color32 GetColorForElementType(ElementType type, int x = 0, int y = 0)
        {
            // Add slight variation based on position for visual interest
            float variation = (Mathf.Sin(x * 0.1f + y * 0.1f) * 0.1f + 1f);
            
            Color32 baseColor = type switch
            {
                ElementType.SAND => SAND_COLOR,
                ElementType.DIRT => DIRT_COLOR,
                ElementType.SNOW => SNOW_COLOR,
                ElementType.GUNPOWDER => GUNPOWDER_COLOR,
                ElementType.COAL => COAL_COLOR,
                ElementType.EMBER => EMBER_COLOR,
                ElementType.STONE => STONE_COLOR,
                ElementType.BRICK => BRICK_COLOR,
                ElementType.WOOD => WOOD_COLOR,
                ElementType.GROUND => GROUND_COLOR,
                ElementType.TITANIUM => TITANIUM_COLOR,
                ElementType.SLIMEMOLD => SLIMEMOLD_COLOR,
                ElementType.WATER => WATER_COLOR,
                ElementType.OIL => OIL_COLOR,
                ElementType.ACID => ACID_COLOR,
                ElementType.LAVA => LAVA_COLOR,
                ElementType.BLOOD => BLOOD_COLOR,
                ElementType.CEMENT => CEMENT_COLOR,
                ElementType.STEAM => STEAM_COLOR,
                ElementType.SMOKE => SMOKE_COLOR,
                ElementType.SPARK => SPARK_COLOR,
                ElementType.EXPLOSIONSPARK => EXPLOSIONSPARK_COLOR,
                ElementType.FLAMMABLEGAS => FLAMMABLEGAS_COLOR,
                ElementType.PLAYERMEAT => PLAYERMEAT_COLOR,
                _ => EMPTY_COLOR
            };
            
            // Apply subtle variation
            return new Color32(
                (byte)Mathf.Clamp(baseColor.r * variation, 0, 255),
                (byte)Mathf.Clamp(baseColor.g * variation, 0, 255),
                (byte)Mathf.Clamp(baseColor.b * variation, 0, 255),
                baseColor.a
            );
        }
    }
}

