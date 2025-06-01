using System;
using UnityEngine;

namespace _Script.interactive_objects
{
    public class ButtonTrigger: MonoBehaviour, IClickable
    {
        public event Action onClick;
        public void Click()
        {
            onClick?.Invoke();
        }
    }
}