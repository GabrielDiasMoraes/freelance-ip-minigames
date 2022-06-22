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

        public PlayerSelectionViewController(PlayerSelectionView view, PlayerSelectionController playerSelectionController, IconMap iconMap)
        {
            _view = view;
            _playerSelectionController = playerSelectionController;
            _iconMap = iconMap;
            _playerViewData = new PlayerSelectionViewData();

            _playerSelectionController.OnEnable += OnControllerEnable;
            _playerSelectionController.OnDispatch += OnControllerDispatch;
            _playerSelectionController.OnPlayerListChange += OnPlayerListChange;
            _playerViewData.OnPlusClicked += _playerSelectionController.AddNewPlayer;
            _playerViewData.OnMinusClicked += _playerSelectionController.RemovePlayer;
            _playerViewData.OnConfirmClicked += _playerSelectionController.Continue;
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
