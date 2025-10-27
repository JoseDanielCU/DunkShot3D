using UnityEngine;
using System.IO;

public class BallPresetApplier : MonoBehaviour
{
    public Material neonGlowMaterial;
    public Material trailMaterial;

    private void Start()
    {
        // Cargar último preset si existe
        if (PlayerPrefs.HasKey("LastPreset"))
        {
            string presetName = PlayerPrefs.GetString("LastPreset");
            string path = Path.Combine(Application.persistentDataPath, $"{presetName}.json");

            if (File.Exists(path))
            {
                BallPreset preset = JsonUtility.FromJson<BallPreset>(File.ReadAllText(path));
                ApplyPreset(preset);
                Debug.Log($"🎨 Preset aplicado automáticamente: {presetName}");
            }
            else
            {
                Debug.LogWarning($"⚠️ No se encontró el archivo del preset: {presetName}");
            }
        }
    }

    private void ApplyPreset(BallPreset preset)
    {
        if (neonGlowMaterial != null)
        {
            neonGlowMaterial.SetColor("_BaseColor", preset.baseColor);
            neonGlowMaterial.SetColor("_RimColor", preset.rimColor);
            neonGlowMaterial.SetFloat("_RimPower", preset.rimPower);
            neonGlowMaterial.SetFloat("_RimIntensity", preset.rimIntensity);
            neonGlowMaterial.SetFloat("_RimThickness", preset.rimThickness);
        }

        if (trailMaterial != null)
        {
            trailMaterial.SetColor("_BaseColor", preset.trailBaseColor);
            trailMaterial.SetColor("_EmissionColor", preset.trailEmissionColor);
            trailMaterial.SetFloat("_EmissionIntensity", preset.trailEmissionIntensity);
            trailMaterial.SetFloat("_Fade", preset.trailFade);
        }
    }
}
