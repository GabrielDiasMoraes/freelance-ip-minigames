using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class LevelInfoView : PoolableItem
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private Button _minusButtonItems;
        [SerializeField] private TextMeshProUGUI _itemsText;
        [SerializeField] private Button _plusButtonItems;
        [Space,SerializeField] private Button _minusButtonTime;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private Button _plusButtonTime;

        private LevelData _levelData;
        private float CurrentTimeSeconds { get => _levelData.levelTime; set => _levelData.levelTime = value; }
        private int ItemAmount { get => _levelData.amountOfItems; set => _levelData.amountOfItems = value; }

    private float _maxTimeSeconds;
        private int _maxItemAmount;

        public void Configure(int levelIndex, LevelData levelData, float maxTimeSeconds, int maxItemAmount)
        {
            _maxTimeSeconds = maxTimeSeconds;
            _maxItemAmount = maxItemAmount;
            _levelData = levelData;

            _levelNumberText.text = $"Level {levelIndex + 1}";

            _plusButtonTime.onClick.RemoveAllListeners();
            _minusButtonTime.onClick.RemoveAllListeners();
            _plusButtonItems.onClick.RemoveAllListeners();
            _minusButtonItems.onClick.RemoveAllListeners();

            _plusButtonTime.onClick.AddListener(() => { UpdateTime(10); }) ;
            _minusButtonTime.onClick.AddListener(() => { UpdateTime(-10); });
            _plusButtonItems.onClick.AddListener(() => { UpdateItem(1); });
            _minusButtonItems.onClick.AddListener(() => { UpdateItem(-1); });
            UpdateTime(0);
            UpdateItem(0);
        }

        public float GetHeight()
        {
            return _rectTransform.rect.height;
        }

        public LevelData GetLevelData()
        {
            return _levelData;
        }

        private void UpdateTime(float valueToAdd)
        {
            CurrentTimeSeconds += valueToAdd;
            CurrentTimeSeconds = Mathf.Clamp(CurrentTimeSeconds, 10f, _maxTimeSeconds);
            _timeText.text = $"{CurrentTimeSeconds:00}<size=70%>s</size>";
            _plusButtonTime.interactable = CurrentTimeSeconds < _maxTimeSeconds;
            _minusButtonTime.interactable = CurrentTimeSeconds > 10f;
        }

        private void UpdateItem(int valueToAdd)
        {
            ItemAmount += valueToAdd;
            ItemAmount = Math.Clamp(ItemAmount, 2, _maxItemAmount);
            _itemsText.text = $"{ItemAmount}";
            _plusButtonItems.interactable = ItemAmount < _maxItemAmount;
            _minusButtonItems.interactable = ItemAmount > 2;
        }


        private void OnDisable()
        {
            base.OnDisable();
            _minusButtonTime.interactable = _plusButtonTime.interactable = false;
            _plusButtonItems.interactable = _minusButtonItems.interactable = false;
        }
    }
}
