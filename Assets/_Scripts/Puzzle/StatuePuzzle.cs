using System;
using UnityEngine;

public class StatuePuzzle : PuzzleBase, IClickable
{
    [SerializeField] private StatuePuzzleSocket[] _statuePuzzleSocket;
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
