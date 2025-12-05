using UnityEngine;
using FallingSand.Core;
using FallingSand.Elements;

namespace FallingSand.UI
{
    public class UIManager : MonoBehaviour
    {
        public CellularAutomaton cellularAutomaton;
        
        private bool showHelp = true;
        private Rect helpWindow;
        private bool helpWindowInitialized = false;
        
        void OnGUI()
        {
            // Initialize help window position on first render (top-right, below Hide Help button)
            if (!helpWindowInitialized)
            {
                helpWindow = new Rect(Screen.width - 420, 50, 400, 620);
                helpWindowInitialized = true;
            }
            
            if (showHelp)
            {
                helpWindow = GUI.Window(0, helpWindow, DrawHelpWindow, "Falling Sand - Controls");
            }
            
            // Toggle help button (top-right)
            if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 30), showHelp ? "Hide Help" : "Show Help"))
            {
                showHelp = !showHelp;
            }
        }
        
        void DrawHelpWindow(int windowID)
        {
            GUILayout.BeginVertical();
            
            GUILayout.Label("=== CONTROLS ===", GUI.skin.box);
            GUILayout.Space(5);
            
            GUILayout.Label("Mouse:");
            GUILayout.Label("  Click - Draw  |  Scroll - Brush Size");
            GUILayout.Space(3);
            
            GUILayout.Label("System:");
            GUILayout.Label("  SPACE - Pause  |  C - Clear");
            GUILayout.Label("  B - Brush Type |  W - Weather");
            GUILayout.Space(5);
            
            GUILayout.Label("=== ELEMENTS ===", GUI.skin.box);
            GUILayout.Space(3);
            
            GUILayout.Label("Number Keys (1-9, 0):");
            GUILayout.Label("  1-Sand     2-Water    3-Stone");
            GUILayout.Label("  4-Wood     5-Lava     6-Gunpowder");
            GUILayout.Label("  7-Oil      8-Acid     9-Snow");
            GUILayout.Label("  0-Eraser");
            GUILayout.Space(3);
            
            GUILayout.Label("Movable Solids (F1-F3):");
            GUILayout.Label("  F1-Dirt    F2-Coal    F3-Ember");
            GUILayout.Space(3);
            
            GUILayout.Label("Immovable Solids (F4-F7):");
            GUILayout.Label("  F4-Brick     F5-Ground");
            GUILayout.Label("  F6-Titanium  F7-SlimeMold");
            GUILayout.Space(3);
            
            GUILayout.Label("Liquids (F8-F9):");
            GUILayout.Label("  F8-Blood   F9-Cement");
            GUILayout.Space(3);
            
            GUILayout.Label("Gases (F10-F12, Q-R):");
            GUILayout.Label("  F10-Steam  F11-Smoke  F12-Spark");
            GUILayout.Label("  Q-ExplosionSpark  E-FlammableGas");
            GUILayout.Label("  R-Gas");
            GUILayout.Space(3);
            
            GUILayout.Label("Special (T):");
            GUILayout.Label("  T-PlayerMeat");
            GUILayout.Space(5);
            
            GUILayout.EndVertical();
            
            GUI.DragWindow();
        }
    }
}

