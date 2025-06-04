using System;
using System.Collections;
using _Script.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class SceneLoaderManager : MonoBehaviour
{
    [Inject] private GameStateManager _gameStateManager;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingSlider;

    public event Action OnSceneLoaded;
    public void LoadScene(int index)
    {   
        SceneManager.LoadScene(index);
        StartCoroutine(LoadingScreenOnFade(index));
        _gameStateManager.ChangeState(index == 0 ? GameState.MainMenu : GameState.Play);
    }

    private IEnumerator LoadingScreenOnFade(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        loadingScreen.SetActive(true);
        loadingSlider.gameObject.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = progress;
            yield return null;
        }
        loadingSlider.value = 1f;
        yield return new WaitForSeconds(0.5f);
        loadingSlider.gameObject.SetActive(false);
        loadingScreen.SetActive(false);
        
        OnSceneLoaded?.Invoke();
    }
}
