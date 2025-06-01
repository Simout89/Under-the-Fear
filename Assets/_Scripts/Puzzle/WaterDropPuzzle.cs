using System;
using System.Collections;
using System.Linq;
using _Script.interactive_objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Script.Puzzle
{
    public class WaterDropPuzzle: PuzzleBase, IClickable
    {
        private bool _canPlay = true;
        
        private int[] _passwords = new int[4];

        [SerializeField] private float timeBetweenDigits = 3;
        [SerializeField] private float timeBetweenDrops = 1;
        
        [Header("References")]
        [SerializeField] private CodeLock _codeLock;

        [Header("Sound")]
        [SerializeField] private AK.Wwise.Event waterDrop;

        private void OnEnable()
        {
            _codeLock.onPasswordChaged += HandlePasswordChanged;
        }

        private void OnDisable()
        {
            _codeLock.onPasswordChaged -= HandlePasswordChanged;
        }

        private void HandlePasswordChanged(int[] obj)
        {
            Debug.Log($"{obj[0]} {obj[1]} {obj[2]} {obj[3]} ");
            if (obj.SequenceEqual(_passwords))
            {
                OnPuzzleSolved();
            }
        }

        public override void Restart()
        {
            for (int i = 0; i < _passwords.Length; i++)
            {
                _passwords[i] = Random.Range(1, 9);
            }
        }

        private void Awake()
        {
            Restart();
        }

        public void Click()
        {
            if (_canPlay)
            {
                StartCoroutine(PlayNumber());
            }
        }

        private IEnumerator PlayNumber()
        {
            _canPlay = false;
            yield return new WaitForSeconds((float)0.5);
            foreach (var password in _passwords)
            {
                for (int i = 0; i < password; i++)
                {
                    yield return new WaitForSeconds(timeBetweenDrops);
                    waterDrop.Post(gameObject);
                }

                yield return new WaitForSeconds(timeBetweenDigits);
            }
            _canPlay = true;


            foreach (var VARIABLE in _passwords)
            {
                Debug.Log(VARIABLE);
            }
        }
    }
}