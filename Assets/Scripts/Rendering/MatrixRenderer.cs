using UnityEngine;
using FallingSand.Core;
using FallingSand.Elements;

namespace FallingSand.Rendering
{
    public class MatrixRenderer : MonoBehaviour
    {
        private CellularMatrix matrix;
        private GameConfig config;
        private Texture2D texture;
        private Color32[] pixels;
        private SpriteRenderer spriteRenderer;
        
        public void Initialize(CellularMatrix matrix, GameConfig config)
        {
            this.matrix = matrix;
            this.config = config;
            
            // Create texture
            texture = new Texture2D(config.screenWidth, config.screenHeight, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            
            // Initialize pixel array
            pixels = new Color32[config.screenWidth * config.screenHeight];
            
            // Setup sprite renderer
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            
            // Create sprite from texture
            Sprite sprite = Sprite.Create(texture, 
                new Rect(0, 0, config.screenWidth, config.screenHeight), 
                new Vector2(0.5f, 0.5f), 
                1f);
            spriteRenderer.sprite = sprite;
            
            // Position the renderer at screen center
            transform.position = new Vector3(config.screenWidth / 2f, config.screenHeight / 2f, 0f);
            
            Debug.Log($"MatrixRenderer initialized: {config.screenWidth}x{config.screenHeight} texture");
        }
        
        public void RenderMatrix()
        {
            if (matrix == null || texture == null)
                return;
            
            int pixSize = config.pixelSizeModifier;
            int screenWidth = config.screenWidth;
            
            // Draw elements with optimized pixel batching
            for (int y = 0; y < matrix.outerArraySize; y++)
            {
                int startPixelY = y * pixSize;
                int pixelYBase = startPixelY * screenWidth;
                
                for (int x = 0; x < matrix.innerArraySize; x++)
                {
                    Element element = matrix.Get(x, y);
                    Color32 color;
                    
                    if (element != null && element.elementType != ElementType.EMPTYCELL)
                    {
                        color = element.color;
                    }
                    else
                    {
                        color = Color.black;
                    }
                    
                    // Fill pixel block (optimized loop)
                    int startPixelX = x * pixSize;
                    
                    for (int py = 0; py < pixSize; py++)
                    {
                        int rowIndex = pixelYBase + py * screenWidth + startPixelX;
                        for (int px = 0; px < pixSize; px++)
                        {
                            pixels[rowIndex + px] = color;
                        }
                    }
                }
            }
            
            // Apply pixels to texture (false = no mipmaps for speed)
            texture.SetPixels32(pixels);
            texture.Apply(false);
        }
    }
}

