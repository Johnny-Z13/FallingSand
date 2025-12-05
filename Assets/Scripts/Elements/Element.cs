using UnityEngine;
using System.Collections.Generic;
using FallingSand.Core;

namespace FallingSand.Elements
{
    public abstract class Element
    {
        // Position
        public int pixelX;
        public int pixelY;
        private int matrixX;
        private int matrixY;
        
        // Physics
        public Vector3 vel;
        public float frictionFactor;
        public bool isFreeFalling = true;
        public float inertialResistance;
        public int stoppedMovingCount = 0;
        public int stoppedMovingThreshold = 1;
        public int mass;
        
        // Health/Damage
        public int health = 500;
        public int explosionResistance = 1;
        public int explosionRadius = 0;
        
        // Fire/Heat
        public int flammabilityResistance = 100;
        public int resetFlammabilityResistance;
        public bool isIgnited;
        public int heatFactor = 10;
        public int fireDamage = 3;
        public bool heated = false;
        public int temperature = 0;
        public int coolingFactor = 5;
        
        // Visual
        public Color32 color;
        public bool discolored = false;
        
        // Lifecycle
        public int? lifeSpan = null;
        public bool isDead = false;
        
        // Type
        public ElementType elementType;
        
        // Physics body reference
        public FallingSand.Physics.PhysicsElementActor owningBody = null;
        public Vector2Int? owningBodyCoords = null;
        public List<Vector2Int> secondaryMatrixCoords = new List<Vector2Int>();
        
        // Stepping
        public bool stepped;
        public float xThreshold = 0;
        public float yThreshold = 0;
        
        protected Element(int x, int y)
        {
            SetCoordinatesByMatrix(x, y);
            this.elementType = GetEnumType();
            this.color = ColorConstants.GetColorForElementType(this.elementType, x, y);
            this.resetFlammabilityResistance = flammabilityResistance / 2;
        }
        
        public abstract void Step(CellularMatrix matrix);
        
        protected abstract bool ActOnNeighboringElement(Element neighbor, int modifiedMatrixX, int modifiedMatrixY, 
            CellularMatrix matrix, bool isFinal, bool isFirst, Vector3 lastValidLocation, int depth);
        
        public virtual bool ActOnOther(Element other, CellularMatrix matrix)
        {
            return false;
        }
        
        public virtual void CustomElementFunctions(CellularMatrix matrix) { }
        
        public void SetVelocity(Vector3 velocity)
        {
            this.vel = velocity;
        }
        
        public void SwapPositions(CellularMatrix matrix, Element toSwap)
        {
            SwapPositions(matrix, toSwap, toSwap.GetMatrixX(), toSwap.GetMatrixY());
        }
        
        public void SwapPositions(CellularMatrix matrix, Element toSwap, int toSwapX, int toSwapY)
        {
            if (this.GetMatrixX() == toSwapX && this.GetMatrixY() == toSwapY)
                return;
                
            matrix.SetElementAtIndex(this.GetMatrixX(), this.GetMatrixY(), toSwap);
            matrix.SetElementAtIndex(toSwapX, toSwapY, this);
        }
        
        public void MoveToLastValid(CellularMatrix matrix, Vector3 moveToLocation)
        {
            if ((int)moveToLocation.x == GetMatrixX() && (int)moveToLocation.y == GetMatrixY()) 
                return;
                
            Element toSwap = matrix.Get((int)moveToLocation.x, (int)moveToLocation.y);
            SwapPositions(matrix, toSwap, (int)moveToLocation.x, (int)moveToLocation.y);
        }
        
        public void Die(CellularMatrix matrix)
        {
            Die(matrix, ElementType.EMPTYCELL);
        }
        
        protected void Die(CellularMatrix matrix, ElementType type)
        {
            this.isDead = true;
            Element newElement = type.CreateElementByMatrix(GetMatrixX(), GetMatrixY());
            matrix.SetElementAtIndex(GetMatrixX(), GetMatrixY(), newElement);
            matrix.ReportToChunkActive(GetMatrixX(), GetMatrixY());
            
            if (owningBody != null)
            {
                owningBody.ElementDeath(this, newElement);
                foreach (var coords in secondaryMatrixCoords)
                {
                    matrix.SetElementAtIndex(coords.x, coords.y, 
                        ElementType.EMPTYCELL.CreateElementByMatrix(0, 0));
                }
            }
        }
        
        public void DieAndReplace(CellularMatrix matrix, ElementType type)
        {
            Die(matrix, type);
        }
        
        public bool ReceiveHeat(CellularMatrix matrix, int heat)
        {
            if (isIgnited) return false;
            
            this.flammabilityResistance -= (int)(Random.value * heat);
            CheckIfIgnited();
            return true;
        }
        
        public bool ReceiveCooling(CellularMatrix matrix, int cooling)
        {
            if (isIgnited)
            {
                this.flammabilityResistance += cooling;
                CheckIfIgnited();
                return true;
            }
            return false;
        }
        
        public void CheckIfIgnited()
        {
            if (this.flammabilityResistance <= 0)
            {
                this.isIgnited = true;
                ModifyColor();
            }
            else
            {
                this.isIgnited = false;
                this.color = ColorConstants.GetColorForElementType(elementType, this.GetMatrixX(), this.GetMatrixY());
            }
        }
        
        public void CheckIfDead(CellularMatrix matrix)
        {
            if (this.health <= 0)
                Die(matrix);
        }
        
        public virtual void ModifyColor()
        {
            if (isIgnited)
                color = EffectColors.GetRandomFireColor();
        }
        
        public bool Explode(CellularMatrix matrix, int strength)
        {
            if (explosionResistance < strength)
            {
                if (Random.value > 0.3f)
                    DieAndReplace(matrix, ElementType.EXPLOSIONSPARK);
                else
                    Die(matrix);
                return true;
            }
            else
            {
                DarkenColor();
                return false;
            }
        }
        
        public void DarkenColor(float factor = 0.85f)
        {
            this.color = new Color32(
                (byte)(color.r * factor),
                (byte)(color.g * factor),
                (byte)(color.b * factor),
                color.a
            );
            this.discolored = true;
        }
        
        public void SetCoordinatesByMatrix(int providedX, int providedY)
        {
            SetXByMatrix(providedX);
            SetYByMatrix(providedY);
        }
        
        public void SetXByMatrix(int providedVal)
        {
            this.SetMatrixX(providedVal);
            this.pixelX = ToPixel(providedVal);
        }
        
        public void SetYByMatrix(int providedVal)
        {
            this.SetMatrixY(providedVal);
            this.pixelY = ToPixel(providedVal);
        }
        
        public int ToMatrix(int pixelVal)
        {
            return Mathf.FloorToInt(pixelVal / (float)CellularAutomaton.Instance.config.pixelSizeModifier);
        }
        
        public int ToPixel(int matrixVal)
        {
            return Mathf.FloorToInt(matrixVal * CellularAutomaton.Instance.config.pixelSizeModifier);
        }
        
        public int GetMatrixX() => matrixX;
        public void SetMatrixX(int value) => matrixX = value;
        public int GetMatrixY() => matrixY;
        public void SetMatrixY(int value) => matrixY = value;
        
        public abstract ElementType GetEnumType();
        
        public bool IsReactionFrame() => CellularAutomaton.frameCount == 3;
        public bool IsEffectsFrame() => CellularAutomaton.frameCount == 1;
    }
}

