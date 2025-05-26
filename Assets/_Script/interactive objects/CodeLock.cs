using System;
using UnityEngine;

namespace _Script.interactive_objects
{
    public class CodeLock: MonoBehaviour
    {
        public event Action<int[]> onPasswordChaged;
        
        private int[] password = {1,1,1,1};
        [SerializeField] private Transform[] lockSegments;
        [SerializeField] private ButtonTrigger[] buttonTrigger;

        private void OnEnable()
        {
            for (int i = 0; i < buttonTrigger.Length; i++)
            {
                int index = i;
                buttonTrigger[i].onClick += () => HandleButtonClick(index);
            }
        }
        
        private void OnDisable()
        {
            for (int i = 0; i < buttonTrigger.Length; i++)
            {
                int index = i;
                buttonTrigger[i].onClick -= () => HandleButtonClick(index);
            }
        }

        private void HandleButtonClick(int index)
        {
            var rotation = lockSegments[index].localRotation.eulerAngles;
            lockSegments[index].localRotation = Quaternion.Euler(rotation + new Vector3(0, 0, 45));
            password[index]++;
            if (password[index] > 8)
                password[index] = 1;
            onPasswordChaged?.Invoke(password);
        }
    }
}