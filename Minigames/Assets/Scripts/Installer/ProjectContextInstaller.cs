using UnityEngine;
using Zenject;

namespace Minigames {

    public class ProjectContextInstaller : MonoInstaller<ProjectContextInstaller>
    {
        [SerializeField] private PlayerSelectionView _playerViewPrefab;
        [SerializeField] private Transform _gameCanvas;
        [SerializeField] private IconMap _iconMap;


        public override void InstallBindings()
        {
            PlayerSelectionView playerView = Instantiate(_playerViewPrefab, _gameCanvas);
            Container.Bind<PlayerSelectionView>().FromInstance(playerView).AsSingle();
            Container.Bind<IconMap>().FromInstance(_iconMap).AsSingle();
            Container.Bind<PlayerSelectionController>().ToSelf().AsSingle();
            Container.Bind<GameFlowController>().ToSelf().AsSingle();

            Container.Instantiate<PlayerSelectionViewController>();
        }
    }
}