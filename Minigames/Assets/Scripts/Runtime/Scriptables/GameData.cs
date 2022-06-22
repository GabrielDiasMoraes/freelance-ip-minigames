using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    [CreateAssetMenu(menuName = "Data/GameData")]
    public class GameData : ScriptableObject
    {
        private GameState _gameState;
        private List<PlayerData> _players = new List<PlayerData>();

        [SerializeField, NonReorderable] private LevelInfo[] _levels;

        private int _currentLevelIndex = 0;

        public List<PlayerData> Players { get => _players; set => _players = value; }
        public LevelInfo[] Levels  => _levels;

        public LevelInfo GetCurrentLevel => Levels[_currentLevelIndex];

        public UnityAction<GameState> OnChangeGameState { get; set; }

        public GameState GameState
        {
            get => _gameState;
            set
            {
                if (_gameState == value) return;
                _gameState = value;
                OnChangeGameState?.Invoke(_gameState);
            }
        }

        public bool TryNextPhase()
        {
            if(_currentLevelIndex + 1 < Levels.Length)
            {
                _currentLevelIndex++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _gameState = GameState.PlayerSelection;
            _currentLevelIndex = 0;
            _players.Clear();
        }

    }

    public class PlayerData
    {
        public int ID;
        public string Name;
        public int Score;
    }

    [Serializable]
    public class LevelInfo
    {
        [NonReorderable]
        public ItemData[] Items;
    }

    [Serializable]
    public struct ItemData
    {
        public Sprite ItemImage;
        public int ItemDesiredPosition;
    }

    public enum GameState
    {
        None,
        PlayerSelection,
        Game,
        Result
    }
}
