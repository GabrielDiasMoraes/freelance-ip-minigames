using UnityEngine;
using Zenject;

namespace Minigames
{
    public class GameAreaSlotInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform _contentArea;
        [SerializeField] private GameViewSlot _gameView;
        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _canvas;


        public override void InstallBindings()
        {
            Container.Bind<RectTransform>().FromInstance(_contentArea).WhenInjectedInto<GameViewSlot>();
            Container.Bind<GameViewSlot>().FromInstance(_gameView).WhenInjectedInto<GameViewSlot>();
            Container.Bind<Canvas>().FromInstance(_canvas).AsSingle();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle();


        }
    }
}
