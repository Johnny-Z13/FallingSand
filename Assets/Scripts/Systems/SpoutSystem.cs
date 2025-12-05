using UnityEngine;
using FallingSand.Core;
using FallingSand.Elements;

namespace FallingSand.Systems
{
    public abstract class Spout
    {
        protected ElementType elementType;
        protected int matrixX;
        protected int matrixY;
        protected int brushSize;
        protected BrushType brushType;
        
        public Spout(ElementType elementType, int matrixX, int matrixY, int brushSize, BrushType brushType)
        {
            this.elementType = elementType;
            this.matrixX = matrixX;
            this.matrixY = matrixY;
            this.brushSize = brushSize;
            this.brushType = brushType;
        }
        
        public abstract void Spawn(CellularMatrix matrix);
    }
    
    public class ElementSpout : Spout
    {
        public ElementSpout(ElementType elementType, int matrixX, int matrixY, int brushSize, BrushType brushType)
            : base(elementType, matrixX, matrixY, brushSize, brushType)
        {
        }
        
        public override void Spawn(CellularMatrix matrix)
        {
            matrix.SpawnElementByMatrixWithBrush(matrixX, matrixY, elementType, brushSize, brushType);
        }
    }
    
    public class ParticleSpout : Spout
    {
        public ParticleSpout(ElementType elementType, int matrixX, int matrixY, int brushSize, BrushType brushType)
            : base(elementType, matrixX, matrixY, brushSize, brushType)
        {
        }
        
        public override void Spawn(CellularMatrix matrix)
        {
            // Spawn with random velocities
            int halfBrush = Mathf.FloorToInt(brushSize / 2f);
            for (int x = matrixX - halfBrush; x <= matrixX + halfBrush; x++)
            {
                for (int y = matrixY - halfBrush; y <= matrixY + halfBrush; y++)
                {
                    if (brushType == BrushType.CIRCLE)
                    {
                        int distance = CellularMatrix.DistanceBetweenTwoPoints(matrixX, x, matrixY, y);
                        if (distance < halfBrush && Random.value > 0.7f)
                        {
                            Vector3 velocity = new Vector3(
                                Random.Range(-200f, 200f),
                                Random.Range(-200f, 200f),
                                0f
                            );
                            // Spawn with velocity
                            matrix.SpawnElementByMatrix(x, y, elementType);
                        }
                    }
                }
            }
        }
    }
}

