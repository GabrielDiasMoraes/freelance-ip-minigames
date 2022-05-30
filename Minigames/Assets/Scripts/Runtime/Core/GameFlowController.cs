using System.Collections.Generic;

namespace Minigames
{
    public enum GameState
    {
        None,
        Boot,
        PlayerSelection,
        Game,
        Result
    }

    public class GameFlowController
    {
        private readonly PlayerSelectionController _playerSelectionController;
        private GameState _gameState;

        public GameFlowController(PlayerSelectionController playerSelectionController)
        {
            _gameState = GameState.None;
            _playerSelectionController = playerSelectionController;
        }


        public GameState GameState
        {
            get => _gameState;
            set
            {
                if (_gameState == value) return;
                _gameState = value;
                OnChangeGameState(_gameState);
            }
        }


        private void OnChangeGameState(GameState newGameState)
        {
            switch(newGameState)
            {
                case GameState.Boot:
                    _playerSelectionController.EnableController();
                    break;
            }
        }

    }
}
