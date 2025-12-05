using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FallingSand.Elements;
using FallingSand.Util;

namespace FallingSand.Core
{
    public class CellularMatrix
    {
        public int innerArraySize; // width
        public int outerArraySize; // height
        public int pixelSizeModifier;
        public bool useChunks = true;
        
        private Element[,] matrix;
        private Chunk[,] chunks;
        private List<int> shuffledXIndexes;
        private List<List<int>> shuffledXIndexesForThreads;
        
        public List<Explosion> explosionArray = new List<Explosion>();
        public List<FallingSand.Entities.Boid> boids = new List<FallingSand.Entities.Boid>();
        public List<Spout> spoutArray = new List<Spout>();
        public List<FallingSand.Physics.PhysicsElementActor> physicsElementActors = new List<FallingSand.Physics.PhysicsElementActor>();
        
        public Rigidbody2D world; // Physics2D world reference
        
        public CellularMatrix(int width, int height, int pixelSizeModifier)
        {
            this.pixelSizeModifier = pixelSizeModifier;
            this.innerArraySize = ToMatrix(width);
            this.outerArraySize = ToMatrix(height);
            this.matrix = GenerateMatrix();
            this.chunks = GenerateChunks();
            this.shuffledXIndexes = GenerateShuffledIndexes(innerArraySize);
        }
        
        private Element[,] GenerateMatrix()
        {
            Element[,] newMatrix = new Element[outerArraySize, innerArraySize];
            for (int y = 0; y < outerArraySize; y++)
            {
                for (int x = 0; x < innerArraySize; x++)
                {
                    newMatrix[y, x] = ElementType.EMPTYCELL.CreateElementByMatrix(x, y);
                }
            }
            return newMatrix;
        }
        
        private Chunk[,] GenerateChunks()
        {
            int rows = Mathf.CeilToInt((float)outerArraySize / Chunk.size);
            int columns = Mathf.CeilToInt((float)innerArraySize / Chunk.size);
            Chunk[,] newChunks = new Chunk[rows, columns];
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    Chunk newChunk = new Chunk();
                    int xPos = c * Chunk.size;
                    int yPos = r * Chunk.size;
                    newChunk.SetTopLeft(new Vector2Int(Mathf.Min(xPos, innerArraySize), Mathf.Min(yPos, outerArraySize)));
                    newChunk.SetBottomRight(new Vector2Int(Mathf.Min(xPos + Chunk.size, innerArraySize), Mathf.Min(yPos + Chunk.size, outerArraySize)));
                    newChunks[r, c] = newChunk;
                }
            }
            return newChunks;
        }
        
        private List<int> GenerateShuffledIndexes(int size)
        {
            List<int> list = new List<int>(size);
            for (int i = 0; i < size; i++)
                list.Add(i);
            return list;
        }
        
        public void ReshuffleXIndexes()
        {
            // Fisher-Yates shuffle
            for (int i = shuffledXIndexes.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int temp = shuffledXIndexes[i];
                shuffledXIndexes[i] = shuffledXIndexes[j];
                shuffledXIndexes[j] = temp;
            }
        }
        
        public void StepAll()
        {
            for (int y = 0; y < outerArraySize; y++)
            {
                foreach (int x in shuffledXIndexes)
                {
                    Element element = matrix[y, x];
                    if (element != null && element.stepped == false)
                    {
                        element.Step(this);
                    }
                }
            }
            
            // Reset stepped flags
            for (int y = 0; y < outerArraySize; y++)
            {
                for (int x = 0; x < innerArraySize; x++)
                {
                    matrix[y, x].stepped = false;
                }
            }
        }
        
        public Element Get(int x, int y)
        {
            if (IsWithinBounds(x, y))
                return matrix[y, x];
            return null;
        }
        
        public void SetElementAtIndex(int x, int y, Element element)
        {
            if (IsWithinBounds(x, y))
            {
                matrix[y, x] = element;
                element.SetCoordinatesByMatrix(x, y);
            }
        }
        
        public Element SpawnElementByMatrix(int matrixX, int matrixY, ElementType elementType)
        {
            if (IsWithinBounds(matrixX, matrixY))
            {
                Element currentElement = Get(matrixX, matrixY);
                if (currentElement.GetType() != elementType.GetType() && !(currentElement is PlayerMeat))
                {
                    Get(matrixX, matrixY).Die(this);
                    Element newElement = elementType.CreateElementByMatrix(matrixX, matrixY);
                    SetElementAtIndex(matrixX, matrixY, newElement);
                    ReportToChunkActive(newElement);
                    return newElement;
                }
            }
            return null;
        }
        
        public void SpawnElementBetweenTwoPoints(Vector3 pos1, Vector3 pos2, ElementType elementType, int brushSize, BrushType brushType)
        {
            int matrixX1 = ToMatrix((int)pos1.x);
            int matrixY1 = ToMatrix((int)pos1.y);
            int matrixX2 = ToMatrix((int)pos2.x);
            int matrixY2 = ToMatrix((int)pos2.y);
            
            if (pos1 == pos2)
            {
                SpawnElementByMatrixWithBrush(matrixX1, matrixY1, elementType, brushSize, brushType);
                return;
            }
            
            // Line interpolation
            int steps = Mathf.Max(Mathf.Abs(matrixX2 - matrixX1), Mathf.Abs(matrixY2 - matrixY1));
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                int x = Mathf.RoundToInt(Mathf.Lerp(matrixX1, matrixX2, t));
                int y = Mathf.RoundToInt(Mathf.Lerp(matrixY1, matrixY2, t));
                SpawnElementByMatrixWithBrush(x, y, elementType, brushSize, brushType);
            }
        }
        
        public void SpawnElementByMatrixWithBrush(int matrixX, int matrixY, ElementType elementType, int brushSize, BrushType brushType)
        {
            int halfBrush = Mathf.FloorToInt(brushSize / 2f);
            for (int x = matrixX - halfBrush; x <= matrixX + halfBrush; x++)
            {
                for (int y = matrixY - halfBrush; y <= matrixY + halfBrush; y++)
                {
                    if (brushType == BrushType.CIRCLE)
                    {
                        int distance = DistanceBetweenTwoPoints(matrixX, x, matrixY, y);
                        if (distance < halfBrush)
                        {
                            SpawnElementByMatrix(x, y, elementType);
                        }
                    }
                    else
                    {
                        SpawnElementByMatrix(x, y, elementType);
                    }
                }
            }
        }
        
        public void AddExplosion(int radius, int strength, Element sourceElement)
        {
            explosionArray.Add(new Explosion(this, radius, strength, sourceElement));
        }
        
        public void ExecuteExplosions()
        {
            foreach (var explosion in explosionArray)
            {
                explosion.Enact();
            }
            explosionArray.Clear();
        }
        
        public bool IsWithinBounds(int matrixX, int matrixY)
        {
            return matrixX >= 0 && matrixY >= 0 && matrixX < innerArraySize && matrixY < outerArraySize;
        }
        
        public int ToMatrix(int pixelVal)
        {
            return pixelVal / pixelSizeModifier;
        }
        
        public int ToPixel(int matrixVal)
        {
            return matrixVal * pixelSizeModifier;
        }
        
        public static int DistanceBetweenTwoPoints(int x1, int x2, int y1, int y2)
        {
            return Mathf.CeilToInt(Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2)));
        }
        
        public void ReportToChunkActive(Element element)
        {
            ReportToChunkActive(element.GetMatrixX(), element.GetMatrixY());
        }
        
        public void ReportToChunkActive(int x, int y)
        {
            if (useChunks && IsWithinBounds(x, y))
            {
                Chunk chunk = GetChunkForCoordinates(x, y);
                if (chunk != null)
                    chunk.SetShouldStepNextFrame(true);
                    
                // Mark neighboring chunks if at edge
                if (x % Chunk.size == 0)
                {
                    Chunk leftChunk = GetChunkForCoordinates(x - 1, y);
                    if (leftChunk != null) leftChunk.SetShouldStepNextFrame(true);
                }
                if (x % Chunk.size == Chunk.size - 1)
                {
                    Chunk rightChunk = GetChunkForCoordinates(x + 1, y);
                    if (rightChunk != null) rightChunk.SetShouldStepNextFrame(true);
                }
                if (y % Chunk.size == 0)
                {
                    Chunk bottomChunk = GetChunkForCoordinates(x, y - 1);
                    if (bottomChunk != null) bottomChunk.SetShouldStepNextFrame(true);
                }
                if (y % Chunk.size == Chunk.size - 1)
                {
                    Chunk topChunk = GetChunkForCoordinates(x, y + 1);
                    if (topChunk != null) topChunk.SetShouldStepNextFrame(true);
                }
            }
        }
        
        public bool ShouldElementInChunkStep(Element element)
        {
            return GetChunkForElement(element).GetShouldStep();
        }
        
        public Chunk GetChunkForElement(Element element)
        {
            return GetChunkForCoordinates(element.GetMatrixX(), element.GetMatrixY());
        }
        
        public Chunk GetChunkForCoordinates(int x, int y)
        {
            if (IsWithinBounds(x, y))
            {
                int chunkY = y / Chunk.size;
                int chunkX = x / Chunk.size;
                if (chunkY >= 0 && chunkY < chunks.GetLength(0) && chunkX >= 0 && chunkX < chunks.GetLength(1))
                    return chunks[chunkY, chunkX];
            }
            return null;
        }
        
        public void ResetChunks()
        {
            for (int r = 0; r < chunks.GetLength(0); r++)
            {
                for (int c = 0; c < chunks.GetLength(1); c++)
                {
                    chunks[r, c].ShiftShouldStepAndReset();
                }
            }
        }
        
        public void ClearAll()
        {
            matrix = GenerateMatrix();
            spoutArray.Clear();
            explosionArray.Clear();
            boids.Clear();
            physicsElementActors.Clear();
            
            for (int r = 0; r < chunks.GetLength(0); r++)
            {
                for (int c = 0; c < chunks.GetLength(1); c++)
                {
                    chunks[r, c].RemoveAllBoids();
                }
            }
        }
    }
    
    public enum BrushType
    {
        CIRCLE,
        SQUARE
    }
    
    // Placeholder classes - will be implemented in later phases
    public class Explosion
    {
        private CellularMatrix matrix;
        private int radius;
        private int strength;
        private int centerX, centerY;
        
        public Explosion(CellularMatrix matrix, int radius, int strength, Element sourceElement)
        {
            this.matrix = matrix;
            this.radius = radius;
            this.strength = strength;
            this.centerX = sourceElement.GetMatrixX();
            this.centerY = sourceElement.GetMatrixY();
        }
        
        public void Enact()
        {
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    int distance = CellularMatrix.DistanceBetweenTwoPoints(centerX, x, centerY, y);
                    if (distance <= radius)
                    {
                        Element element = matrix.Get(x, y);
                        if (element != null)
                        {
                            element.Explode(matrix, strength);
                        }
                    }
                }
            }
        }
    }
    
    public class Spout
    {
        // Placeholder - will be implemented in additional systems phase
    }
}

