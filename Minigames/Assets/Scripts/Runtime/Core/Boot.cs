using UnityEngine;
using Zenject;

namespace Minigames
{
    public class Boot : MonoBehaviour
    {
        private GameFlowController _gameFlowController;

        [Inject]
        public void Constructor(GameFlowController gameFlowController)
        {
            _gameFlowController = gameFlowController;
        }

        private void Start()
        {
            _gameFlowController.GameState = GameState.Boot;
        }
    }
}
