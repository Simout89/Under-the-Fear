using UnityEngine;

public class Collectible : MonoBehaviour, IClickable, IPickupable
{
    [SerializeField] private AK.Wwise.Event throwSound;
    [SerializeField] private AK.Wwise.Event pickupSound;
    public void Click()
    {
        
    }

    public void PlayThrowSound()
    {
        throwSound.Post(Camera.main.gameObject);
    }

    public void PlayPickupSound()
    {
            pickupSound.Post(Camera.main.gameObject);
    }
}

public interface IPickupable
{
    public void PlayThrowSound();
    public void PlayPickupSound();
}