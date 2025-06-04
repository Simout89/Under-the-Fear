using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class TriggerSceneLoader : MonoBehaviour
{
    [Inject] private SceneLoaderManager _sceneLoaderManager;

    private void OnTriggerEnter(Collider other)
    {
        _sceneLoaderManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
