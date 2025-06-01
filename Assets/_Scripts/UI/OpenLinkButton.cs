using UnityEngine;
using UnityEngine.InputSystem.HID;

public class OpenLinkButton : MonoBehaviour
{
    public void OpenUrl(string link)
    {
        Application.OpenURL(link);
    }
}
