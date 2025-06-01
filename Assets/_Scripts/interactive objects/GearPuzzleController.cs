using System;
using UnityEngine;

public class GearPuzzleController : PuzzleBase, IClickable
{
    [SerializeField] private GearPuzzleSocket[] _gearPuzzleSockets;

    public void TryActivate()
    {
        foreach (var gearPuzzleSocket in _gearPuzzleSockets)
        {
            if(gearPuzzleSocket.ItemSocket.HoldingItem == null || gearPuzzleSocket.ItemSocket.HoldingItem.name != gearPuzzleSocket.Name)
            {
                Debug.Log("Не Решено");
                Fail();
                return;
            }
        }
        
        Debug.Log("Решено");
        OnPuzzleSolved();
    }

    public void Click()
    {
        TryActivate();
    }
}

[Serializable]
public class GearPuzzleSocket
{
    [SerializeField] public string Name;
    [SerializeField] public ItemSocket ItemSocket;
}
