using UnityEngine;
using Zenject;

namespace Minigames {

    public class ProjectContextInstaller : MonoInstaller<ProjectContextInstaller>
    {
        [SerializeField] private PlayerSelectionView _playerViewPrefab;
        [SerializeField] private ResultScreenView _resultScreenView;
        [SerializeField] private GameViewSlot _gameViewSlot;

        [SerializeField] private IconMap _iconMap;
        [SerializeField] private GameData _gameData;

        [SerializeField] private Item _itemPrefab;
        [SerializeField] private Slot _slotPrefab;

        [SerializeField] private RectTransform _gameCanvas;
        [SerializeField] private CoroutineRunner _coroutineRunner;

        public override void InstallBindings()
        {
            //Core
            Container.Bind<CoroutineRunner>().FromInstance(_coroutineRunner).AsSingle();

            //Prefabs
            Container.Bind<Item>().FromInstance(_itemPrefab).AsSingle();
            Container.Bind<Slot>().FromInstance(_slotPrefab).AsSingle();
            Container.Bind<GameViewSlot>().FromInstance(_gameViewSlot).AsSingle();

            //Scriptables
            Container.Bind<IconMap>().FromInstance(_iconMap).AsSingle();
            var _sessionGameData = Instantiate(_gameData);
            _sessionGameData.Reset();
            Container.Bind<GameData>().FromInstance(_sessionGameData).AsSingle();

            // Controllers
            Container.Bind<PlayerSelectionController>().ToSelf().AsSingle();
            Container.Bind<ResultController>().ToSelf().AsSingle();
            Container.Bind<GameController>().ToSelf().AsSingle();
            Container.Bind(typeof(InputController), typeof(ITickable)).To<InputController>().AsSingle();

            //View
            PlayerSelectionView playerSelectionView = Container.InstantiatePrefabForComponent<PlayerSelectionView>(_playerViewPrefab, _gameCanvas);
            Container.Bind<PlayerSelectionView>().FromInstance(playerSelectionView).AsSingle();

            ResultScreenView resultScreenView = Container.InstantiatePrefabForComponent<ResultScreenView>(_resultScreenView, _gameCanvas);
            Container.Bind<ResultScreenView>().FromInstance(resultScreenView).AsSingle();
        }

        private void Start()
        {
            Container.Instantiate<PlayerSelectionViewController>();
            Container.Instantiate<ResultScreenViewController>();
            Container.Instantiate<GameController>();
        }
    }
}