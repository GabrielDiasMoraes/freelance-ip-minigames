using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    [Serializable]
    public struct PlayerSelectionViewData : IViewData
    {
        public PlayerSelectionViewType ViewType;
        //Main View
        public PlayerInfo[] PlayersInfo;
        public UnityAction OnPlusClicked;
        public UnityAction OnMinusClicked;
        public UnityAction OnConfirmClicked;
        public UnityAction OnConfigClicked;
        // Config View
        public int MaxPlayers;
        public int ResultTimeShow;
        public int CorrectResultTimeShow;
        public List<LevelData> LevelData;
        public UnityAction OnBackClicked;
    }

    [Serializable]
    public enum PlayerSelectionViewType
    {
        MainView,
        ConfigView,
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
