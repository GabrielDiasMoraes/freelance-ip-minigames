using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    [CreateAssetMenu(menuName = "Data/GameItemsData")]
    public class GameItemsData : ScriptableObject
    {
        [SerializeField] GameItem[] _gameItems;

        public List<GameItem> GetGameItems()
        {
            return new List<GameItem>(_gameItems); ;
        }
    }

    [Serializable]
    public struct GameItem
    {
        public string Name;
        public float AvgMonthConsumption;
        public Sprite ItemImage;
        public string UsagePerDay;
    }
}
