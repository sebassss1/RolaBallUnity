using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Pantalla de selección de vehículo que aparece al dar Play.
/// Muestra dos opciones: Bola o Coche. Al elegir, carga la escena del juego.
/// 
/// CÓMO USAR:
/// 1. Crea una nueva escena llamada "VehicleSelection" (o el nombre que prefieras)
/// 2. En esa escena, crea un Canvas y añade este script a un GameObject vacío
/// 3. Asigna el Canvas en el Inspector
/// 4. En Build Settings, pon la escena de selección primero (index 0)
///    y la escena del juego segunda (index 1)
/// </summary>
public class VehicleSelectionMenu : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre exacto de la escena del juego a cargar")]
    public string gameSceneName = "SampleScene";

    [Header("Canvas UI (se crea automáticamente si no se asigna)")]
    public Canvas canvas;

    private GameObject _ui;

    private void Start()
    {
        // Aseguramos que el singleton VehicleSelector exista
        if (VehicleSelector.Instance == null)
        {
            GameObject vs = new GameObject("VehicleSelector");
            vs.AddComponent<VehicleSelector>();
        }

        BuildUI();
    }

    private void BuildUI()
    {
        // ── Canvas ──────────────────────────────────────────────
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("SelectionCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();

            // EventSystem
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        // ── Fondo oscuro ─────────────────────────────────────────
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvas.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.05f, 0.05f, 0.1f, 1f);
        RectTransform bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;

        // ── Título ────────────────────────────────────────────────
        CreateLabel(canvas.transform, "ELIGE TU VEHÍCULO", new Vector2(0, 200), 60);

        // ── Botón BOLA ────────────────────────────────────────────
        GameObject ballBtn = CreateButton(canvas.transform, "🔵  BOLA", new Vector2(-220, 0),
            new Color(0f, 0.6f, 1f), new Vector2(340, 160));
        ballBtn.GetComponent<Button>().onClick.AddListener(() => SelectAndPlay(VehicleType.Ball));

        CreateLabel(ballBtn.transform, "Control clásico\nRoll a Ball", new Vector2(0, -50), 22,
            new Color(0.9f, 0.9f, 0.9f));

        // ── Botón COCHE ───────────────────────────────────────────
        GameObject carBtn = CreateButton(canvas.transform, "🚗  COCHE", new Vector2(220, 0),
            new Color(1f, 0.3f, 0.1f), new Vector2(340, 160));
        carBtn.GetComponent<Button>().onClick.AddListener(() => SelectAndPlay(VehicleType.Car));

        CreateLabel(carBtn.transform, "McLaren P1\nFísicas de coche", new Vector2(0, -50), 22,
            new Color(0.9f, 0.9f, 0.9f));

        // ── Instrucción inferior ──────────────────────────────────
        CreateLabel(canvas.transform, "Selecciona un vehículo para comenzar", new Vector2(0, -250), 28,
            new Color(0.7f, 0.7f, 0.7f));
    }

    private void SelectAndPlay(VehicleType type)
    {
        VehicleSelector.Instance.SelectedVehicle = type;
        SceneManager.LoadScene(gameSceneName);
    }

    // ─── Helpers de UI ───────────────────────────────────────────

    private GameObject CreateButton(Transform parent, string label, Vector2 pos,
                                    Color color, Vector2 size)
    {
        GameObject btn = new GameObject("Btn_" + label);
        btn.transform.SetParent(parent, false);

        RectTransform rt = btn.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = btn.AddComponent<Image>();
        img.color = color;

        Button b = btn.AddComponent<Button>();
        ColorBlock cb = b.colors;
        cb.highlightedColor = color * 1.3f;
        cb.pressedColor = color * 0.7f;
        b.colors = cb;

        // Texto principal del botón
        GameObject textGO = new GameObject("Label");
        textGO.transform.SetParent(btn.transform, false);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        RectTransform trt = textGO.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0, 0.4f);
        trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero;
        trt.anchoredPosition = Vector2.zero;

        return btn;
    }

    private GameObject CreateLabel(Transform parent, string text, Vector2 pos,
                                   float fontSize, Color? color = null)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(900, 80);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color ?? Color.white;
        tmp.fontStyle = FontStyles.Bold;

        return go;
    }
}
