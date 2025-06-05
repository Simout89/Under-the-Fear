using System;
using System.Collections;
using System.Collections.Generic;
using _Script.interactive_objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class DictaphonePuzzle : PuzzleBase
{
    [Header("Settings")]
    [SerializeField] private float duration = 1;

    [Header("References")]
    [SerializeField] private PressurePlate[] _pressurePlate;
    [Header("Sound")]
    [SerializeField] private AK.Wwise.Event[] numbers;

    [SerializeField] private ButtonTrigger _buttonTrigger;

    private List<int> stayOnPlateOrder = new List<int>();
    private int[] order = new int[6];
    private bool _canPlay = true;

    private void OnEnable()
    {
        for (int i = 0; i < _pressurePlate.Length; i++)
        {
            int index = i;
            _pressurePlate[i].onStay += () => HandleStay(index);
        }
        _buttonTrigger.onClick += HandleClickOnRadio;
    }
    private void OnDisable()
    {
        for (int i = 0; i < _pressurePlate.Length; i++)
        {
            int index = i;
            _pressurePlate[i].onStay -= () => HandleStay(index);
        }
        _buttonTrigger.onClick -= HandleClickOnRadio;
    }

    private void HandleClickOnRadio()
    {
        if (_canPlay)
        {
            StartCoroutine(PlayNumber());
        }
    }

    private void HandleStay(int i)
    {
        Debug.Log(i);
        stayOnPlateOrder.Add(i + 1);
        for (int j = 0; j < stayOnPlateOrder.Count; j++)
        {
            if (stayOnPlateOrder[j] == order[j])
            {
                Debug.Log("Наступил правильно");
            }
            else
            {
                Debug.Log("Наступил не правильно");
                stayOnPlateOrder = new List<int>();
                foreach (var VARIABLE in _pressurePlate)
                {
                    VARIABLE.Restart();
                }
                Fail();
                return;
            }
        }
        _pressurePlate[i].Right();

        if (stayOnPlateOrder.Count == 6)
        {
            OnPuzzleSolved();
            Debug.Log("Головоломка решена");
        }
    }

    private IEnumerator PlayNumber()
    {
        _canPlay = false;
        for (int i = 0; i < order.Length; i++)
        {
            numbers[order[i] - 1].Post(_buttonTrigger.gameObject);
            yield return new WaitForSeconds(duration);
        }
        _canPlay = true;
    }

    private void Awake()
    {
        Restart();
    }

    public override void Restart()
    {
        stayOnPlateOrder = new List<int>();
        order = new int[6];
        List<int> temp = new List<int>();
        for (int i = 1; i < 7; i++)
        {
            temp.Add(i);
        }

        var tempCount = temp.Count;
        for (int i = 0; i < tempCount; i++)
        {
            var tempI = Random.Range(0, temp.Count);
            order[i] = temp[tempI];
            temp.Remove(temp[tempI]);
        }
    }
}
