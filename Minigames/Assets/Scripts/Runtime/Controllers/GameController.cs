using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Minigames
{
    public class GameController : IController
    {
        public UnityAction OnEnable { get; set; }
        public UnityAction OnDispatch { get; set; }

        private readonly GameData _gameData;
        private readonly GameViewSlot _gameViewSlotPrefab;
        private readonly IconMap _iconMap;
        private readonly CoroutineRunner _coroutineRunner;

        private PoolUtility _viewPool;
        private List<GameViewSlot> _viewSlots;
        private Dictionary<int, bool> _playerConfirmed;

        private readonly WaitForSeconds _waitForSeconds;

        public GameController(GameData gameData, GameViewSlot gameViewSlotPrefab, DiContainer diContainer, IconMap iconMap, CoroutineRunner coroutineRunner)
        {
            _gameData = gameData;
            _gameViewSlotPrefab = gameViewSlotPrefab;
            _iconMap = iconMap;
            _coroutineRunner = coroutineRunner;
            _viewPool = new PoolUtility(_gameViewSlotPrefab, 0, diContainer: diContainer);
            _viewSlots = new List<GameViewSlot>();
            _playerConfirmed = new Dictionary<int, bool>();
            _waitForSeconds = new WaitForSeconds(3);
            _gameData.OnChangeGameState += OnChangeGameState;
        }

        private void OnChangeGameState(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.Game:
                    EnableController();
                    break;
                case GameState.None:
                case GameState.PlayerSelection:
                case GameState.Result:
                    DisableController();
                    break;
            }
        }

        public void EnableController()
        {
            _playerConfirmed.Clear();
            OnEnable?.Invoke();
            ClearViewSlots();
            CreateViewSlots();
            ConfigureViewSlots();
        }

        public void DisableController()
        {
            ClearViewSlots();
        }

        private void ClearViewSlots()
        {
            for (int i = 0; i < _viewSlots.Count; i++)
            {
                _viewSlots[i].enabled = false;
            }
            _viewSlots.Clear();
        }

        private void CreateViewSlots()
        {
            int maxPlayers = _gameData.Players.Count;
            for (int i = 0; i < maxPlayers; i++)
            {
                GameViewSlot gameViewSlot = _viewPool.GetFromPool<GameViewSlot>();                
                _viewSlots.Add(gameViewSlot);
            }
        }

        private void ConfigureViewSlots()
        {
            int maxPlayers = _gameData.Players.Count;
            for (int i = 0; i < _viewSlots.Count; i++)
            {
                var playerData = _gameData.Players[i];
                var gameViewSlot = _viewSlots[i];

                GameViewSlotData _viewData = new GameViewSlotData();

                if (_iconMap.PlayerBackground.TryGetValue(playerData.ID, out var background))
                {
                    _viewData.BackgroundImage = background;
                }

                if (_iconMap.PlayerIcons.TryGetValue(playerData.ID, out var icon))
                {
                    _viewData.PlayerIcon = icon;
                }
                _viewData.PlayerID = playerData.ID;
                _viewData.PlayerName = playerData.Name;
                _viewData.CameraData = GenerateCameraRect(i, maxPlayers);
                _viewData.Items = GetItems();
                _viewData.CanvasRotationZ = i >= Mathf.Ceil(maxPlayers / 2f) ? 180f : 0;
                _viewData.OnConfirmClick += OnConfirmClicked;
                gameViewSlot.UpdateView(_viewData);
            }
        }

        private ItemData[] GetItems()
        {
            List<ItemData> shuffleItems = new List<ItemData>(_gameData.GetCurrentLevel.Items);
            shuffleItems.Shuffle();
            return shuffleItems.ToArray();
        }

        private void OnConfirmClicked(int playerID)
        {
            _playerConfirmed.TryAdd(playerID, true);
            if(_playerConfirmed.Count == _gameData.Players.Count)
            {
                for (int i = 0; i < _viewSlots.Count; i++)
                {
                    var viewSlot = _viewSlots[i];
                    int score = viewSlot.ShowResults();
                    for (int o = 0; o < _gameData.Players.Count; o++)
                    {
                        var player = _gameData.Players[o];
                        if (player.ID != viewSlot.GetPlayerID())
                            continue;
                        player.Score += score;
                    }
                }
                if(_gameData.TryNextPhase())
                {
                    _coroutineRunner.StartCoroutine(LoadNextPhase());
                }
                else
                {
                    _coroutineRunner.StartCoroutine(LoadResultPhase());
                }
            }
        }

        private IEnumerator LoadNextPhase()
        {
            yield return _waitForSeconds;

            _playerConfirmed.Clear();
            ConfigureViewSlots();

            yield return null;
        }

        private IEnumerator LoadResultPhase()
        {
            yield return _waitForSeconds;
            _gameData.GameState = GameState.Result;
        }

        private Rect GenerateCameraRect(int viewIndex, int viewCount)
        {
            if (viewCount == 1)
            {
                return new Rect(Vector2.zero, Vector2.one);
            }
            else
            {
                int columnAmount = (int)Mathf.Ceil(viewCount / 2f);
                int columnAmountTop = viewCount - columnAmount;
                if (viewIndex+1 > columnAmount)
                {
                    return GenerateTopLineRect(viewIndex-columnAmount, columnAmountTop);
                }
                else
                {
                    return GenerateBottomLineRect(viewIndex, columnAmount);
                }
            }
        }

        private Rect GenerateBottomLineRect(int viewIndex, int columnAmount)
        {
            float heightSize = 0.5f;

            float widthSize = 1f / columnAmount;
            Vector2 position = new Vector2()
            {
                x = viewIndex * widthSize,
                y = 0,
            };

            Vector2 size = new Vector2()
            {
                x = widthSize,
                y = heightSize
            };
            return new Rect(position, size);
        }

        private Rect GenerateTopLineRect(int viewIndex, int columnAmount)
        {
            float heightSize = 0.5f;

            float widthSize = 1f / columnAmount;
            Vector2 position = new Vector2()
            {
                x = ((columnAmount-1) - viewIndex) * widthSize,
                y = 0.5f,
            };

            Vector2 size = new Vector2()
            {
                x = widthSize,
                y = heightSize
            };
            return new Rect(position, size);
        }

        private void ClearViews()
        {
            for (int i = 0; i < _viewSlots.Count; i++)
            {
                _viewSlots[i].enabled = false;
            }
            _viewSlots.Clear();
        }
    }
}
