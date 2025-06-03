using UnityEngine;

public class WwiseEventTriggerZoneChance : MonoBehaviour
{
    public string eventName = "Play_Sound"; // Название события из Wwise
    public GameObject eventTarget;          // Объект, от имени которого звучит событие
    public float cooldownTime = 5f;         // Время кулдауна между активациями (в секундах)
    public float activationChance = 50f;    // Шанс активации события в процентах (0-100)

    private bool hasActivated = false;
    private float lastActivationTime = -Mathf.Infinity;

    void Start()
    {
        if (eventTarget == null)
            eventTarget = gameObject; // Если не указано — использовать сам объект
    }

    void Update()
    {
        // Если прошло достаточно времени, сбрасываем флаг активации
        if (Time.time - lastActivationTime >= cooldownTime)
        {
            hasActivated = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastActivationTime < cooldownTime) return; // Проверка на кулдаун

            // Проверяем шанс активации
            float randomValue = Random.Range(0f, 100f);
            if (randomValue > activationChance) return; // Если случайное число больше шанса, выходим

            if (!hasActivated)
            {
                AkSoundEngine.PostEvent(eventName, eventTarget);
                Debug.Log($"Wwise event '{eventName}' triggered from {eventTarget.name}");
                hasActivated = true;
                lastActivationTime = Time.time; // Запоминаем время активации
            }
        }
    }
}
