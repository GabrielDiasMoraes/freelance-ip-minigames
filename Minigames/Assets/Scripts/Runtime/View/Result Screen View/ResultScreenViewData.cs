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
        public Rect CameraData;
        public float CanvasRotationZ;
        public Sprite Background;
        public int CurrentPlayerPosition;
        public int PlayerCount;
    }

    [Serializable]
    public struct PlayerResultInfo
    {
        public Sprite PlayerIcon;
        public string PlayerName;
        public int PlayerScore;
        public float PlayerSpentTime;
    }
}
