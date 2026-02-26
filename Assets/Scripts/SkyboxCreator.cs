using UnityEngine;
using UnityEditor;

public class SkyboxCreator : MonoBehaviour
{
    public static Material CreateGradientSkybox()
    {
        // Create procedural skybox material
        Material skyboxMat = new Material(Shader.Find("Skybox/Procedural"));
        
        // Beautiful gradient colors
        skyboxMat.SetColor("_SkyTint", new Color(0.2f, 0.4f, 0.8f)); // Blue
        skyboxMat.SetColor("_GroundColor", new Color(0.3f, 0.2f, 0.4f)); // Purple
        skyboxMat.SetFloat("_SunSize", 0.04f);
        skyboxMat.SetFloat("_SunSizeConvergence", 5f);
        skyboxMat.SetFloat("_AtmosphereThickness", 1.5f);
        skyboxMat.SetFloat("_Exposure", 1.3f);
        
        return skyboxMat;
    }
}
