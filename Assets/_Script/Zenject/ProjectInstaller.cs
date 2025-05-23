using _Script.Puzzle;
using _Script.Settings;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private SceneLoaderManager _sceneLoaderManager;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerSettings>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
        Container.BindInterfacesAndSelfTo<GameStateManager>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
        Container.BindInterfacesAndSelfTo<PuzzleManager>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
        
        SceneLoaderManager sceneLoaderManager = Container.InstantiatePrefabForComponent<SceneLoaderManager>(_sceneLoaderManager);
        Container.Bind<SceneLoaderManager>().FromInstance(sceneLoaderManager).AsSingle().NonLazy();
    }
}