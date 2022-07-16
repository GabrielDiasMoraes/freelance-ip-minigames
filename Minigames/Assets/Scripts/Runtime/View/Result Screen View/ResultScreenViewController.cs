using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Minigames
{
    public class ResultScreenViewController
    {
        private readonly ResultController _resultController;
        private readonly GameData _gameData;
        private ResultScreenView _resultScreenPrefab;
        private readonly IconMap _iconMap;
        private readonly PoolUtility _viewPool;
        private readonly List<ResultScreenView> _viewSlots;

        public ResultScreenViewController(ResultController resultController, GameData gameData, ResultScreenView resultScreenPrefab, IconMap iconMap, DiContainer diContainer)
        {
            _resultController = resultController;
            _gameData = gameData;
            _resultScreenPrefab = resultScreenPrefab;
            _iconMap = iconMap;

            _viewPool = new PoolUtility(_resultScreenPrefab, 0, diContainer: diContainer);
            _viewSlots = new List<ResultScreenView>();

            _resultController.OnEnable += OnControllerEnable;
            _resultController.OnDisable += OnControllerDispatch;

        }

        private void OnControllerDispatch()
        {
            ClearViews();
        }

        private void OnControllerEnable()
        {
            ClearViews();
            PlayerResultInfo[] playerResultInfos = new PlayerResultInfo[_gameData.Players.Count];
            ResultScreenViewData resultScreenViewData = new ResultScreenViewData();
            int playersCount = _gameData.Players.Count;
            for (int i = 0; i < _gameData.Players.Count; i++)
            {
                var playerData = _gameData.Players[i];

                _iconMap.PlayerIcons.TryGetValue(playerData.ID, out var sprite);

                playerResultInfos[i] = new PlayerResultInfo
                {
                    PlayerName = playerData.Name,
                    PlayerScore = playerData.Score,
                    PlayerIcon = sprite,
                    PlayerSpentTime = playerData.TimeSpent
                };
            }
            resultScreenViewData.Results = playerResultInfos;
            resultScreenViewData.ContinueAction = _resultController.ResetGame;
            resultScreenViewData.PlayerCount = playersCount;

            for (int i = 0; i < _gameData.Players.Count; i++)
            {
                var playerData = _gameData.Players[i];
                ConfigureView(playerData.ID, i, playersCount, resultScreenViewData);
            }

        }

        private void ConfigureView(int playerId, int playerIndex, int playerCount, ResultScreenViewData resultScreenViewDataBase)
        {
            _iconMap.PlayerBackground.TryGetValue(playerId, out var background);

            resultScreenViewDataBase.Background = background;
            resultScreenViewDataBase.CurrentPlayerPosition = playerIndex;
            resultScreenViewDataBase.CanvasRotationZ = playerIndex >= Mathf.Ceil(playerCount / 2f) ? 180f : 0;
            resultScreenViewDataBase.CameraData = Utils.GenerateCameraRect(playerIndex, playerCount);
            ResultScreenView resultScreenView = _viewPool.GetFromPool<ResultScreenView>();
            resultScreenView.UpdateView(resultScreenViewDataBase);

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
