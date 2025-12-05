using UnityEngine;
using FallingSand.Core;
using FallingSand.Elements;
using FallingSand.Systems;

namespace FallingSand.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        [Header("Settings")]
        public int maxBrushSize = 50;
        public int minBrushSize = 1;
        public int brushSize = 5;
        public BrushType brushType = BrushType.CIRCLE;
        
        [Header("References")]
        public CellularAutomaton cellularAutomaton;
        public Camera mainCamera;
        
        private ElementType currentlySelectedElement = ElementType.SAND;
        private MouseMode mouseMode = MouseMode.SPAWN;
        private Vector3 lastMousePos;
        private bool wasMouseDown = false;
        
        public WeatherSystem weatherSystem;
        
        void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
                
            weatherSystem = new WeatherSystem(ElementType.SNOW, 2);
        }
        
        void Update()
        {
            HandleBrushControls();
            HandleElementSelection();
            HandleMouseInput();
            HandleModeChanges();
            HandleWeather();
        }
        
        void HandleBrushControls()
        {
            // Brush size
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                brushSize = Mathf.Clamp(brushSize + (int)(scroll * 2), minBrushSize, maxBrushSize);
            }
            
            // Brush type toggle
            if (Input.GetKeyDown(KeyCode.B))
            {
                brushType = brushType == BrushType.CIRCLE ? BrushType.SQUARE : BrushType.CIRCLE;
                Debug.Log($"Brush type: {brushType}");
            }
        }
        
        void HandleElementSelection()
        {
            // Number keys for common elements (1-9, 0)
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetElement(ElementType.SAND);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetElement(ElementType.WATER);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetElement(ElementType.STONE);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SetElement(ElementType.WOOD);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SetElement(ElementType.LAVA);
            if (Input.GetKeyDown(KeyCode.Alpha6)) SetElement(ElementType.GUNPOWDER);
            if (Input.GetKeyDown(KeyCode.Alpha7)) SetElement(ElementType.OIL);
            if (Input.GetKeyDown(KeyCode.Alpha8)) SetElement(ElementType.ACID);
            if (Input.GetKeyDown(KeyCode.Alpha9)) SetElement(ElementType.SNOW);
            if (Input.GetKeyDown(KeyCode.Alpha0)) SetElement(ElementType.EMPTYCELL);
            
            // F keys for additional movable solids
            if (Input.GetKeyDown(KeyCode.F1)) SetElement(ElementType.DIRT);
            if (Input.GetKeyDown(KeyCode.F2)) SetElement(ElementType.COAL);
            if (Input.GetKeyDown(KeyCode.F3)) SetElement(ElementType.EMBER);
            
            // F keys for immovable solids
            if (Input.GetKeyDown(KeyCode.F4)) SetElement(ElementType.BRICK);
            if (Input.GetKeyDown(KeyCode.F5)) SetElement(ElementType.GROUND);
            if (Input.GetKeyDown(KeyCode.F6)) SetElement(ElementType.TITANIUM);
            if (Input.GetKeyDown(KeyCode.F7)) SetElement(ElementType.SLIMEMOLD);
            
            // F keys for liquids
            if (Input.GetKeyDown(KeyCode.F8)) SetElement(ElementType.BLOOD);
            if (Input.GetKeyDown(KeyCode.F9)) SetElement(ElementType.CEMENT);
            
            // F keys for gases
            if (Input.GetKeyDown(KeyCode.F10)) SetElement(ElementType.STEAM);
            if (Input.GetKeyDown(KeyCode.F11)) SetElement(ElementType.SMOKE);
            if (Input.GetKeyDown(KeyCode.F12)) SetElement(ElementType.SPARK);
            
            // Letter keys for remaining gases
            if (Input.GetKeyDown(KeyCode.Q)) SetElement(ElementType.EXPLOSIONSPARK);
            if (Input.GetKeyDown(KeyCode.E)) SetElement(ElementType.FLAMMABLEGAS);
            if (Input.GetKeyDown(KeyCode.R)) SetElement(ElementType.GAS);
            
            // Special elements
            if (Input.GetKeyDown(KeyCode.T)) SetElement(ElementType.PLAYERMEAT);
        }
        
        void HandleMouseInput()
        {
            Vector3 mousePos = GetMouseWorldPosition();
            
            if (Input.GetMouseButton(0))
            {
                if (wasMouseDown && mouseMode == MouseMode.SPAWN)
                {
                    // Draw line between last pos and current pos
                    cellularAutomaton.matrix.SpawnElementBetweenTwoPoints(
                        lastMousePos, mousePos, currentlySelectedElement, brushSize, brushType);
                }
                else
                {
                    // Single point spawn
                    cellularAutomaton.matrix.SpawnElementByMatrixWithBrush(
                        cellularAutomaton.matrix.ToMatrix((int)mousePos.x),
                        cellularAutomaton.matrix.ToMatrix((int)mousePos.y),
                        currentlySelectedElement,
                        brushSize,
                        brushType
                    );
                }
                wasMouseDown = true;
            }
            else
            {
                wasMouseDown = false;
            }
            
            lastMousePos = mousePos;
        }
        
        void HandleModeChanges()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                mouseMode = mouseMode == MouseMode.SPAWN ? MouseMode.DELETE : MouseMode.SPAWN;
                Debug.Log($"Mouse mode: {mouseMode}");
            }
        }
        
        void HandleWeather()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                bool newState = !weatherSystem.IsEnabled();
                weatherSystem.SetEnabled(newState);
                Debug.Log($"Weather toggled: {(newState ? "ON" : "OFF")}");
            }
            
            weatherSystem?.Enact(cellularAutomaton.matrix);
        }
        
        Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return mousePos;
        }
        
        void SetElement(ElementType element)
        {
            currentlySelectedElement = element;
            Debug.Log($"Selected element: {element}");
        }
        
        void OnGUI()
        {
            // Display status info
            int y = 70;
            int lineHeight = 18;
            
            GUI.Label(new Rect(10, y, 250, lineHeight), $"Element: {currentlySelectedElement}");
            y += lineHeight;
            GUI.Label(new Rect(10, y, 250, lineHeight), $"Brush Size: {brushSize}");
            y += lineHeight;
            GUI.Label(new Rect(10, y, 250, lineHeight), $"Brush Type: {brushType}");
        }
    }
    
    public enum MouseMode
    {
        SPAWN,
        DELETE,
        HEAT
    }
}

