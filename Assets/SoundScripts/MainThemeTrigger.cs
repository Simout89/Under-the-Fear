using UnityEngine;

public class MainThemeTrigger : MonoBehaviour
{
    public MusicRTPCController musicController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && musicController != null)
        {
            musicController.PlayMusic();
        }
    }
}
