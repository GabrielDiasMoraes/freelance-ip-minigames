using System;
using System.Collections.Generic;

namespace Minigames
{
    public class PlayerSelectionViewController
    {
        private PlayerSelectionView _view;
        private PlayerSelectionViewData _playerViewData;
        private readonly PlayerSelectionController _playerSelectionController;
        private readonly IconMap _iconMap;
        private readonly GameData _gameData;

        public PlayerSelectionViewController(PlayerSelectionView view, PlayerSelectionController playerSelectionController, IconMap iconMap, GameData gameData)
        {
            _view = view;
            _playerSelectionController = playerSelectionController;
            _iconMap = iconMap;
            _gameData = gameData;
            _playerViewData = new PlayerSelectionViewData();

            _playerSelectionController.OnEnable += OnControllerEnable;
            _playerSelectionController.OnDispatch += OnControllerDispatch;
            _playerSelectionController.OnPlayerListChange += OnPlayerListChange;
            _playerViewData.ViewType = PlayerSelectionViewType.MainView;
            _playerViewData.OnPlusClicked += _playerSelectionController.AddNewPlayer;
            _playerViewData.OnMinusClicked += _playerSelectionController.RemovePlayer;
            _playerViewData.OnConfirmClicked += _playerSelectionController.Continue;
            _playerViewData.OnConfigClicked += OnConfigClicked;
            _playerViewData.LevelData = _gameData.LevelDataArray;
            _playerViewData.MaxPlayers = _gameData.MaxAllowedPlayer;
            _playerViewData.CorrectResultTimeShow = _gameData.CorrectResultTimer;
            _playerViewData.ResultTimeShow = _gameData.PlayerResultTimer;
            _playerViewData.OnBackClicked += OnBackClicked;
        }

        private void OnBackClicked()
        {
            _playerViewData = _view.GetViewData();
            _playerViewData.ViewType = PlayerSelectionViewType.MainView;
            if (_playerSelectionController.Players.Count > _playerViewData.MaxPlayers)
            {
                int difference = _playerSelectionController.Players.Count - _playerViewData.MaxPlayers;
                for (int i = 0; i < difference; i++)
                {
                    _playerSelectionController.RemovePlayer();
                }
            }
            ConfigurePlayers(_playerSelectionController.Players);
            _gameData.LevelDataArray = _playerViewData.LevelData;
            _gameData.MaxAllowedPlayer = _playerViewData.MaxPlayers;
            _gameData.CorrectResultTimer = _playerViewData.CorrectResultTimeShow;
            _gameData.PlayerResultTimer = _playerViewData.ResultTimeShow;
            _view.UpdateView(_playerViewData);
        }

        private void OnConfigClicked()
        {
            _playerViewData.ViewType = PlayerSelectionViewType.ConfigView;
            _view.UpdateView(_playerViewData);
        }

        private void OnControllerDispatch()
        {
            _view.CloseView();
        }

        private void OnPlayerListChange()
        {
            ConfigurePlayers(_playerSelectionController.Players);
            _view.UpdateView(_playerViewData);
        }

        private void OnControllerEnable()
        {
            ConfigurePlayers(_playerSelectionController.Players);
            _view.OpenView();
            _view.UpdateView(_playerViewData);
        }



        private void ConfigurePlayers(List<PlayerData> players)
        {
            _playerViewData.PlayersInfo = new PlayerInfo[players.Count];

            for (int i = 0; i < players.Count; i++)
            {
                PlayerInfo playerInfo = _playerViewData.PlayersInfo[i];
                PlayerData playerData = players[i];

                if (!_iconMap.PlayerIcons.TryGetValue(playerData.ID, out var sprite))
                {
                    throw new Exception("No Icon for that Player ID");
                }

                playerInfo.PlayerID = playerData.ID;
                playerInfo.PlayerName = playerData.Name;
                playerInfo.PlayerIcon = sprite;
                playerInfo.OnChangeText += _playerSelectionController.UpdatePlayerName;
                _playerViewData.PlayersInfo[i] = playerInfo;
            }
        }

    }
}
