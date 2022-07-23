using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    public class PlayerSelectionController : IController
    {

        public enum SelectionPhase
        {
            PlayerQuantitySelection,
            PlayerIconSelection
        }

        private GameData _gameData;
        public UnityAction OnEnable { get; set; }
        public UnityAction OnDispatch { get; set; }
        public UnityAction OnPlayerListChange { get; set; }

        public UnityAction<SelectionPhase> OnSelectionPhaseChange { get; set; }

        public List<PlayerData> Players => _gameData.Players;

        public SelectionPhase CurrentPhase { get => _currentPhase;
            set
            {
                if (value == _currentPhase)
                    return;
                _currentPhase = value;
                OnSelectionPhaseChange?.Invoke(_currentPhase);
            }
        }

        public List<int> PlayerIds;

        private SelectionPhase _currentPhase;

        private int _playerWithIconSelected;

        public PlayerSelectionController(GameData gameData)
        {
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
            PlayerIds = new List<int>() { 0, 1, 2, 3, 4, 5 };
            _currentPhase = SelectionPhase.PlayerQuantitySelection;
            _playerWithIconSelected = 0;
            _gameData.InitOrReset();
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
                Name = $"Jogador {count + 1}",
                Score = 0,
                PlayerIconID = -1,
            };

            Players.Add(newPlayer);
            OnPlayerListChange?.Invoke();
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
            if(CurrentPhase == SelectionPhase.PlayerQuantitySelection)
            {
                CurrentPhase = SelectionPhase.PlayerIconSelection;
                return;
            }
            _gameData.GameState = GameState.Game;
        }

        public void SelectCurrentIcon(int playerIndex, int iconID)
        {
            Players[playerIndex].PlayerIconID = iconID;
        }

    }

}
