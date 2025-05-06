using UnityEngine;

public class RTPCZoneTriggerCooldown : MonoBehaviour
{
    [Header("RTPC Settings")]
    public float intensityStep = 25f;
    public MusicRTPCController musicController;

    [Header("Cooldown")]
    public float cooldownTime = 5f; // Время кулдауна
    private bool isOnCooldown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isOnCooldown && other.CompareTag("Player") && musicController != null)
        {
            musicController.IncreaseIntensity(intensityStep);
            Debug.Log($"RTPC Intensity increased by {intensityStep}");
            StartCoroutine(CooldownRoutine());
        }
    }

    private System.Collections.IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        Debug.Log("RTPC trigger cooldown started...");
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        Debug.Log("RTPC trigger reactivated.");
    }
}
