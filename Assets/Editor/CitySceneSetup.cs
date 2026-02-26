using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Crea automaticamente una escena de ciudad grande para explorar con el coche.
/// Menu: Roll a Ball > Create City Exploration Scene
/// </summary>
public class CitySceneSetup : EditorWindow
{
    [MenuItem("Roll a Ball/Create City Exploration Scene")]
    public static void ShowWindow()
    {
        CitySceneSetup w = GetWindow<CitySceneSetup>("City Setup");
        w.minSize = new Vector2(380, 200);
        w.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Escena Ciudad - Exploracion Libre", EditorStyles.boldLabel);
        GUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "Crea una nueva escena con:\n" +
            "- Mapa de ciudad grande (400x400)\n" +
            "- Calles en cuadricula con edificios\n" +
            "- Rampas para saltar\n" +
            "- Rectas largas y curvas\n" +
            "- Obstaculos por la ciudad\n" +
            "- McLaren P1 listo para conducir",
            MessageType.Info);

        GUILayout.Space(15);
        GUI.backgroundColor = new Color(1f, 0.6f, 0.1f);
        if (GUILayout.Button("CREAR ESCENA CIUDAD", GUILayout.Height(45)))
            CreateCityScene();
        GUI.backgroundColor = Color.white;
    }

    private void CreateCityScene()
    {
        // Crear nueva escena
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        SetupLighting();
        SetupSkybox();
        CreateGround();
        CreateCityGrid();
        CreateRamps();
        CreateLongStraights();
        CreateObstacles();
        CreateBoundaryWalls();
        SetupCamera();
        CreateCarPlayer();
        CreateMiniMap();

        // Guardar escena
        EnsureFolder("Assets/Scenes");
        string path = "Assets/Scenes/CityExploration.unity";
        EditorSceneManager.SaveScene(scene, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Ciudad creada",
            "Escena 'CityExploration' creada en Assets/Scenes/\n\n" +
            "IMPORTANTE:\n" +
            "1. Anade la escena al Build Settings\n" +
            "2. En el menu VehicleSelection, anade un boton 'CIUDAD'\n" +
            "   o dale Play directamente desde CityExploration\n\n" +
            "Controles: W/S acelerar, A/D girar", "OK");
    }

    // ───────────────────────────────────────────────────────────────
    //  ILUMINACION Y SKYBOX
    // ───────────────────────────────────────────────────────────────

    private void SetupLighting()
    {
        GameObject lightGO = new GameObject("Directional Light");
        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(55f, -30f, 0f);
        light.intensity = 1.4f;
        light.color = new Color(1f, 0.96f, 0.88f);
        light.shadows = LightShadows.Soft;
        light.shadowResolution = UnityEngine.Rendering.LightShadowResolution.High;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor    = new Color(0.5f, 0.65f, 0.9f);
        RenderSettings.ambientEquatorColor = new Color(0.4f, 0.45f, 0.5f);
        RenderSettings.ambientGroundColor  = new Color(0.2f, 0.2f, 0.25f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.6f, 0.7f, 0.85f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 80f;
        RenderSettings.fogEndDistance = 300f;
    }

    private void SetupSkybox()
    {
        Material sky = new Material(Shader.Find("Skybox/Procedural"));
        if (sky != null)
        {
            sky.SetFloat("_SunSize", 0.04f);
            sky.SetFloat("_AtmosphereThickness", 1f);
            sky.SetColor("_SkyTint", new Color(0.5f, 0.65f, 1f));
            sky.SetColor("_GroundColor", new Color(0.37f, 0.35f, 0.35f));
            sky.SetFloat("_Exposure", 1.2f);
            EnsureFolder("Assets/Materials");
            AssetDatabase.CreateAsset(sky, "Assets/Materials/CitySkybox.mat");
            RenderSettings.skybox = sky;
        }
    }

    // ───────────────────────────────────────────────────────────────
    //  SUELO
    // ───────────────────────────────────────────────────────────────

    private void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(40f, 1f, 40f); // 400x400 unidades

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.25f, 0.25f, 0.28f); // asfalto oscuro
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Glossiness", 0.1f);
        EnsureFolder("Assets/Materials");
        AssetDatabase.CreateAsset(mat, "Assets/Materials/GroundMat.mat");
        ground.GetComponent<Renderer>().material = mat;
    }

    // ───────────────────────────────────────────────────────────────
    //  CUADRICULA DE CIUDAD
    // ───────────────────────────────────────────────────────────────

    private void CreateCityGrid()
    {
        GameObject cityRoot = new GameObject("City");

        Material roadMat   = CreateMaterial("RoadMat",     new Color(0.18f, 0.18f, 0.2f),  0f, 0.05f);
        Material buildMat1 = CreateMaterial("BuildMat1",   new Color(0.55f, 0.55f, 0.6f),  0.3f, 0.4f);
        Material buildMat2 = CreateMaterial("BuildMat2",   new Color(0.7f,  0.65f, 0.5f),  0.2f, 0.3f);
        Material buildMat3 = CreateMaterial("BuildMat3",   new Color(0.3f,  0.45f, 0.6f),  0.5f, 0.6f);
        Material[] buildMats = { buildMat1, buildMat2, buildMat3 };

        // Bloques de ciudad: rejilla de 7x7
        int blocks = 7;
        float blockSize = 50f;   // tamaño de cada manzana
        float streetWidth = 12f;
        float totalCell = blockSize + streetWidth;
        float offset = -(blocks / 2f) * totalCell;

        for (int x = 0; x < blocks; x++)
        {
            for (int z = 0; z < blocks; z++)
            {
                float cx = offset + x * totalCell + blockSize / 2f;
                float cz = offset + z * totalCell + blockSize / 2f;

                // Calle horizontal
                CreateStreet(cityRoot, roadMat,
                    new Vector3(cx, 0.05f, offset + z * totalCell - streetWidth / 2f),
                    new Vector3(blockSize, 0.1f, streetWidth));

                // Calle vertical
                CreateStreet(cityRoot, roadMat,
                    new Vector3(offset + x * totalCell - streetWidth / 2f, 0.05f, cz),
                    new Vector3(streetWidth, 0.1f, blockSize));

                // Edificios dentro de la manzana (4-8 por manzana)
                int buildCount = Random.Range(3, 7);
                for (int b = 0; b < buildCount; b++)
                {
                    float bx = cx + Random.Range(-blockSize * 0.35f, blockSize * 0.35f);
                    float bz = cz + Random.Range(-blockSize * 0.35f, blockSize * 0.35f);
                    float bw = Random.Range(6f, 16f);
                    float bh = Random.Range(8f, 45f);
                    float bd = Random.Range(6f, 16f);
                    Material bm = buildMats[Random.Range(0, buildMats.Length)];
                    CreateBuilding(cityRoot, bm, new Vector3(bx, bh / 2f, bz), new Vector3(bw, bh, bd));
                }
            }
        }
    }

    private void CreateStreet(GameObject parent, Material mat, Vector3 center, Vector3 size)
    {
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "Street";
        s.transform.SetParent(parent.transform);
        s.transform.position = center;
        s.transform.localScale = size;
        s.GetComponent<Renderer>().material = mat;
    }

    private void CreateBuilding(GameObject parent, Material mat, Vector3 center, Vector3 size)
    {
        GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.name = "Building";
        b.transform.SetParent(parent.transform);
        b.transform.position = center;
        b.transform.localScale = size;
        b.GetComponent<Renderer>().material = mat;
    }

    // ───────────────────────────────────────────────────────────────
    //  RAMPAS
    // ───────────────────────────────────────────────────────────────

    private void CreateRamps()
    {
        GameObject rampRoot = new GameObject("Ramps");
        Material rampMat = CreateMaterial("RampMat", new Color(0.8f, 0.5f, 0.1f), 0.2f, 0.5f);

        // 6 rampas distribuidas por la ciudad
        Vector3[] rampPositions = {
            new Vector3(60,  0, 30),
            new Vector3(-70, 0, 60),
            new Vector3(30,  0, -80),
            new Vector3(-40, 0, -50),
            new Vector3(100, 0, 100),
            new Vector3(-100,0, -80),
        };
        float[] rampAngles = { 0, 45, 90, 135, 20, 70 };

        for (int i = 0; i < rampPositions.Length; i++)
        {
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Ramp_" + i;
            ramp.transform.SetParent(rampRoot.transform);
            ramp.transform.position = rampPositions[i] + new Vector3(0, 1.5f, 0);
            ramp.transform.localScale = new Vector3(8f, 0.4f, 14f);
            ramp.transform.rotation = Quaternion.Euler(-15f, rampAngles[i], 0f);
            ramp.GetComponent<Renderer>().material = rampMat;

            // Soporte debajo de la rampa
            GameObject support = GameObject.CreatePrimitive(PrimitiveType.Cube);
            support.name = "RampSupport";
            support.transform.SetParent(rampRoot.transform);
            support.transform.position = rampPositions[i] + new Vector3(0, 0.5f, 3f);
            support.transform.localScale = new Vector3(8f, 1f, 5f);
            support.GetComponent<Renderer>().material = rampMat;
        }
    }

    // ───────────────────────────────────────────────────────────────
    //  RECTAS LARGAS Y CURVAS
    // ───────────────────────────────────────────────────────────────

    private void CreateLongStraights()
    {
        GameObject straightRoot = new GameObject("Straights_and_Curves");
        Material trackMat = CreateMaterial("TrackMat", new Color(0.15f, 0.15f, 0.18f), 0f, 0.1f);
        Material lineMat  = CreateMaterial("LineMat",  new Color(1f, 0.85f, 0f),        0f, 0.3f);

        // Recta central norte-sur (larga)
        CreateTrackSegment(straightRoot, trackMat, lineMat,
            new Vector3(0, 0.06f, 0), new Vector3(14f, 0.12f, 300f), 0f);

        // Recta este-oeste
        CreateTrackSegment(straightRoot, trackMat, lineMat,
            new Vector3(0, 0.06f, 80f), new Vector3(280f, 0.12f, 14f), 0f);

        // Curva elevada en esquina (simulada con segmentos)
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 11.25f;
            float rad = angle * Mathf.Deg2Rad;
            float r = 55f;
            float cx = 120f + Mathf.Cos(rad) * r;
            float cz = 120f + Mathf.Sin(rad) * r;

            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seg.name = "CurveSegment_" + i;
            seg.transform.SetParent(straightRoot.transform);
            seg.transform.position = new Vector3(cx, 0.06f, cz);
            seg.transform.localScale = new Vector3(14f, 0.12f, 16f);
            seg.transform.rotation = Quaternion.Euler(0, angle, 0);
            seg.GetComponent<Renderer>().material = trackMat;
        }
    }

    private void CreateTrackSegment(GameObject parent, Material trackMat, Material lineMat,
                                    Vector3 pos, Vector3 size, float rotY)
    {
        GameObject track = GameObject.CreatePrimitive(PrimitiveType.Cube);
        track.name = "Track";
        track.transform.SetParent(parent.transform);
        track.transform.position = pos;
        track.transform.localScale = size;
        track.transform.rotation = Quaternion.Euler(0, rotY, 0);
        track.GetComponent<Renderer>().material = trackMat;

        // Linea central
        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line.name = "CenterLine";
        line.transform.SetParent(parent.transform);
        line.transform.position = pos + Vector3.up * 0.07f;
        line.transform.localScale = new Vector3(0.4f, 0.05f, size.z * 0.95f);
        line.transform.rotation = Quaternion.Euler(0, rotY, 0);
        line.GetComponent<Renderer>().material = lineMat;
        DestroyImmediate(line.GetComponent<BoxCollider>());
    }

    // ───────────────────────────────────────────────────────────────
    //  OBSTACULOS
    // ───────────────────────────────────────────────────────────────

    private void CreateObstacles()
    {
        GameObject obsRoot = new GameObject("Obstacles");
        Material coneMat   = CreateMaterial("ConeMat",   new Color(1f, 0.3f, 0f),  0f, 0.4f);
        Material barrierMat= CreateMaterial("BarrierMat",new Color(0.9f, 0.9f, 0.9f), 0.1f, 0.3f);

        // Conos (cilindros naranjas) dispersos por calles
        for (int i = 0; i < 30; i++)
        {
            float rx = Random.Range(-160f, 160f);
            float rz = Random.Range(-160f, 160f);
            GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cone.name = "Cone_" + i;
            cone.transform.SetParent(obsRoot.transform);
            cone.transform.position = new Vector3(rx, 0.6f, rz);
            cone.transform.localScale = new Vector3(0.5f, 0.6f, 0.5f);
            cone.GetComponent<Renderer>().material = coneMat;
        }

        // Barreras (cajas blancas) bloqueando parcialmente calles
        Vector3[] barrierPos = {
            new Vector3(25, 0.5f, 0),   new Vector3(-25, 0.5f, 40),
            new Vector3(0, 0.5f, -60),  new Vector3(50, 0.5f, 80),
            new Vector3(-80, 0.5f, 20), new Vector3(70, 0.5f, -40),
            new Vector3(-50, 0.5f, -90),new Vector3(90, 0.5f, 60),
        };
        float[] barrierRots = { 0, 45, 90, 30, 60, 15, 75, 0 };

        for (int i = 0; i < barrierPos.Length; i++)
        {
            GameObject barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrier.name = "Barrier_" + i;
            barrier.transform.SetParent(obsRoot.transform);
            barrier.transform.position = barrierPos[i];
            barrier.transform.localScale = new Vector3(6f, 1f, 0.5f);
            barrier.transform.rotation = Quaternion.Euler(0, barrierRots[i], 0);
            barrier.GetComponent<Renderer>().material = barrierMat;
        }
    }

    // ───────────────────────────────────────────────────────────────
    //  MUROS PERIMETRALES
    // ───────────────────────────────────────────────────────────────

    private void CreateBoundaryWalls()
    {
        Material wallMat = CreateMaterial("WallMat", new Color(0.6f, 0.6f, 0.65f), 0.2f, 0.3f);
        float size = 200f;
        float h    = 5f;
        float t    = 2f;

        CreateWall("Wall_N", wallMat, new Vector3(0, h/2f,  size), new Vector3(size*2f+t, h, t));
        CreateWall("Wall_S", wallMat, new Vector3(0, h/2f, -size), new Vector3(size*2f+t, h, t));
        CreateWall("Wall_E", wallMat, new Vector3( size, h/2f, 0), new Vector3(t, h, size*2f));
        CreateWall("Wall_W", wallMat, new Vector3(-size, h/2f, 0), new Vector3(t, h, size*2f));
    }

    private void CreateWall(string name, Material mat, Vector3 pos, Vector3 scale)
    {
        GameObject w = GameObject.CreatePrimitive(PrimitiveType.Cube);
        w.name = name;
        w.transform.position = pos;
        w.transform.localScale = scale;
        w.GetComponent<Renderer>().material = mat;
    }

    // ───────────────────────────────────────────────────────────────
    //  CAMARA
    // ───────────────────────────────────────────────────────────────

    private void SetupCamera()
    {
        GameObject camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.fieldOfView = 75f;
        cam.farClipPlane = 400f;
        camGO.transform.position = new Vector3(0, 6f, -12f);
        camGO.transform.rotation = Quaternion.Euler(15f, 0f, 0f);

        CameraController cc = camGO.AddComponent<CameraController>();
        cc.offset = new Vector3(0, 6f, -12f);
        cc.smoothSpeed = 0.08f;
        cc.lookAtPlayer = true;

        // AudioListener
        camGO.AddComponent<AudioListener>();
    }

    // ───────────────────────────────────────────────────────────────
    //  COCHE JUGADOR
    // ───────────────────────────────────────────────────────────────

    private void CreateCarPlayer()
    {
        // Intentar cargar prefab existente
        GameObject carPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/McLarenP1_Player.prefab");

        GameObject player;
        if (carPrefab != null)
        {
            player = (GameObject)PrefabUtility.InstantiatePrefab(carPrefab);
            player.transform.position = new Vector3(0, 1f, -150f);
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Placeholder si no existe el prefab
            player = new GameObject("McLarenP1_Player");
            player.tag = "Player";
            player.transform.position = new Vector3(0, 1f, -150f);

            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.mass = 1.5f; rb.drag = 4f; rb.angularDrag = 8f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            BoxCollider col = player.AddComponent<BoxCollider>();
            col.size = new Vector3(1.8f, 0.8f, 4f);
            col.center = new Vector3(0, 0.4f, 0);

            player.AddComponent<CarController>();

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(player.transform);
            body.transform.localPosition = new Vector3(0, 0.4f, 0);
            body.transform.localScale = new Vector3(1.8f, 0.6f, 4f);
            DestroyImmediate(body.GetComponent<BoxCollider>());
            Material bm = CreateMaterial("CarBodyMat", new Color(0.8f, 0.1f, 0.1f), 0.8f, 0.9f);
            body.GetComponent<Renderer>().material = bm;

            Debug.LogWarning("[CitySetup] Prefab McLarenP1_Player no encontrado. Usando placeholder. " +
                             "Ejecuta primero 'Setup Vehicle Selection System'.");
        }

        // Asignar player a la camara
        CameraController cc = Object.FindObjectOfType<CameraController>();
        if (cc != null) cc.player = player.transform;

        // UI simple para ciudad (solo velocidad)
        CreateCityHUD();
    }

    // ───────────────────────────────────────────────────────────────
    //  HUD MINIMALISTA
    // ───────────────────────────────────────────────────────────────

    private void CreateCityHUD()
    {
        GameObject canvasGO = new GameObject("CityHUD");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // EventSystem
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Añadir CityHUDManager para mostrar velocidad
        canvasGO.AddComponent<CityHUDManager>();
    }

    // ───────────────────────────────────────────────────────────────
    //  MINIMAPA (indicador de posicion)
    // ───────────────────────────────────────────────────────────────

    private void CreateMiniMap()
    {
        // Camara de minimapa
        GameObject miniCamGO = new GameObject("MinimapCamera");
        Camera miniCam = miniCamGO.AddComponent<Camera>();
        miniCam.orthographic = true;
        miniCam.orthographicSize = 120f;
        miniCam.transform.position = new Vector3(0, 200f, 0);
        miniCam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        miniCam.farClipPlane = 250f;
        miniCam.depth = -1;

        // RenderTexture para el minimapa
        RenderTexture rt = new RenderTexture(256, 256, 16);
        rt.name = "MinimapRT";
        AssetDatabase.CreateAsset(rt, "Assets/Materials/MinimapRT.renderTexture");
        miniCam.targetTexture = rt;
    }

    // ───────────────────────────────────────────────────────────────
    //  HELPERS
    // ───────────────────────────────────────────────────────────────

    private Material CreateMaterial(string name, Color color, float metallic, float gloss)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Glossiness", gloss);
        EnsureFolder("Assets/Materials");
        string path = "Assets/Materials/" + name + ".mat";
        // Evitar duplicados
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
