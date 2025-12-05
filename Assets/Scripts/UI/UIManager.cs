using UnityEngine;
using FallingSand.Core;
using FallingSand.Elements;

namespace FallingSand.UI
{
    public class UIManager : MonoBehaviour
    {
        public CellularAutomaton cellularAutomaton;
        
        private bool showHelp = true;
        private Rect helpWindow = new Rect(10, 10, 280, 400);
        
        void OnGUI()
        {
            if (showHelp)
            {
                helpWindow = GUI.Window(0, helpWindow, DrawHelpWindow, "Falling Sand - Controls");
            }
            
            // Toggle help
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
            GUILayout.Label("  Click - Draw");
            GUILayout.Label("  Scroll - Brush Size");
            GUILayout.Space(3);
            
            GUILayout.Label("Keyboard:");
            GUILayout.Label("  SPACE - Pause");
            GUILayout.Label("  C - Clear");
            GUILayout.Label("  B - Brush Type");
            GUILayout.Label("  W - Weather");
            GUILayout.Space(3);
            
            GUILayout.Label("Elements (1-9, 0):");
            GUILayout.Label("  1-Sand  2-Water  3-Stone");
            GUILayout.Label("  4-Wood  5-Lava   6-Powder");
            GUILayout.Label("  7-Oil   8-Acid   9-Snow");
            GUILayout.Label("  0-Eraser");
            GUILayout.Space(5);
            
            GUILayout.EndVertical();
            
            GUI.DragWindow();
        }
    }
}

