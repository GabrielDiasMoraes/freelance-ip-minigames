using System.Collections.Generic;
using UnityEngine;

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
            _playerSelectionController.OnSelectionPhaseChange += OnSelectionPhaseChange;
            _playerViewData.ViewType = PlayerSelectionViewType.PlayerQuantityView;
            _playerViewData.OnPlusClicked += _playerSelectionController.AddNewPlayer;
            _playerViewData.OnMinusClicked += _playerSelectionController.RemovePlayer;
            _playerViewData.OnConfirmClicked += _playerSelectionController.Continue;
            _playerViewData.OnConfigClicked += OnConfigClicked;
            _playerViewData.LevelData = _gameData.LevelDataArray;
            _playerViewData.MaxPlayers = _gameData.MaxAllowedPlayer;
            _playerViewData.CorrectResultTimeShow = _gameData.CorrectResultTimer;
            _playerViewData.ResultTimeShow = _gameData.PlayerResultTimer;
            _playerViewData.OnBackClicked += OnBackClicked;
            _playerViewData.IconList = _iconMap.PlayerIcons;
        }

        private void OnSelectionPhaseChange(PlayerSelectionController.SelectionPhase currentPhase)
        {
            _playerViewData.ViewType = PlayerSelectionViewType.PlayerIconSelectView;
            _playerViewData.OnConfirmClicked -= _playerSelectionController.Continue;
            _playerViewData.OnConfirmClicked += SelectCurrentIcon;
            _playerViewData.SelectedIcon = -1;
            _playerViewData.CurrentPlayerIndex = 0;
            _playerViewData.CurrentPlayerColor = _gameData.PlayerColor[_playerViewData.CurrentPlayerIndex];
            _view.UpdateView(_playerViewData);
        }

        private void SelectCurrentIcon()
        {
            _playerSelectionController.SelectCurrentIcon(_playerViewData.CurrentPlayerIndex, _view.GetSelectedIcon());
            _playerViewData.CurrentPlayerIndex++;
            if (_playerViewData.CurrentPlayerIndex >= _playerSelectionController.Players.Count)
            {
                _playerSelectionController.Continue();
                return;
            }
            _playerViewData.CurrentPlayerColor = _gameData.PlayerColor[_playerViewData.CurrentPlayerIndex];
            _playerViewData.SelectedIcon = -1;
            _view.UpdateView(_playerViewData);
        }

        private void OnBackClicked()
        {
            _playerViewData = _view.GetViewData();
            _playerViewData.ViewType = PlayerSelectionViewType.PlayerQuantityView;
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
            _playerViewData.PlayersIds = new int[players.Count];

            for (int i = 0; i < players.Count; i++)
            {
                PlayerData playerData = players[i];
                _playerViewData.PlayersIds[i] = playerData.ID;
            }
        }

    }
}
