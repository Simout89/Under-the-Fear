using System;
using System.Collections.Generic;
using _Script.Sound;
using UnityEngine;

namespace _Script.Manager
{
    public class AudioManager: MonoBehaviour
    {
        [SerializeField] private AudioVolumeFader[] _audioVolumeFaders;

        private void Awake()
        {
            foreach (var audioVolumeFader in _audioVolumeFaders)
            {
                audioVolumeFader.Init(this, 2f);
            }
        }
    }
}