using System;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    [Serializable]
    public struct PlayerViewData
    {
        public PlayerInfo[] PlayersInfo;
        public UnityAction OnPlusClicked;
        public UnityAction OnMinusClicked;
        public UnityAction OnConfirmClicked;
    }

    [Serializable]
    public struct PlayerInfo
    {
        public int Index;
        public Sprite PlayerIcon;
        public string PlayerName;
        public UnityAction<string> OnChangeText;
    }
}
