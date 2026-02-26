using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class VehicleSystemSetup : EditorWindow
{
    [MenuItem("Roll a Ball/Setup Vehicle Selection System")]
    public static void ShowWindow()
    {
        VehicleSystemSetup window = GetWindow<VehicleSystemSetup>("Vehicle Setup");
        window.minSize = new Vector2(420, 260);
        window.Show();
    }

    private string gameSceneName = "Rolabal";

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Sistema de Seleccion de Vehiculos", EditorStyles.boldLabel);
        GUILayout.Space(5);

        EditorGUILayout.HelpBox(
            "Configura automaticamente:\n" +
            "- Prefab McLaren P1 (CarController)\n" +
            "- Prefab Bola (PlayerController)\n" +
            "- PlayerSpawner en la escena\n" +
            "- Escena de seleccion de vehiculo",
            MessageType.Info);

        GUILayout.Space(10);
        gameSceneName = EditorGUILayout.TextField("Nombre escena del juego:", gameSceneName);

        GUILayout.Space(15);

        GUI.backgroundColor = new Color(0.3f, 0.9f, 0.3f);
        if (GUILayout.Button("CONFIGURAR SISTEMA COMPLETO", GUILayout.Height(45)))
        {
            RunSetup();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(8);

        if (GUILayout.Button("Solo crear Prefab del Coche", GUILayout.Height(30)))
        {
            CreateCarPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("OK", "Prefab del coche creado en Assets/Prefabs/", "OK");
        }
    }

    private void RunSetup()
    {
        bool ok = EditorUtility.DisplayDialog("Configurar Sistema",
            "Se crearan los prefabs y la escena de seleccion.\n\nContinuar?",
            "Si", "Cancelar");
        if (!ok) return;

        GameObject carPrefab  = CreateCarPrefab();
        GameObject ballPrefab = CreateBallPrefab();
        CreateSelectionScene();
        AddPlayerSpawner(carPrefab, ballPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Completado",
            "Sistema configurado!\n\n" +
            "PASOS FINALES:\n" +
            "1. File > Build Settings\n" +
            "2. Anade VehicleSelection como index 0\n" +
            "3. Tu escena del juego como index 1\n" +
            "4. Elimina el Player fijo de la jerarquia\n" +
            "5. Dale Play desde VehicleSelection", "OK");
    }

    private GameObject CreateCarPrefab()
    {
        EnsureFolder("Assets/Prefabs");

        GameObject car = new GameObject("McLarenP1_Player");
        car.tag = "Player";

        GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CarModel/mclaren_p1.c4d.fbx");
        if (fbx != null)
        {
            GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
            model.transform.SetParent(car.transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale    = new Vector3(0.015f, 0.015f, 0.015f);
        }
        else
        {
            Debug.LogWarning("[VehicleSetup] FBX no encontrado. Usando placeholder.");
            CreatePlaceholder(car);
        }

        Rigidbody rb = car.AddComponent<Rigidbody>();
        rb.mass        = 1.5f;
        rb.drag        = 2f;
        rb.angularDrag = 5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.centerOfMass = new Vector3(0, -0.4f, 0);

        BoxCollider col = car.AddComponent<BoxCollider>();
        col.size   = new Vector3(1.8f, 0.8f, 4f);
        col.center = new Vector3(0, 0.4f, 0);

        car.AddComponent<CarController>();

        string path = "Assets/Prefabs/McLarenP1_Player.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(car, path);
        DestroyImmediate(car);

        Debug.Log("[VehicleSetup] Prefab coche creado: " + path);
        return prefab;
    }

    private void CreatePlaceholder(GameObject parent)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(1f, 0.35f, 0.05f);
        mat.SetFloat("_Metallic", 0.8f);
        mat.SetFloat("_Glossiness", 0.9f);

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(parent.transform);
        body.transform.localPosition = new Vector3(0, 0.4f, 0);
        body.transform.localScale    = new Vector3(1.8f, 0.6f, 4f);
        body.GetComponent<Renderer>().material = mat;
        DestroyImmediate(body.GetComponent<BoxCollider>());

        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.name = "Roof";
        roof.transform.SetParent(parent.transform);
        roof.transform.localPosition = new Vector3(0, 0.95f, -0.2f);
        roof.transform.localScale    = new Vector3(1.5f, 0.5f, 2f);
        roof.GetComponent<Renderer>().material = mat;
        DestroyImmediate(roof.GetComponent<BoxCollider>());

        Material wheelMat = new Material(Shader.Find("Standard"));
        wheelMat.color = Color.black;

        Vector3[] wheelPositions = {
            new Vector3(-0.9f, 0.2f,  1.3f),
            new Vector3( 0.9f, 0.2f,  1.3f),
            new Vector3(-0.9f, 0.2f, -1.3f),
            new Vector3( 0.9f, 0.2f, -1.3f)
        };
        foreach (Vector3 pos in wheelPositions)
        {
            GameObject w = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            w.name = "Wheel";
            w.transform.SetParent(parent.transform);
            w.transform.localPosition = pos;
            w.transform.localScale    = new Vector3(0.4f, 0.15f, 0.4f);
            w.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            w.GetComponent<Renderer>().material = wheelMat;
            DestroyImmediate(w.GetComponent<CapsuleCollider>());
        }
    }

    private GameObject CreateBallPrefab()
    {
        string path = "Assets/Prefabs/Ball_Player.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        EnsureFolder("Assets/Prefabs");

        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball_Player";
        ball.tag  = "Player";
        ball.transform.position = new Vector3(0, 0.5f, 0);

        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass        = 1f;
        rb.drag        = 0f;
        rb.angularDrag = 0.5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        ball.AddComponent<PlayerController>();

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0f, 0.8f, 1f);
        mat.SetFloat("_Metallic",    0.7f);
        mat.SetFloat("_Glossiness",  0.9f);
        ball.GetComponent<Renderer>().material = mat;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(ball, path);
        DestroyImmediate(ball);

        Debug.Log("[VehicleSetup] Prefab bola creado: " + path);
        return prefab;
    }

    private void CreateSelectionScene()
    {
        EnsureFolder("Assets/Scenes");

        string path = "Assets/Scenes/VehicleSelection.unity";
        if (System.IO.File.Exists(path))
        {
            Debug.Log("[VehicleSetup] La escena VehicleSelection ya existe.");
            return;
        }

        Scene sel = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);

        GameObject vs = new GameObject("VehicleSelector");
        SceneManager.MoveGameObjectToScene(vs, sel);
        vs.AddComponent<VehicleSelector>();

        GameObject menu = new GameObject("VehicleSelectionMenu");
        SceneManager.MoveGameObjectToScene(menu, sel);
        VehicleSelectionMenu vm = menu.AddComponent<VehicleSelectionMenu>();
        vm.gameSceneName = gameSceneName;

        EditorSceneManager.SaveScene(sel, path);
        EditorSceneManager.CloseScene(sel, true);

        Debug.Log("[VehicleSetup] Escena creada: " + path);
    }

    private void AddPlayerSpawner(GameObject carPrefab, GameObject ballPrefab)
    {
        PlayerSpawner existing = Object.FindObjectOfType<PlayerSpawner>();
        if (existing != null)
        {
            existing.carPrefab  = carPrefab;
            existing.ballPrefab = ballPrefab;
            EditorUtility.SetDirty(existing);
            Debug.Log("[VehicleSetup] PlayerSpawner actualizado.");
            return;
        }

        GameObject go = new GameObject("PlayerSpawner");
        PlayerSpawner ps = go.AddComponent<PlayerSpawner>();
        ps.carPrefab      = carPrefab;
        ps.ballPrefab     = ballPrefab;
        ps.spawnPosition  = new Vector3(0, 0.5f, 0);

        EditorUtility.SetDirty(go);
        Debug.Log("[VehicleSetup] PlayerSpawner creado en la escena.");
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
