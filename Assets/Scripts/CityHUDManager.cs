using UnityEngine;
using TMPro;

/// <summary>
/// HUD minimalista para la escena de ciudad.
/// Muestra velocidad y controles. Se crea automaticamente por CitySceneSetup.
/// </summary>
public class CityHUDManager : MonoBehaviour
{
    private CarController car;
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI controlsText;

    private void Start()
    {
        BuildUI();
        StartCoroutine(FindCarDelayed());
    }

    private System.Collections.IEnumerator FindCarDelayed()
    {
        yield return null;
        yield return null;
        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
            car = playerGO.GetComponent<CarController>();
    }

    private void BuildUI()
    {
        Canvas canvas = GetComponent<Canvas>();

        // ── Panel velocidad (abajo derecha) ──────────────────────
        GameObject speedPanel = new GameObject("SpeedPanel");
        speedPanel.transform.SetParent(canvas.transform, false);

        UnityEngine.UI.Image bg = speedPanel.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);

        RectTransform rt = speedPanel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot     = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-20f, 20f);
        rt.sizeDelta = new Vector2(220f, 80f);

        // Texto velocidad
        GameObject stGO = new GameObject("SpeedText");
        stGO.transform.SetParent(speedPanel.transform, false);
        speedText = stGO.AddComponent<TextMeshProUGUI>();
        speedText.text = "0 km/h";
        speedText.fontSize = 32;
        speedText.fontStyle = FontStyles.Bold;
        speedText.color = Color.white;
        speedText.alignment = TextAlignmentOptions.Center;

        RectTransform stRt = stGO.GetComponent<RectTransform>();
        stRt.anchorMin = Vector2.zero;
        stRt.anchorMax = Vector2.one;
        stRt.sizeDelta = Vector2.zero;
        stRt.anchoredPosition = Vector2.zero;

        // ── Panel controles (abajo izquierda) ───────────────────
        GameObject ctrlPanel = new GameObject("ControlsPanel");
        ctrlPanel.transform.SetParent(canvas.transform, false);

        UnityEngine.UI.Image bg2 = ctrlPanel.AddComponent<UnityEngine.UI.Image>();
        bg2.color = new Color(0f, 0f, 0f, 0.45f);

        RectTransform ctRt = ctrlPanel.GetComponent<RectTransform>();
        ctRt.anchorMin = new Vector2(0f, 0f);
        ctRt.anchorMax = new Vector2(0f, 0f);
        ctRt.pivot     = new Vector2(0f, 0f);
        ctRt.anchoredPosition = new Vector2(20f, 20f);
        ctRt.sizeDelta = new Vector2(260f, 90f);

        GameObject ctGO = new GameObject("ControlsText");
        ctGO.transform.SetParent(ctrlPanel.transform, false);
        controlsText = ctGO.AddComponent<TextMeshProUGUI>();
        controlsText.text = "W/S — Acelerar / Frenar\nA/D — Girar\nESC — Salir";
        controlsText.fontSize = 18;
        controlsText.color = new Color(0.85f, 0.85f, 0.85f);
        controlsText.alignment = TextAlignmentOptions.Left;

        RectTransform ctTRt = ctGO.GetComponent<RectTransform>();
        ctTRt.anchorMin = Vector2.zero;
        ctTRt.anchorMax = Vector2.one;
        ctTRt.sizeDelta = new Vector2(-16f, 0f);
        ctTRt.anchoredPosition = new Vector2(8f, 0f);

        // ── Titulo arriba centro ─────────────────────────────────
        GameObject titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI title = titleGO.AddComponent<TextMeshProUGUI>();
        title.text = "🏙  CIUDAD LIBRE";
        title.fontSize = 28;
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(1f, 0.85f, 0.2f);
        title.alignment = TextAlignmentOptions.Center;

        RectTransform titRt = titleGO.GetComponent<RectTransform>();
        titRt.anchorMin = new Vector2(0.3f, 1f);
        titRt.anchorMax = new Vector2(0.7f, 1f);
        titRt.pivot     = new Vector2(0.5f, 1f);
        titRt.anchoredPosition = new Vector2(0f, -15f);
        titRt.sizeDelta = new Vector2(0f, 50f);
    }

    private void Update()
    {
        if (car != null && speedText != null)
        {
            float kmh = car.GetSpeed() * 3.6f;
            speedText.text = Mathf.RoundToInt(kmh) + " km/h";

            // Color segun velocidad
            if (kmh > 120f)       speedText.color = new Color(1f, 0.3f, 0.1f);
            else if (kmh > 60f)   speedText.color = new Color(1f, 0.8f, 0.1f);
            else                  speedText.color = Color.white;
        }

        // ESC para salir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("VehicleSelection");
        }
    }
}
