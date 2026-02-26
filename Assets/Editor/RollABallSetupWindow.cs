using UnityEngine;
using UnityEditor;

// INSTRUCCIONES SIMPLES:
// 1. En Unity, ve al menú superior: Window → Roll a Ball Setup
// 2. Haz clic en el botón grande "CONFIGURAR ESCENA"

public class RollABallSetupWindow : EditorWindow
{
    [MenuItem("Window/Roll a Ball Setup")]
    public static void ShowWindow()
    {
        GetWindow<RollABallSetupWindow>("Setup Roll a Ball");
    }

    void OnGUI()
    {
        GUILayout.Label("Configuración Automática de Escena", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("Este script configurará automáticamente:");
        GUILayout.Label("✓ Player con física");
        GUILayout.Label("✓ Cámara con seguimiento");
        GUILayout.Label("✓ Corredor infinito");
        GUILayout.Label("✓ Sistema de obstáculos");
        GUILayout.Label("✓ UI completa");
        GUILayout.Label("✓ Materiales y luces");
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("CONFIGURAR ESCENA", GUILayout.Height(50)))
        {
            RollABallSceneSetup.SetupScene();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Después de configurar, presiona Play ▶️", EditorStyles.helpBox);
        GUILayout.Label("Controles: A/D o Flechas ←→", EditorStyles.helpBox);
    }
}
