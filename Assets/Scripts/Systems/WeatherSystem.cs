using UnityEngine;
using FallingSand.Core;
using FallingSand.Elements;

namespace FallingSand.Systems
{
    public class WeatherSystem
    {
        private ElementType weatherElement;
        private int intensity;
        private bool enabled = false;
        
        public WeatherSystem(ElementType weatherElement, int intensity)
        {
            this.weatherElement = weatherElement;
            this.intensity = intensity;
        }
        
        public void SetWeatherElement(ElementType element)
        {
            this.weatherElement = element;
        }
        
        public void SetIntensity(int intensity)
        {
            this.intensity = intensity;
        }
        
    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }
    
    public bool IsEnabled()
    {
        return enabled;
    }
        
        public void Enact(CellularMatrix matrix)
        {
            if (!enabled) return;
            
            // Spawn elements from the top of the screen
            for (int i = 0; i < intensity; i++)
            {
                int randomX = Random.Range(0, matrix.innerArraySize);
                int spawnY = matrix.outerArraySize - 1;
                
                Element current = matrix.Get(randomX, spawnY);
                if (current is EmptyCell)
                {
                    matrix.SpawnElementByMatrix(randomX, spawnY, weatherElement);
                }
            }
        }
    }
}

