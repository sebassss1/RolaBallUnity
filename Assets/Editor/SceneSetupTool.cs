using UnityEngine;
using UnityEditor;

// INSTRUCCIONES:
// 1. Selecciona este script en el Project
// 2. Mira el Inspector (panel derecho)
// 3. Haz clic en el botón "CONFIGURAR ESCENA AUTOMÁTICAMENTE"

[CreateAssetMenu(fileName = "SceneSetupTool", menuName = "Roll a Ball/Scene Setup Tool")]
public class SceneSetupTool : ScriptableObject
{
    [ContextMenu("CONFIGURAR ESCENA AUTOMÁTICAMENTE")]
    public void SetupScene()
    {
        RollABallSceneSetup.SetupScene();
    }
}
