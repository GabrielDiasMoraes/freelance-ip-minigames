using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    public class PlayerSelectionController : IController
    {
        private GameData _gameData;
        public UnityAction OnEnable { get; set; }
        public UnityAction OnDispatch { get; set; }
        public UnityAction OnPlayerListChange { get; set; }

        public List<PlayerData> Players => _gameData.Players;

        public List<int> PlayerIds;

        public PlayerSelectionController(GameData gameData)
        {
            PlayerIds = new List<int>() { 0, 1, 2, 3, 4, 5 };
            _gameData = gameData;
            _gameData.OnChangeGameState += OnChangeGameState;
        }

        private void OnChangeGameState(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.PlayerSelection:
                    EnableController();
                    break;
                case GameState.None:
                case GameState.Game:
                case GameState.Result:
                    DisableController();
                    break;
            }
        }

        public void EnableController()
        {
            _gameData.Reset();
            if(Players.Count == 0)
            {
                AddNewPlayer();
            }
            OnEnable?.Invoke();
        }

        public void DisableController()
        {
            OnDispatch?.Invoke();
        }

        public void AddNewPlayer()
        {
            int id = GetID();

            int count = Players.Count;

            PlayerData newPlayer = new PlayerData
            {
                ID = id,
                Name = $"Player {count + 1}",
                Score = 0
            };

            Players.Add(newPlayer);
            OnPlayerListChange?.Invoke();
        }

        public void UpdatePlayerName(int playerID, string newText)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                var player = Players[i];
                if (player.ID == playerID)
                {
                    player.Name = newText;
                }
            }
        }



        private int GetID()
        {
            PlayerIds.Shuffle();
            int index = Random.Range(0, PlayerIds.Count);
            int id = PlayerIds[index];
            PlayerIds.RemoveAt(index);
            return id;
        }

        public void RemovePlayer()
        {
            RemovePlayer(Players.Count - 1);
        }

        public void RemovePlayer(int index)
        {
            if (index >= Players.Count) return;

            PlayerIds.Add(Players[index].ID);
            Players.RemoveAt(index);
            OnPlayerListChange?.Invoke();
        }

        public void Continue()
        {
            _gameData.GameState = GameState.Game;
        }
    }

}
