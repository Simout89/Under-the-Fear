using UnityEngine;
using Zenject;

public class SceneLoader : MonoBehaviour
{
    [Inject] private SceneLoaderManager _sceneLoaderManager;
    public void LoadScene(int index)
    {
        _sceneLoaderManager.LoadScene(index);
    }
}
