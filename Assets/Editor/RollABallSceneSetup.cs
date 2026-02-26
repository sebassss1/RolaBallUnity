using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class RollABallSceneSetup : EditorWindow
{
    [MenuItem("Roll a Ball/Auto Setup Scene")]
    public static void SetupScene()
    {
        if (EditorUtility.DisplayDialog("Auto Setup Scene", 
            "Esto configurará automáticamente toda la escena del juego Roll a Ball.\n\n" +
            "¿Continuar?", "Sí", "Cancelar"))
        {
            CreateScene();
            EditorUtility.DisplayDialog("Completado", 
                "¡Escena configurada correctamente!\n\n" +
                "Presiona Play para jugar.\n" +
                "Controles: A/D o Flechas ←→", "OK");
        }
    }

    private static void CreateScene()
    {
        // Set quality settings for best visuals
        QualitySettings.SetQualityLevel(5, true); // Ultra quality
        QualitySettings.antiAliasing = 8; // 8x MSAA
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
        QualitySettings.shadowDistance = 150f;
        QualitySettings.shadowCascades = 4;
        QualitySettings.vSyncCount = 1; // Enable VSync
        
        // Create required tags
        CreateTag("Obstacle");
        CreateTag("Player");
        
        // Store references to camera and light before cleanup
        Camera mainCamera = Camera.main;
        Light directionalLight = null;
        Light[] lights = Light.GetLights(LightType.Directional, 0);
        if (lights.Length > 0)
        {
            directionalLight = lights[0];
        }
        
        // Clear existing objects (except camera and light)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // Skip camera and light
            if (mainCamera != null && obj == mainCamera.gameObject) continue;
            if (directionalLight != null && obj == directionalLight.gameObject) continue;
            
            // Delete everything else
            if (obj.name != "Main Camera" && obj.name != "Directional Light")
            {
                DestroyImmediate(obj);
            }
        }
        
        // Ensure we have a camera
        if (Camera.main == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<Camera>();
            Debug.Log("✅ Created Main Camera");
        }
        
        // Ensure we have a directional light
        if (Light.GetLights(LightType.Directional, 0).Length == 0)
        {
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            Debug.Log("✅ Created Directional Light");
        }

        // Create folders
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");

        // 1. Create Player
        GameObject player = CreatePlayer();

        // 2. Setup Camera
        SetupCamera(player);

        // 3. Create Prefabs
        GameObject corridorPrefab = CreateCorridorPrefab();
        GameObject obstaclePrefab = CreateObstaclePrefab();

        // 4. Create Managers
        GameObject gameManager = CreateGameManager(player);
        CreateCorridorManager(player, corridorPrefab);
        CreateObstacleSpawner(obstaclePrefab);

        // 5. Create UI
        GameObject canvas = CreateUI(gameManager);

        // 6. Create Materials
        CreateMaterials(player, corridorPrefab, obstaclePrefab);

        // 7. Setup Lighting and Skybox
        SetupLighting();
        SetupSkybox();

        // Save
        EditorUtility.SetDirty(player);
        AssetDatabase.SaveAssets();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("✅ Escena configurada correctamente!");
    }
    
    private static void CreateTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        // Check if tag already exists
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
            {
                found = true;
                break;
            }
        }
        
        // Add tag if not found
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            tagsProp.GetArrayElementAtIndex(0).stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }
    }

    private static GameObject CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.transform.position = new Vector3(0, 0.5f, 0);
        player.tag = "Player";

        // Add Rigidbody
        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.drag = 0f;
        rb.angularDrag = 0.5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Add PlayerController
        PlayerController pc = player.AddComponent<PlayerController>();

        return player;
    }

    private static void SetupCamera(GameObject player)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 5, -8);
            mainCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
            
            // Improved camera settings
            mainCamera.clearFlags = CameraClearFlags.Skybox;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            mainCamera.fieldOfView = 70f;

            // Add CameraController
            CameraController cc = mainCamera.gameObject.AddComponent<CameraController>();
            
            // DIRECT assignment with proper saving
            cc.player = player.transform;
            cc.offset = new Vector3(0, 5, -8);
            cc.smoothSpeed = 0.1f;
            cc.lookAtPlayer = true;
            
            // CRITICAL: Mark as dirty to save changes
            EditorUtility.SetDirty(cc);
            EditorUtility.SetDirty(mainCamera.gameObject);
            
            // Add camera shake
            CameraShake shake = mainCamera.gameObject.AddComponent<CameraShake>();
            EditorUtility.SetDirty(shake);
            
            // Add trail effect to player
            TrailEffect trail = player.AddComponent<TrailEffect>();
            EditorUtility.SetDirty(trail);
            
            Debug.Log($"✅ Camera configured to follow player: {player.name}");
            Debug.Log($"✅ Player Transform assigned: {cc.player != null}");
        }
        else
        {
            Debug.LogError("❌ Main Camera not found!");
        }
    }

    private static GameObject CreateCorridorPrefab()
    {
        GameObject corridor = new GameObject("CorridorSegment");

        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.parent = corridor.transform;
        floor.transform.localPosition = new Vector3(0, -0.5f, 10);
        floor.transform.localScale = new Vector3(10, 1, 20);

        // Left Wall
        GameObject wallLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallLeft.name = "WallLeft";
        wallLeft.transform.parent = corridor.transform;
        wallLeft.transform.localPosition = new Vector3(-5, 1, 10);
        wallLeft.transform.localScale = new Vector3(1, 3, 20);

        // Right Wall
        GameObject wallRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallRight.name = "WallRight";
        wallRight.transform.parent = corridor.transform;
        wallRight.transform.localPosition = new Vector3(5, 1, 10);
        wallRight.transform.localScale = new Vector3(1, 3, 20);

        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(corridor, "Assets/Prefabs/CorridorSegment.prefab");
        DestroyImmediate(corridor);

        return prefab;
    }

    private static GameObject CreateObstaclePrefab()
    {
        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.name = "Obstacle";
        obstacle.transform.position = new Vector3(0, 0.5f, 0);
        obstacle.tag = "Obstacle";

        // Add Rigidbody
        Rigidbody rb = obstacle.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // Add Obstacle script
        obstacle.AddComponent<Obstacle>();

        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obstacle, "Assets/Prefabs/Obstacle.prefab");
        DestroyImmediate(obstacle);

        return prefab;
    }

    private static GameObject CreateGameManager(GameObject player)
    {
        GameObject gm = new GameObject("GameManager");
        GameManager gameManager = gm.AddComponent<GameManager>();

        SerializedObject so = new SerializedObject(gameManager);
        so.FindProperty("player").objectReferenceValue = player.GetComponent<PlayerController>();
        so.FindProperty("difficultyIncreaseRate").floatValue = 1f;
        so.ApplyModifiedProperties();

        return gm;
    }

    private static void CreateCorridorManager(GameObject player, GameObject corridorPrefab)
    {
        GameObject cm = new GameObject("CorridorManager");
        CorridorManager corridorManager = cm.AddComponent<CorridorManager>();

        SerializedObject so = new SerializedObject(corridorManager);
        so.FindProperty("corridorSegmentPrefab").objectReferenceValue = corridorPrefab;
        so.FindProperty("segmentsAhead").intValue = 5;
        so.FindProperty("segmentLength").floatValue = 20f;
        so.FindProperty("player").objectReferenceValue = player.transform;
        so.ApplyModifiedProperties();
    }

    private static void CreateObstacleSpawner(GameObject obstaclePrefab)
    {
        GameObject os = new GameObject("ObstacleSpawner");
        ObstacleSpawner spawner = os.AddComponent<ObstacleSpawner>();

        SerializedObject so = new SerializedObject(spawner);
        so.FindProperty("obstaclePrefab").objectReferenceValue = obstaclePrefab;
        so.FindProperty("spawnDistance").floatValue = 50f;
        so.FindProperty("baseSpawnInterval").floatValue = 2f;
        so.FindProperty("minSpawnInterval").floatValue = 0.5f;
        so.FindProperty("spawnRangeX").floatValue = 3f;
        so.FindProperty("spawnHeight").floatValue = 0.5f;
        so.FindProperty("minScale").vector3Value = new Vector3(0.5f, 0.5f, 0.5f);
        so.FindProperty("maxScale").vector3Value = new Vector3(1.5f, 2f, 1.5f);
        so.ApplyModifiedProperties();
    }

    private static GameObject CreateUI(GameObject gameManager)
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // Event System
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // HUD Panel
        GameObject hudPanel = CreateHUDPanel(canvasObj);

        // Game Over Panel
        GameObject gameOverPanel = CreateGameOverPanel(canvasObj);
        gameOverPanel.SetActive(false);

        // Add UIManager
        UIManager uiManager = canvasObj.AddComponent<UIManager>();
        
        SerializedObject so = new SerializedObject(uiManager);
        so.FindProperty("scoreText").objectReferenceValue = hudPanel.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        so.FindProperty("distanceText").objectReferenceValue = hudPanel.transform.Find("DistanceText").GetComponent<TextMeshProUGUI>();
        so.FindProperty("timeText").objectReferenceValue = hudPanel.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
        so.FindProperty("speedText").objectReferenceValue = hudPanel.transform.Find("SpeedText").GetComponent<TextMeshProUGUI>();
        so.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
        so.FindProperty("finalScoreText").objectReferenceValue = gameOverPanel.transform.Find("FinalScoreText").GetComponent<TextMeshProUGUI>();
        so.FindProperty("finalDistanceText").objectReferenceValue = gameOverPanel.transform.Find("FinalDistanceText").GetComponent<TextMeshProUGUI>();
        so.FindProperty("finalTimeText").objectReferenceValue = gameOverPanel.transform.Find("FinalTimeText").GetComponent<TextMeshProUGUI>();
        so.FindProperty("restartButton").objectReferenceValue = gameOverPanel.transform.Find("RestartButton").GetComponent<Button>();
        so.ApplyModifiedProperties();

        // Connect to GameManager
        SerializedObject gmSo = new SerializedObject(gameManager.GetComponent<GameManager>());
        gmSo.FindProperty("uiManager").objectReferenceValue = uiManager;
        gmSo.ApplyModifiedProperties();

        return canvasObj;
    }

    private static GameObject CreateHUDPanel(GameObject canvas)
    {
        GameObject panel = new GameObject("HUD");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);

        // Score Text
        CreateText(panel, "ScoreText", new Vector2(20, -20), TextAnchor.UpperLeft, "PUNTUACIÓN: 0", 36);
        CreateText(panel, "DistanceText", new Vector2(20, -70), TextAnchor.UpperLeft, "DISTANCIA: 0m", 36);
        CreateText(panel, "TimeText", new Vector2(20, -120), TextAnchor.UpperLeft, "TIEMPO: 00:00", 36);
        CreateText(panel, "SpeedText", new Vector2(-20, -20), TextAnchor.UpperRight, "VELOCIDAD: 0", 36);

        return panel;
    }

    private static GameObject CreateGameOverPanel(GameObject canvas)
    {
        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.78f);

        // Title
        GameObject title = CreateText(panel, "GameOverTitle", new Vector2(0, -150), TextAnchor.MiddleCenter, "GAME OVER", 72);
        title.GetComponent<TextMeshProUGUI>().color = Color.red;

        // Stats
        GameObject finalScore = CreateText(panel, "FinalScoreText", new Vector2(0, -300), TextAnchor.MiddleCenter, "PUNTUACIÓN FINAL: 0", 48);
        finalScore.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        
        CreateText(panel, "FinalDistanceText", new Vector2(0, -370), TextAnchor.MiddleCenter, "DISTANCIA: 0 metros", 48);
        CreateText(panel, "FinalTimeText", new Vector2(0, -440), TextAnchor.MiddleCenter, "TIEMPO: 00:00", 48);

        // Restart Button
        GameObject buttonObj = new GameObject("RestartButton");
        buttonObj.transform.SetParent(panel.transform, false);
        
        RectTransform buttonRt = buttonObj.AddComponent<RectTransform>();
        buttonRt.anchoredPosition = new Vector2(0, -550);
        buttonRt.sizeDelta = new Vector2(300, 80);
        
        Image buttonImg = buttonObj.AddComponent<Image>();
        buttonImg.color = Color.green;
        
        Button button = buttonObj.AddComponent<Button>();
        
        GameObject buttonText = new GameObject("Text");
        buttonText.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI tmp = buttonText.AddComponent<TextMeshProUGUI>();
        tmp.text = "REINICIAR";
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        RectTransform textRt = buttonText.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        return panel;
    }

    private static GameObject CreateText(GameObject parent, string name, Vector2 position, TextAnchor anchor, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(500, 50);
        
        if (anchor == TextAnchor.UpperLeft)
        {
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
        }
        else if (anchor == TextAnchor.UpperRight)
        {
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
        }
        else
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        
        // Better colors with high contrast
        if (name.Contains("Score") || name.Contains("Distance") || name.Contains("Time") || name.Contains("Speed"))
        {
            // HUD text - bright cyan with black outline
            tmp.color = new Color(0f, 1f, 1f); // Cyan
            tmp.outlineColor = Color.black;
            tmp.outlineWidth = 0.3f;
        }
        else
        {
            // Game Over text - keep white but add outline
            tmp.color = Color.white;
            tmp.outlineColor = Color.black;
            tmp.outlineWidth = 0.2f;
        }
        
        tmp.fontStyle = FontStyles.Bold;
        
        // Add shadow for extra visibility
        var shadow = textObj.AddComponent<UnityEngine.UI.Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.8f);
        shadow.effectDistance = new Vector2(2, -2);
        
        if (anchor == TextAnchor.UpperRight)
            tmp.alignment = TextAlignmentOptions.Right;
        else if (anchor == TextAnchor.MiddleCenter)
            tmp.alignment = TextAlignmentOptions.Center;
        else
            tmp.alignment = TextAlignmentOptions.Left;
        
        return textObj;
    }

    private static void CreateMaterials(GameObject player, GameObject corridorPrefab, GameObject obstaclePrefab)
    {
        // Player Material - Bright cyan with glow
        Material playerMat = new Material(Shader.Find("Standard"));
        playerMat.color = new Color(0f, 0.8f, 1f); // Bright cyan
        playerMat.SetFloat("_Metallic", 0.7f);
        playerMat.SetFloat("_Glossiness", 0.9f);
        playerMat.EnableKeyword("_EMISSION");
        playerMat.SetColor("_EmissionColor", new Color(0f, 0.6f, 1f) * 0.5f);
        AssetDatabase.CreateAsset(playerMat, "Assets/Materials/PlayerMaterial.mat");
        player.GetComponent<Renderer>().material = playerMat;

        // Corridor Material - Dark with subtle blue tint
        Material corridorMat = new Material(Shader.Find("Standard"));
        corridorMat.color = new Color(0.15f, 0.15f, 0.2f); // Dark blue-grey
        corridorMat.SetFloat("_Metallic", 0.4f);
        corridorMat.SetFloat("_Glossiness", 0.7f);
        AssetDatabase.CreateAsset(corridorMat, "Assets/Materials/CorridorMaterial.mat");

        // Obstacle Material - Bright orange with glow
        Material obstacleMat = new Material(Shader.Find("Standard"));
        obstacleMat.color = new Color(1f, 0.4f, 0f); // Bright orange
        obstacleMat.SetFloat("_Metallic", 0.3f);
        obstacleMat.SetFloat("_Glossiness", 0.8f);
        obstacleMat.EnableKeyword("_EMISSION");
        obstacleMat.SetColor("_EmissionColor", new Color(1f, 0.3f, 0f) * 0.4f);
        AssetDatabase.CreateAsset(obstacleMat, "Assets/Materials/ObstacleMaterial.mat");
    }

    private static void SetupLighting()
    {
        Light dirLight = Light.GetLights(LightType.Directional, 0)[0];
        if (dirLight != null)
        {
            dirLight.transform.rotation = Quaternion.Euler(50, -30, 0);
            dirLight.intensity = 1.8f;
            dirLight.color = new Color(1f, 0.95f, 0.9f);
            dirLight.shadows = LightShadows.Soft;
        }

        // Enhanced ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.4f, 0.5f, 0.7f);
        RenderSettings.ambientEquatorColor = new Color(0.3f, 0.3f, 0.4f);
        RenderSettings.ambientGroundColor = new Color(0.2f, 0.2f, 0.3f);
        RenderSettings.ambientIntensity = 1.2f;
        
        // Add fog for depth
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.3f, 0.4f, 0.6f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 30f;
        RenderSettings.fogEndDistance = 100f;
    }
    
    private static void SetupSkybox()
    {
        // Create beautiful gradient skybox
        Material skyboxMat = SkyboxCreator.CreateGradientSkybox();
        AssetDatabase.CreateAsset(skyboxMat, "Assets/Materials/GradientSkybox.mat");
        
        // Apply skybox
        RenderSettings.skybox = skyboxMat;
        
        // Update skybox in scene
        DynamicGI.UpdateEnvironment();
    }
}
