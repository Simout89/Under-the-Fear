using _Script.Settings;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameStateManager>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
        Container.BindInterfacesAndSelfTo<SettingsManager>()
            .FromNewComponentOnNewGameObject()
            .AsSingle()
            .NonLazy();
    }
}