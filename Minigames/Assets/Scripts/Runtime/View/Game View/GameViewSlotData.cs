using System;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    [Serializable]
    public struct GameViewSlotData : IViewData
    {
        public int PlayerID;
        [NonReorderable]
        public ItemData[] Items;
        public Rect CameraData;
        public Sprite BackgroundImage;
        public Sprite BackgroundButtonImage;
        public Sprite PlayerIcon;
        public string PlayerName;
        public float CanvasRotationZ;
        public UnityAction<int> OnConfirmClick;
    }
}
