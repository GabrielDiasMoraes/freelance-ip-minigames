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
        private Dictionary<int, float> _playerSpentTimePhase;

        private readonly WaitForSecondsRealtime _waitForPlayerResult;
        private readonly WaitForSecondsRealtime _waitForCorrectResult;

        private float phaseTimer;
        private float maxTimer;
        private Coroutine gameTimerCoroutine;

        public GameController(GameData gameData, GameViewSlot gameViewSlotPrefab, DiContainer diContainer, IconMap iconMap, CoroutineRunner coroutineRunner)
        {
            _gameData = gameData;
            _gameViewSlotPrefab = gameViewSlotPrefab;
            _iconMap = iconMap;
            _coroutineRunner = coroutineRunner;
            _viewPool = new PoolUtility(_gameViewSlotPrefab, 0, diContainer: diContainer);
            _viewSlots = new List<GameViewSlot>();
            _playerConfirmed = new Dictionary<int, bool>();
            _playerSpentTimePhase = new Dictionary<int, float>();
            _waitForPlayerResult = new WaitForSecondsRealtime(gameData.PlayerResultTimer);
            _waitForCorrectResult = new WaitForSecondsRealtime(gameData.CorrectResultTimer);
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
            _playerSpentTimePhase.Clear();
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
            maxTimer = phaseTimer = GetTimer();
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

                if (_iconMap.PlayerButtonBackground.TryGetValue(playerData.ID, out var buttonBackground))
                {
                    _viewData.BackgroundButtonImage = buttonBackground;
                }

                if (_iconMap.PlayerIcons.TryGetValue(playerData.ID, out var icon))
                {
                    _viewData.PlayerIcon = icon;
                }
                _viewData.PlayerID = playerData.ID;
                _viewData.PlayerName = playerData.Name;
                _viewData.CameraData = Utils.GenerateCameraRect(i, maxPlayers);
                _viewData.Items = GetItems();
                _viewData.CanvasRotationZ = i >= Mathf.Ceil(maxPlayers / 2f) ? 180f : 0;
                _viewData.OnConfirmClick += OnConfirmClicked;
                gameViewSlot.UpdateView(_viewData);
                gameViewSlot.UpdateGameTimer((int)phaseTimer);
            }
            _coroutineRunner.StartCoroutine(CooldownBlocker(delegate()
            {
                gameTimerCoroutine = _coroutineRunner.StartCoroutine(UpdateGameTimerCoroutine());
            }));
        }

        private IEnumerator CooldownBlocker(UnityAction onComplete = null)
        {
            var time = 5f;
            UpdateBlockCounter(true, (int)time);
            yield return new WaitForSecondsRealtime(1f);
            while (time >= 1)
            {
                UpdateBlockCounter(true, (int)time);
                time -= Time.deltaTime;
                yield return null;
            }

            UpdateBlockCounter(false);

            onComplete?.Invoke();

        }

        private void UpdateBlockCounter(bool isActive, int value = 0)
        {
            for (int i = 0; i < _viewSlots.Count; i++)
            {
                _viewSlots[i].UpdateBlockCounter(isActive, value);
            }
        }

        private IEnumerator UpdateGameTimerCoroutine()
        {
            yield return new WaitForSecondsRealtime(1f);
            while (phaseTimer >= 1f)
            {
                UpdateGameViewTimers((int)phaseTimer);
                phaseTimer -= Time.deltaTime;
                yield return null;
            }
            UpdateGameViewTimers((int)phaseTimer);
            _coroutineRunner.StartCoroutine(CompletePhase());
        }

        private void UpdateGameViewTimers(int value)
        {
            for (int i = 0; i < _viewSlots.Count; i++)
            {
                _viewSlots[i].UpdateGameTimer(value);
            }
        }

        private ItemData[] GetItems()
        {
            var currentLevel = _gameData.GetCurrentLevel;
            var gameItems = _gameData.GetRandomItemDataList(currentLevel.amountOfItems);

            gameItems.Shuffle();
            return gameItems.ToArray();
        }

        private float GetTimer()
        {
            var currentLevel = _gameData.GetCurrentLevel;
            return currentLevel.levelTime;
        }

        private void OnConfirmClicked(int playerID)
        {
            _playerConfirmed.TryAdd(playerID, true);
            _playerSpentTimePhase.TryAdd(playerID, maxTimer - phaseTimer);
            if(_playerConfirmed.Count == _gameData.Players.Count)
            {
                _coroutineRunner.StopCoroutine(gameTimerCoroutine);
                _coroutineRunner.StartCoroutine(CompletePhase());
            }
        }

        private IEnumerator CompletePhase()
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
                    if(!_playerSpentTimePhase.TryGetValue(player.ID, out var spentTime))
                    {
                        spentTime = maxTimer;
                    }
                    player.TimeSpent += spentTime;
                }
            }
            yield return _waitForPlayerResult;

            //Todo: Show Correct Result;
            for (int i = 0; i < _viewSlots.Count; i++)
            {
                var viewSlot = _viewSlots[i];
                viewSlot.ShowCorrectResults();
            }

            yield return _waitForCorrectResult;

            if (_gameData.TryNextPhase())
            {
                LoadNextPhase();
            }
            else
            {
                LoadResultPhase();
            }
        }

        private void LoadNextPhase()
        {
            _playerConfirmed.Clear();
            _playerSpentTimePhase.Clear();
            ConfigureViewSlots();
        }

        private void LoadResultPhase()
        {
            _gameData.GameState = GameState.Result;
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
