using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject point;

    public void ShowPoint() => point.SetActive(true);
    public void HidePoint() => point.SetActive(false);
}
