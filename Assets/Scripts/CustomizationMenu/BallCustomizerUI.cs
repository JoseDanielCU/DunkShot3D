using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallCustomizerUI : MonoBehaviour
{
    [Header("Materials")]
    public Material neonGlowMaterial;
    public Material trailMaterial;

    [Header("Preview")]
    public Transform previewBall;
    public float rotationSpeed = 50f;

    [Header("Color Pickers")]
    public FlexibleColorPicker basePicker;
    public FlexibleColorPicker rimPicker;
    public FlexibleColorPicker trailBasePicker;
    public FlexibleColorPicker trailEmissionPicker;

    [Header("Sliders")]
    public Slider rimPower;
    public Slider rimIntensity;
    public Slider rimThickness;
    public Slider trailEmissionIntensity;
    public Slider trailFade;

    [Header("Preset System")]
    public TMP_InputField presetNameInput;
    public GameObject presetListPanel;
    public Transform presetListContent;
    public GameObject presetButtonPrefab;
    public TextMeshProUGUI feedbackText;

    [Header("Preset Save Popup")]
    public GameObject presetSavePanel;
    public TMP_InputField saveNameInput;

    private void Start()
    {
        if (presetListPanel != null)
            presetListPanel.SetActive(false);

        if (presetSavePanel != null)
            presetSavePanel.SetActive(false);

        // 🔹 Habilitar modo preview para el trail
        if (previewBall != null)
        {
            BallTrailController trailCtrl = previewBall.GetComponent<BallTrailController>();
            if (trailCtrl != null)
                trailCtrl.isPreview = true;
        }

        // 🔹 Intentar cargar el último preset guardado
        if (PlayerPrefs.HasKey("LastPreset"))
        {
            string lastPreset = PlayerPrefs.GetString("LastPreset");
            if (!string.IsNullOrEmpty(lastPreset))
                LoadPreset(lastPreset);
        }
    }



    private void Update()
    {
        if (previewBall != null)
            previewBall.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        UpdateNeonGlow();
        UpdateTrail();
    }

    // --- SHADER UPDATES ---
    public void UpdateNeonGlow()
    {
        if (neonGlowMaterial == null) return;
        neonGlowMaterial.SetColor("_BaseColor", basePicker.color);
        neonGlowMaterial.SetColor("_RimColor", rimPicker.color);
        neonGlowMaterial.SetFloat("_RimPower", rimPower.value);
        neonGlowMaterial.SetFloat("_RimIntensity", rimIntensity.value);
        neonGlowMaterial.SetFloat("_RimThickness", rimThickness.value);
    }

    public void UpdateTrail()
    {
        if (trailMaterial == null) return;
        trailMaterial.SetColor("_BaseColor", trailBasePicker.color);
        trailMaterial.SetColor("_EmissionColor", trailEmissionPicker.color);
        trailMaterial.SetFloat("_EmissionIntensity", trailEmissionIntensity.value);
        trailMaterial.SetFloat("_Fade", trailFade.value);
        BallPreviewTrail previewTrail = previewBall.GetComponentInChildren<BallPreviewTrail>();
        if (previewTrail != null)
            previewTrail.SetTrailMaterial(trailMaterial);
    }

    // --- SAVE / LOAD SYSTEM ---
    // Abre la ventana para escribir el nombre
    public void OpenSavePresetPanel()
    {
        if (presetSavePanel != null)
        {
            saveNameInput.text = ""; // Limpia el campo
            presetSavePanel.SetActive(true);
        }
    }

    // Guarda el preset con el nombre del campo
    public void ConfirmSavePreset()
    {
        string name = saveNameInput.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            ShowMessage("Ingresa un nombre válido.");
            return;
        }

        BallPreset preset = new BallPreset
        {
            baseColor = basePicker.color,
            rimColor = rimPicker.color,
            rimPower = rimPower.value,
            rimIntensity = rimIntensity.value,
            rimThickness = rimThickness.value,
            trailBaseColor = trailBasePicker.color,
            trailEmissionColor = trailEmissionPicker.color,
            trailEmissionIntensity = trailEmissionIntensity.value,
            trailFade = trailFade.value
        };

        string path = GetPresetPath(name);
        File.WriteAllText(path, JsonUtility.ToJson(preset, true));

        ShowMessage($"✅ Preset guardado: {name}");
        presetSavePanel.SetActive(false);
    }
    public void CancelSavePreset()
    {
        if (presetListPanel != null)
            presetListPanel.SetActive(false);

        if (presetSavePanel != null)
            presetSavePanel.SetActive(false);
    }


    public void LoadPreset(string name)
    {
        string path = GetPresetPath(name);
        if (!File.Exists(path))
        {
            ShowMessage("❌ Preset no encontrado.");
            return;
        }

        BallPreset preset = JsonUtility.FromJson<BallPreset>(File.ReadAllText(path));

        basePicker.color = preset.baseColor;
        rimPicker.color = preset.rimColor;
        trailBasePicker.color = preset.trailBaseColor;
        trailEmissionPicker.color = preset.trailEmissionColor;

        rimPower.value = preset.rimPower;
        rimIntensity.value = preset.rimIntensity;
        rimThickness.value = preset.rimThickness;
        trailEmissionIntensity.value = preset.trailEmissionIntensity;
        trailFade.value = preset.trailFade;

        UpdateNeonGlow();
        UpdateTrail();

        presetNameInput.text = name;

        // 🔹 Guardar el último preset en PlayerPrefs
        PlayerPrefs.SetString("LastPreset", name);
        PlayerPrefs.Save();

        ShowMessage($"🎨 Preset cargado: {name}");
    }


    public void CopyPresetName()
    {
        GUIUtility.systemCopyBuffer = presetNameInput.text;
        ShowMessage("📋 Nombre copiado al portapapeles");
    }

    // --- LISTA DE PRESETS ---
    public void OpenPresetList()
    {
        foreach (Transform child in presetListContent)
            Destroy(child.gameObject);

        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
        if (files.Length == 0)
        {
            ShowMessage("No hay presets guardados.");
            return;
        }

        foreach (string file in files)
        {
            string presetName = Path.GetFileNameWithoutExtension(file);
            GameObject btn = Instantiate(presetButtonPrefab, presetListContent);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = presetName;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                LoadPreset(presetName);
                presetListPanel.SetActive(false);
            });
        }

        presetListPanel.SetActive(true);
    }

    public void ClosePresetList()
    {
        presetListPanel.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-2);
    }
    // --- HELPERS ---
    private string GetPresetPath(string name)
    {
        return Path.Combine(Application.persistentDataPath, $"{name}.json");
    }

    private void ShowMessage(string msg)
    {
        if (feedbackText != null)
        {
            feedbackText.text = msg;
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 2f);
        }
        Debug.Log(msg);
    }

    private void ClearMessage()
    {
        feedbackText.text = "";
    }
}

// --- DATA CLASS ---
[System.Serializable]
public class BallPreset
{
    public Color baseColor;
    public Color rimColor;
    public float rimPower;
    public float rimIntensity;
    public float rimThickness;
    public Color trailBaseColor;
    public Color trailEmissionColor;
    public float trailEmissionIntensity;
    public float trailFade;
}
