using System;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    [Serializable]
    public struct ResultScreenViewData : IViewData
    {
        [NonReorderable]
        public PlayerResultInfo[] Results;
        public UnityAction ContinueAction;
    }

    [Serializable]
    public struct PlayerResultInfo
    {
        public Sprite PlayerIcon;
        public string PlayerName;
        public int PlayerScore;
    }
}
