using System;
using UnityEngine;

public class Lever : MonoBehaviour, IClickable, ISwitchable
{
    public event Action<bool> OnSwitchState;
    public bool Status { get; private set; }

    [Header("References")]
    [SerializeField] private Transform handlePivot;

    private Quaternion startRotation;

    private void Awake()
    {
        startRotation = handlePivot.localRotation;
    }

    public void Click()
    {
        Status = !Status;
        OnSwitchState?.Invoke(Status);
        if (Status)
        {
            handlePivot.localRotation = Quaternion.Inverse(startRotation);
        }
        else
        {
            handlePivot.localRotation = startRotation;
        }
    }
}

public interface ISwitchable
{
    public event Action<bool> OnSwitchState;
    public bool Status { get; }
}

public interface IClickable
{
    public void Click();
}
