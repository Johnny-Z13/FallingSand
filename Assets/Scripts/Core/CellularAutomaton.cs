using UnityEngine;
using FallingSand.Elements;
using FallingSand.Rendering;

namespace FallingSand.Core
{
    public class CellularAutomaton : MonoBehaviour
    {
        public static CellularAutomaton Instance { get; private set; }
        public static int frameCount = 0;
        
        [Header("Configuration")]
        public GameConfig config;
        
        [Header("Components")]
        public MatrixRenderer matrixRenderer;
        
        public CellularMatrix matrix;
        private Camera mainCamera;
        
        private bool isPaused = false;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
        void Start()
        {
            // Setup camera
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                mainCamera = camObj.AddComponent<Camera>();
                mainCamera.tag = "MainCamera";
            }
            
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = config.screenHeight / 2f;
            mainCamera.transform.position = new Vector3(config.screenWidth / 2f, config.screenHeight / 2f, -10f);
            
            // Initialize matrix
            matrix = new CellularMatrix(config.screenWidth, config.screenHeight, config.pixelSizeModifier);
            
            // Initialize renderer
            if (matrixRenderer == null)
            {
                matrixRenderer = gameObject.AddComponent<MatrixRenderer>();
            }
            matrixRenderer.Initialize(matrix, config);
            
            // Create initial ground
            SetupBasicGround();
            
            Debug.Log($"Cellular Automaton initialized: {matrix.innerArraySize}x{matrix.outerArraySize} matrix");
        }
        
        void Update()
        {
            // Handle input
            HandleInput();
            
            if (isPaused)
                return;
            
            // Increment frame counter
            frameCount = (frameCount + 1) % 4;
            
            // Reset chunks
            if (matrix.useChunks)
            {
                matrix.ResetChunks();
            }
            
            // Only reshuffle occasionally (not every frame)
            if (frameCount == 0)
            {
                matrix.ReshuffleXIndexes();
            }
            
            // Step all elements
            matrix.StepAll();
            
            // Execute explosions
            matrix.ExecuteExplosions();
            
            // Render matrix
            matrixRenderer.RenderMatrix();
        }
        
        void HandleInput()
        {
            // Pause toggle
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                isPaused = !isPaused;
                Debug.Log($"Simulation {(isPaused ? "paused" : "resumed")}");
            }
            
            // Clear all
            if (UnityEngine.Input.GetKeyDown(KeyCode.C))
            {
                matrix.ClearAll();
                SetupBasicGround();
                Debug.Log("Matrix cleared");
            }
            
            // Note: Mouse input is handled by InputManager component
        }
        
        void SetupBasicGround()
        {
            // Create a simple floor
            int floorY = 20;
            int floorHeight = 10;
            for (int y = 0; y < floorHeight; y++)
            {
                for (int x = 0; x < matrix.innerArraySize; x++)
                {
                    matrix.SpawnElementByMatrix(x, floorY + y, ElementType.STONE);
                }
            }
        }
        
        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        void OnGUI()
        {
            // Simple debug info
            GUI.Label(new Rect(10, 10, 200, 20), $"FPS: {(int)(1f / Time.smoothDeltaTime)}");
            GUI.Label(new Rect(10, 30, 200, 20), $"Paused: {isPaused}");
            GUI.Label(new Rect(10, 50, 200, 20), $"Matrix: {matrix.innerArraySize}x{matrix.outerArraySize}");
        }
    }
}

