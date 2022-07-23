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
        public int[] PlayersIds;
        public UnityAction OnPlusClicked;
        public UnityAction OnMinusClicked;
        public UnityAction OnConfirmClicked;
        public UnityAction OnConfigClicked;
        // Icon Select View
        public int SelectedIcon;
        public Sprite[] IconList;
        public Color CurrentPlayerColor;
        public int CurrentPlayerIndex;
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
        PlayerQuantityView,
        PlayerIconSelectView,
        ConfigView,
    }
}
