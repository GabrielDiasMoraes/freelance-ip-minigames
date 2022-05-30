using System;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    [Serializable]
    public struct PlayerSelectionViewData
    {
        public PlayerInfo[] PlayersInfo;
        public UnityAction OnPlusClicked;
        public UnityAction OnMinusClicked;
        public UnityAction OnConfirmClicked;
    }

    [Serializable]
    public struct PlayerInfo
    {
        public int PlayerID;
        public string PlayerName;
        public Sprite PlayerIcon;
        public UnityAction<int, string> OnChangeText;
    }
}
