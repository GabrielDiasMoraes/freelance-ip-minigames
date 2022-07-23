using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Minigames
{
    public class IconCard : PoolableItem
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Button _iconHiddenButton;
        [SerializeField] private Image _arrowPointer;

        private int _iconIndex;
        private UnityAction<int> _onClick;

        public void Configure(Sprite icon, int iconIndex, Color playerColor, UnityAction<int> OnClick)
        {
            _icon.sprite = icon;
            _iconIndex = iconIndex;
            _onClick = OnClick;
            _iconHiddenButton.onClick.RemoveAllListeners();
            _iconHiddenButton.onClick.AddListener(OnButtonClick);
            _arrowPointer.color = playerColor;
        }

        private void OnButtonClick()
        {
            _onClick?.Invoke(_iconIndex);
        }

        public void EnableArrow(bool value)
        {
            _arrowPointer.gameObject.SetActive(value);
        }
    }
}
