using System;
using UnityEngine;

namespace Minigames
{
    [Serializable]
    public struct ResultScreenViewData : IViewData
    {
        [NonReorderable]
        public PlayerResultInfo[] Results;
    }

    [Serializable]
    public struct PlayerResultInfo
    {
        public Sprite PlayerIcon;
        public string PlayerName;
        public int PlayerScore;
    }
}
