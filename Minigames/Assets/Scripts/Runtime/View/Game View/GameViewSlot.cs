

using System.Collections.Generic;
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

        private int _playerId;
        private PoolUtility _slotPool;
        private PoolUtility _itemPool;
        private InputController _inputController;
        private Camera _camera;
        private Canvas _gameCanvas;
        private List<Slot> _slotsObjects;
        private List<Item> _availableItems;
        private UnityAction<int> _onConfirmButton;

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

        public override void CloseView()
        {
            throw new System.NotImplementedException();
        }

        public override void OpenView()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateView(GameViewSlotData viewData)
        {
            _gameViewCamera.rect = viewData.CameraData;
            _backgroundImage.sprite = viewData.BackgroundImage;
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
                ConfigureSlot(i);
            }
            _confirmButton.interactable = true;
            ConfigureButton();
            _onConfirmButton = viewData.OnConfirmClick;
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
            for (int i = 0; i < _availableItems.Count; i++)
            {
                _availableItems[i].GameEnded = true;
            }
            _confirmButton.interactable = false;
            _onConfirmButton?.Invoke(_playerId);
        }

        private void ConfigureItem(ItemData itemData)
        {
            Item itemObj = _itemPool.GetFromPool<Item>();
            itemObj.Constructor(_inputController, _contentArea, this, _camera, _gameCanvas);
            itemObj.ConfigureItem(itemData.ItemImage, itemData.ItemDesiredPosition);
            
            _availableItems.Add(itemObj);
        }

        private void ConfigureSlot(int index)
        {
            Slot slotObj = _slotPool.GetFromPool<Slot>();
            slotObj.ConfigureSlot(index);
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
