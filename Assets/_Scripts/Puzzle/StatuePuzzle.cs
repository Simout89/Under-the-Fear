using System;
using _Script.interactive_objects;
using UnityEngine;

public class StatuePuzzle : PuzzleBase, IClickable
{
    [SerializeField] private StatuePuzzleSocket[] _statuePuzzleSocket;
    [SerializeField] private ButtonTrigger _buttonTrigger;

    private void OnEnable()
    {
        _buttonTrigger.onClick += HandleClick;
    }
    private void OnDisable()
    {
        _buttonTrigger.onClick -= HandleClick;
    }

    private void HandleClick()
    {
        Click();
    }

    public void Click()
    {
        foreach (var statue in _statuePuzzleSocket)
        {
            if(statue.Statue.GetRotation() != statue.RightY)
            {
                Debug.Log("Не решено");
                Fail();
                return;
            }
        }
        Debug.Log("Решено");
        
        OnPuzzleSolved();
    }
}

[Serializable]
public class StatuePuzzleSocket
{
    public Statue Statue;
    public int RightY;
}
