using UnityEngine;

public class RTPCZoneTriggerOnce : MonoBehaviour  //код для зоны которая срабатывает 1 раз
{
    public float intensityStep = 25f;
    public MusicRTPCController musicController;

    private bool hasActivated = false; //устанавливает что зона ещё не была активирована

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Player") && musicController != null)
        {
            musicController.IncreaseIntensity(intensityStep);
            hasActivated = true; //устанавливает что зона была активирована
        }
    }
}
