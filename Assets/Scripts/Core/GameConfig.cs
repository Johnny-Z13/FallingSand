using UnityEngine;

namespace FallingSand.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "FallingSand/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Screen Settings")]
        public int screenWidth = 1280;
        public int screenHeight = 800;
        public int pixelSizeModifier = 6;
        public int box2dSizeModifier = 10;
        
        [Header("Physics")]
        public Vector3 gravity = new Vector3(0f, -5f, 0f);
        
        [Header("Threading")]
        public int numThreads = 12;
        public bool useMultiThreading = true;
        
        [Header("Optimization")]
        public bool useChunks = true;
        
        [Header("Performance Tuning")]
        [Tooltip("Higher = smaller matrix = better FPS")]
        [Range(4, 12)]
        public int performancePixelSize = 6;
        
        public int MatrixWidth => screenWidth / pixelSizeModifier;
        public int MatrixHeight => screenHeight / pixelSizeModifier;
    }
}

