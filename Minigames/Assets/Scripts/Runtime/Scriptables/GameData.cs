using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Minigames
{
    [CreateAssetMenu(menuName = "Data/GameData")]
    public class GameData : ScriptableObject
    {
        [SerializeField] GameItemsData _gameItems;
        [SerializeField] private List<LevelData> _levelDataArray;

        private GameState _gameState;
        private List<PlayerData> _players = new List<PlayerData>();

        private int _currentLevelIndex = 0;
        private int _maxAllowedPlayer = 4;
        private int _correctResultTimer = 5;
        private int _playerResultTimer = 5;

        public List<PlayerData> Players { get => _players; set => _players = value; }


        public UnityAction<GameState> OnChangeGameState { get; set; }

        public LevelData GetCurrentLevel => LevelDataArray[_currentLevelIndex];

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

        public List<LevelData> LevelDataArray { get => _levelDataArray; set => _levelDataArray = value; }

        public int MaxAllowedPlayer { get => _maxAllowedPlayer; set => _maxAllowedPlayer = value; }

        public int CorrectResultTimer { get => _correctResultTimer; set => _correctResultTimer = value; }

        public int PlayerResultTimer { get => _playerResultTimer; set => _playerResultTimer = value; }

        public bool TryNextPhase()
        {
            if (_currentLevelIndex + 1 < _levelDataArray.Count)
            {
                _currentLevelIndex++;
                return true;
            }
            return false;
        }

        public void InitOrReset()
        {
            _gameState = GameState.PlayerSelection;
            _currentLevelIndex = 0;
            _players.Clear();
        }

        public List<ItemData> GetRandomItemDataList(int ammount)
        {
            var itemDataList = new List<ItemData>();
            var gameItems = _gameItems.GetGameItems();
            gameItems.Shuffle();

            for (int i = 0; i < ammount; i++)
            {
                var randIndex = UnityEngine.Random.Range(0, gameItems.Count);
                var randGameItem = gameItems[randIndex];
                ItemData newItemData = new ItemData()
                {
                    AvgMonthCons = randGameItem.AvgMonthConsumption,
                    ItemImage = randGameItem.ItemImage,
                    ItemName = randGameItem.Name,
                    ItemText = randGameItem.UsagePerDay,
                };
                gameItems.RemoveAt(randIndex);
                itemDataList.Add(newItemData);
            }

            itemDataList = itemDataList.OrderByDescending(item => item.AvgMonthCons).ToList();
            for (int i = 0; i < itemDataList.Count; i++)
            {
                itemDataList[i] = new ItemData()
                {
                    ItemDesiredPosition = i,
                    AvgMonthCons = itemDataList[i].AvgMonthCons,
                    ItemImage = itemDataList[i].ItemImage,
                    ItemName = itemDataList[i].ItemName,
                    ItemText = itemDataList[i].ItemText,
                };
            }


            return itemDataList;
        }



    }

    public class PlayerData
    {
        public int ID;
        public string Name;
        public int Score;
        public float TimeSpent;
        public bool HasQuitted;
    }

    [Serializable]
    public struct LevelData
    {
        public int amountOfItems;
        public float levelTime;
    }


    [Serializable]
    public struct ItemData
    {
        public string ItemName;
        public Sprite ItemImage;
        public int ItemDesiredPosition;
        public string ItemText;
        public float AvgMonthCons;
    }

    public enum GameState
    {
        None,
        PlayerSelection,
        Game,
        Result
    }
}
