

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Minigames
{
    public class GameViewSlot : ViewBase<GameViewSlotData>
    {

        [SerializeField] Image _playerIcon;
        [SerializeField] TextMeshProUGUI _playerName;
        [SerializeField] Camera _gameViewCamera;
        [SerializeField] RectTransform _slotsRoot;
        [SerializeField] RectTransform _itemsRoot;
        [SerializeField] Image _backgroundImage;
        [SerializeField] private RectTransform _contentArea;
        [SerializeField] private RectTransform _context;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Image _confirmButtonBackground;
        [SerializeField] private GameObject _counterParent;
        [SerializeField] private TextMeshProUGUI _counterText;
        [SerializeField] private TextMeshProUGUI _gameTimer;

        private int _playerId;
        private PoolUtility _slotPool;
        private PoolUtility _itemPool;
        private InputController _inputController;
        private Camera _camera;
        private Canvas _gameCanvas;
        private List<Slot> _slotsObjects;
        private List<Item> _availableItems;
        private UnityAction<int> _onConfirmButton;
        private bool _gameEnded;
        private bool _hasBlocker;

        public bool CanInteract => !GameEnded && !_hasBlocker;

        public bool GameEnded { get => _gameEnded; set => _gameEnded = value; }

        [Inject]
        public void Construct(Item _itemPrefab, Slot _slotPrefab, InputController inputController, Camera camera, Canvas gameCanvas)
        {
            _availableItems = new List<Item>();
            _slotsObjects = new List<Slot>();
            _inputController = inputController;
            _camera = camera;
            _gameCanvas = gameCanvas;

            _slotPool = new PoolUtility(_slotPrefab, 0, _slotsRoot);
            _itemPool = new PoolUtility(_itemPrefab, 0, _itemsRoot);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ClearItems();
            ClearSlots();
        }

        public override void CloseView() { }

        public override void OpenView() { }

        public override void UpdateView(GameViewSlotData viewData)
        {
            _gameViewCamera.rect = viewData.CameraData;
            _backgroundImage.sprite = viewData.BackgroundImage;
            _confirmButtonBackground.sprite = viewData.BackgroundImage;
            _playerIcon.sprite = viewData.PlayerIcon;
            _playerName.text = viewData.PlayerName;
            _context.rotation = new Quaternion(0, 0, viewData.CanvasRotationZ, 0);
            _playerId = viewData.PlayerID;

            ClearItems();
            ClearSlots();
            for (int i = 0; i < viewData.Items.Length; i++)
            {
                var itemData = viewData.Items[i];
                ConfigureItem(itemData);
                CreateSlot(i);
            }
            List<ItemData> itemListClone = new List<ItemData>(viewData.Items);
            itemListClone = itemListClone.OrderByDescending(item => item.AvgMonthCons).ToList();
            for (int i = 0; i < itemListClone.Count; i++)
            {
                _slotsObjects[i].ConfigureSlot(i, itemListClone[i].AvgMonthCons);
            }

            //slotObj.ConfigureSlot(index);
            _confirmButton.interactable = true;
            ConfigureButton();
            _onConfirmButton = viewData.OnConfirmClick;
            GameEnded = false;
            _hasBlocker = false;
        }

        public void UpdateBlockCounter(bool isActive, int value = 0)
        {
            _counterParent.SetActive(isActive);
            _counterText.text = $"{value}";
            _hasBlocker = isActive;
        }

        public void UpdateGameTimer(int value)
        {
            _gameTimer.text = $"{value}";
        }

        public void ConfigureButton()
        {
            _confirmButton.onClick.RemoveAllListeners();
            for (int i = 0; i < _availableItems.Count; i++)
            {
                var item = _availableItems[i];
                if (!item.IsInSlot)
                {
                    _confirmButton.gameObject.SetActive(false);
                    return;
                }
            }
            _confirmButton.onClick.AddListener(OnConfirButtonClick);
            _confirmButton.gameObject.SetActive(true);
        }

        public int ShowResults()
        {
            GameEnded = true;
            int score = 0;
            for (int i = 0; i < _slotsObjects.Count; i++)
            {
                var slotObj = _slotsObjects[i];
                score += slotObj.ShowAnswer() ? 1: 0;
            }
            return score;
        }

        private void OnConfirButtonClick()
        {
            GameEnded = true;
            _confirmButton.interactable = false;
            _onConfirmButton?.Invoke(_playerId);
        }

        private void ConfigureItem(ItemData itemData)
        {
            Item itemObj = _itemPool.GetFromPool<Item>();
            itemObj.Constructor(_inputController, _contentArea, this, _camera, _gameCanvas);
            itemObj.ConfigureItem(itemData.ItemImage, itemData.ItemDesiredPosition, itemData.ItemText, itemData.ItemName);
            
            _availableItems.Add(itemObj);
        }

        private void CreateSlot(int index)
        {
            Slot slotObj = _slotPool.GetFromPool<Slot>();
            slotObj.transform.SetSiblingIndex(index);
            _slotsObjects.Add(slotObj);
        }

        private void ClearItems()
        {
            for (int i = 0; i < _availableItems.Count; i++)
            {
                _availableItems[i].enabled = false;
            }
            _availableItems.Clear();
        }

        private void ClearSlots()
        {
            for (int i = 0; i < _slotsObjects.Count; i++)
            {
                _slotsObjects[i].enabled = false;
            }
            _slotsObjects.Clear();
        }

        public bool IsInsideSlot(Vector2 screenPoint, Camera cameraRef, out Slot slot)
        {
            slot = null;
            for (int i = 0; i < _slotsObjects.Count; i++)
            {
                if(RectTransformUtility.RectangleContainsScreenPoint(_slotsObjects[i].RectTransform, screenPoint, cameraRef))
                {
                    slot = _slotsObjects[i];
                    return true;
                }
            }
            return false;
        }

        public int GetPlayerID()
        {
            return _playerId;
        }
    }
}
