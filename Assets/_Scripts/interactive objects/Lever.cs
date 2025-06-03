using System;
using DG.Tweening;
using UnityEngine;

public class Lever : MonoBehaviour, IClickable, ISwitchable
{
    public event Action<bool> OnSwitchState;
    public bool Status { get; private set; }
    private bool _isRotate;

    [Header("Settings")]
    [SerializeField] private float duration = 2;
    
    [Header("References")]
    [SerializeField] private Transform handlePivot;

    [Header("Sound")]
    [SerializeField] private AK.Wwise.Event sound;

    private Quaternion startRotation;

    private void Awake()
    {
        startRotation = handlePivot.localRotation;
    }

    public void Click()
    {
        Status = !Status;
        OnSwitchState?.Invoke(Status);
        if (Status && !_isRotate)
        {
            _isRotate = true;
            sound.Post(gameObject);
            handlePivot.DOLocalRotate(Quaternion.Inverse(startRotation).eulerAngles, duration).OnComplete(() => _isRotate = false);

        }
        else
        {
            _isRotate = true;
            sound.Post(gameObject);
            handlePivot.DOLocalRotate(startRotation.eulerAngles, duration).OnComplete(() => _isRotate = false);
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
