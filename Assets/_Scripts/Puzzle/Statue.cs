using System;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class Statue : MonoBehaviour, IClickable
{
    [Header("Settings")]
    [SerializeField] private float rotateDuration = 2f;
    [Header("Sound")]
    [SerializeField] private AK.Wwise.Event startRotation;
    [SerializeField] private AK.Wwise.Event endRotation;
    private bool _isRotate = false;
    private void Awake()
    {
        Restart();
    }

    private void Restart()
    {
        Random rnd = new Random();
        transform.rotation = Quaternion.Euler(0, rnd.Next(0,4) * 90, 0);
    }

    public void Click()
    {
        if(_isRotate) return;

        startRotation.Post(gameObject);

        _isRotate = true;
        
        var rotation = transform.rotation.eulerAngles;
        
        transform.DORotate(new Vector3(0, rotation.y + 90, 0), rotateDuration).SetEase(Ease.Linear).OnComplete(() => {
            _isRotate = false;
            endRotation.Post(gameObject);
        });
    }

    public int GetRotation()
    {
        return (int)transform.rotation.eulerAngles.y;
    }
}

