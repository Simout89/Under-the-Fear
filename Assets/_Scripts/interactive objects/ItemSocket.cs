using System;
using System.Drawing;
using _Script.Manager;
using UnityEngine;
using Zenject;

public class ItemSocket : MonoBehaviour, IStorable, IClickable
{
    [Inject] private GameManager _gameManager;
    public GameObject HoldingItem { set; get; }
    public Transform HoldingPoint { get; set; }
    
    [SerializeField] private Transform holdingPoint;

    private void Awake()
    {
        HoldingPoint = holdingPoint;
    }

    public void Click()
    {
        
    }
}

public interface IStorable
{
    public GameObject HoldingItem { get; set; }
    public Transform HoldingPoint { get; set; }
}
