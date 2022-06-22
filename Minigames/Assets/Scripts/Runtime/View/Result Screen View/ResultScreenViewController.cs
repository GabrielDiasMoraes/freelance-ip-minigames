using System;
using System.Collections.Generic;

namespace Minigames
{
    public class ResultScreenViewController
    {
        private readonly ResultController _resultController;
        private readonly GameData _gameData;
        private ResultScreenView _view;
        private ResultScreenViewData _resultScreenViewData;
        private readonly IconMap _iconMap;

        public ResultScreenViewController(ResultController resultController, GameData gameData, ResultScreenView view, IconMap iconMap)
        {
            _resultController = resultController;
            _gameData = gameData;
            _view = view;
            _iconMap = iconMap;

            _resultController.OnEnable += OnControllerEnable;
            _resultController.OnDisable += OnControllerDispatch;
        }

        private void OnControllerDispatch()
        {
            _view.CloseView();
        }

        private void OnControllerEnable()
        {
            PlayerResultInfo[] playerResultInfos = new PlayerResultInfo[_gameData.Players.Count];
            for(int i = 0; i < _gameData.Players.Count; i++)
            {
                var playerData = _gameData.Players[i];

                _iconMap.PlayerIcons.TryGetValue(playerData.ID, out var sprite);

                playerResultInfos[i] = new PlayerResultInfo
                {
                    PlayerName = playerData.Name,
                    PlayerScore = playerData.Score,
                    PlayerIcon = sprite
                };
            }
            _resultScreenViewData.Results = playerResultInfos;
            _view.OpenView();
            _view.UpdateView(_resultScreenViewData);
        }

    }
}
