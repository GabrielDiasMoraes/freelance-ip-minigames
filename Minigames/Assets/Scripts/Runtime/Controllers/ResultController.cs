using UnityEngine.Events;

namespace Minigames
{
    public class ResultController : IController
    {
        private readonly GameData _gameData;

        public UnityAction OnEnable { get; set; }
        public UnityAction OnDisable { get; set; }

        public ResultController(GameData gameData)
        {
            _gameData = gameData;
            _gameData.OnChangeGameState += OnChangeGameState;
        }

        private void OnChangeGameState(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.Result:
                    EnableController();
                    break;
                case GameState.None:
                case GameState.PlayerSelection:
                case GameState.Game:
                    DisableController();
                    break;
            }
        }

        public void DisableController()
        {
            OnDisable?.Invoke();
        }

        public void EnableController()
        {
            OnEnable?.Invoke();
        }
    }
}
