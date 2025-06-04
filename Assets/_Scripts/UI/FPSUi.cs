using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text fpsText;
    
    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.5f;
    
    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    
    private void Start()
    {
        timeleft = updateInterval;
    }
    
    private void Update()
    {
        // Используем unscaledDeltaTime для игнорирования timeScale
        timeleft -= Time.unscaledDeltaTime;
        accum += 1.0f / Time.unscaledDeltaTime;
        frames++;
        
        if (timeleft <= 0.0)
        {
            float fps = accum / frames;
            
            if (fpsText != null)
            {
                fpsText.text = $"{fps:F1}";
                
                // Цветовая индикация производительности
                if (fps >= 60)
                    fpsText.color = Color.green;
                else if (fps >= 30)
                    fpsText.color = Color.yellow;
                else
                    fpsText.color = Color.red;
            }
            
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}