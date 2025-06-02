using _Script.Manager;
using Zenject;

namespace _Script.Zenject
{
    public class SceneInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameManager>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<MonsterEars>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<UiManager>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerHealth>().FromComponentsInHierarchy().AsSingle(); 
            Container.BindInterfacesAndSelfTo<MonsterAi>().FromComponentsInHierarchy().AsSingle(); 
            Container.BindInterfacesAndSelfTo<MonsterController>().FromComponentsInHierarchy().AsSingle(); 
            Container.BindInterfacesAndSelfTo<SaveManager>().FromComponentsInHierarchy().AsSingle(); 
        }
    }
}