/*namespace Minigames
{

    public class GameFlowController
    {
        private readonly PlayerSelectionController _playerSelectionController;
        private readonly GameController _gameController;
        private readonly GameData _gameData;

        public GameFlowController(GameData gameData)
        {
            _gameData = gameData;
            //_gameData.OnChangeGameState += OnChangeGameState;
        }

        private void OnCompleteSelection()
        {
            _playerSelectionController.OnCompleteSelection -= OnCompleteSelection;
            _playerSelectionController.DisableController();
            _gameData.GameState = GameState.Game;
        }

        

        private void OnChangeGameState(GameState newGameState)
        {
            switch(newGameState)
            {
                case GameState.Boot:
                    _playerSelectionController.OnCompleteSelection += OnCompleteSelection;
                    _playerSelectionController.EnableController();
                    break;
                case GameState.Game:
                    _gameController.EnableController();
                    break;
                case GameState.Result:
                    _gameData.GameState = GameState.Boot;
                    break;
            }
        }

    }
}
*/