using System;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class Statue : MonoBehaviour, IClickable
{
    private bool _isRotate = false;
    private void Awake()
    {
        Random rnd = new Random();
        transform.rotation = Quaternion.Euler(0, rnd.Next(0,4) * 90, 0);
    }

    public void Click()
    {
        if(_isRotate) return;

        _isRotate = true;
        
        var rotation = transform.rotation.eulerAngles;
        
        transform.DORotate(new Vector3(0, rotation.y + 90, 0), 1f).OnComplete(() => _isRotate = false);
    }

    public int GetRotation()
    {
        return (int)transform.rotation.eulerAngles.y;
    }
}

