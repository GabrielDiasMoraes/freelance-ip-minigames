using UnityEngine;
using Zenject;

namespace Minigames
{
    public class Boot : MonoBehaviour
    {
        private GameData _gameData;

        [Inject]
        public void Constructor(GameData gameData)
        {
            _gameData = gameData;
        }

        private void Start()
        {
            _gameData.OnChangeGameState?.Invoke(GameState.PlayerSelection);
        }
    }
}
