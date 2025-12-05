using UnityEngine;

namespace FallingSand.Elements
{
    public static class EffectColors
    {
        private static readonly Color32[] FIRE_COLORS = new Color32[]
        {
            new Color32(255, 69, 0, 255),    // Red-Orange
            new Color32(255, 140, 0, 255),   // Dark Orange
            new Color32(255, 165, 0, 255),   // Orange
            new Color32(255, 215, 0, 255),   // Gold
            new Color32(255, 255, 0, 255),   // Yellow
            new Color32(255, 100, 0, 255),   // Bright Red-Orange
        };
        
        public static Color32 GetRandomFireColor()
        {
            return FIRE_COLORS[Random.Range(0, FIRE_COLORS.Length)];
        }
    }
}

