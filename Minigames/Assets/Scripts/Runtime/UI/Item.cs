using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class Item : PoolableItem
    {
        [SerializeField] private Image _itemImage;
        private int _desiredPosition;

        // Serialized References
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Canvas _itemCanvas;
        [SerializeField] private TextMeshProUGUI _itemText;
        [SerializeField] private TextMeshProUGUI _itemName;

        // Injectables References
        private InputController _inputController;
        private RectTransform _contentArea;
        private GameViewSlot _gameView;
        private Camera _camera;
        private Canvas _gameCanvas;

        // Runtime References
        private Transform _originalParent;
        private int _originalSiblingIndex;
        private Slot _slotRef;
        private int _pointerID;
        private bool _isDragging;
        private bool _gameEnded;
        private bool _isInSlot;

        public bool IsInSlot => _isInSlot;

        public bool CanInteract => _gameView.CanInteract;
        public int DesiredPosition => _desiredPosition;

        public void Constructor(InputController inputController, RectTransform contentArea, GameViewSlot gameView, Camera camera, Canvas gameCanvas)
        {
            _inputController = inputController;
            _contentArea = contentArea;
            _camera = camera;
            _originalParent = transform.parent;
            _gameCanvas = gameCanvas;

            _inputController.OnNewPointer += OnNewPointer;
            _inputController.OnExitPointer += OnExitPointer;
            _inputController.OnUpdatePointer += OnUpdatePointer;
            _gameView = gameView;
            ClearSlotRef();
            RevertToOriginalPosition();
        }

        public void ConfigureItem(Sprite sprite, int desiredPosition, string itemText, string itemName)
        {
            _itemName.text = itemName;
            _desiredPosition = desiredPosition;
            _itemImage.sprite = sprite;
            _itemText.text = $"{itemText}/dia";
            _itemImage.type = Image.Type.Simple;
            _itemImage.preserveAspect = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Init()
        {
            base.Init();
            _itemCanvas.sortingLayerName = "Overlay"; //Overlay Layer
            _itemCanvas.sortingOrder = 1;
        }

        private void OnNewPointer(PointerData[] pointers)
        {
            if (!CanInteract) return;
            for (int i = 0; i < pointers.Length; i++)
            {
                PointerData pointerData = pointers[i];
                if (IsInsideArea(pointerData))
                {
                    _itemCanvas.sortingOrder = 2;
                    _isDragging = true;
                    _pointerID = pointerData.PointerID;
                    if (_slotRef == null)
                    {
                        _originalSiblingIndex = transform.GetSiblingIndex();
                    }
                    break;
                }
            }
        }

        private void OnUpdatePointer(PointerData[] pointers)
        {
            if (!_isDragging) return;

            if (!CanInteract) RevertToOriginalPosition();

            for (int i = 0; i < pointers.Length; i++)
            {
                var pointer = pointers[i];

                if (pointer.PointerID != _pointerID) continue;


                Vector3 worldPoint = _camera.ScreenToWorldPoint(pointer.PointerPosition);
                worldPoint.z = 300;
                _rectTransform.position = worldPoint;
                ClampPosition(_rectTransform);
            }
        }

        private void OnExitPointer(PointerData[] pointers)
        {
            if (!CanInteract) return;
            for (int i = 0; i < pointers.Length; i++)
            {
                PointerData pointerData = pointers[i];
                if (pointerData.PointerID == _pointerID)
                {
                    _isDragging = false;
                    _itemCanvas.sortingOrder = 1;
                    _pointerID = -1;
                    if (_gameView.IsInsideSlot(pointerData.PointerPosition, _camera, out var slot))
                    {
                        if(_slotRef != null)
                        {
                            _slotRef.ClearItem();
                        }
                        slot.SetItem(this);
                    }
                    else
                    {
                        ClearSlotRef();
                        RevertToOriginalPosition();
                    }
                }
            }
            _gameView.ConfigureConfirmButton();
        }

        private void ClampPosition(RectTransform rectTransform)
        {
            Vector3 screenPos = _camera.WorldToScreenPoint(transform.position);

            Rect rect = rectTransform.rect;

            int yMpy = (int)Math.Ceiling( 1 / _camera.rect.height);
            int xMpy = (int)Math.Ceiling(1 / _camera.rect.width);

            float heightScaled = _contentArea.rect.height * _gameCanvas.scaleFactor;
            float widthScaled = _contentArea.rect.width * _gameCanvas.scaleFactor;


            var yAdditional = _camera.rect.height * yMpy * (_camera.rect.y * heightScaled * yMpy);
            var xAdditional = _camera.rect.width * xMpy * (_camera.rect.x * widthScaled * xMpy);

            screenPos.x = Mathf.Clamp(screenPos.x - xAdditional, rect.width/2, widthScaled - rect.width/2) + xAdditional;
            screenPos.y = Mathf.Clamp(screenPos.y - yAdditional, rect.height/2, heightScaled - rect.height/2) + yAdditional;

            Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
            worldPos.z = transform.position.z;

            transform.position = worldPos;
        }


        
        

        public void RevertToOriginalPosition()
        {
            transform.SetParent(null);
            transform.SetParent(_originalParent);
            transform.SetSiblingIndex(_originalSiblingIndex);
        }

        public void ClearSlotRef()
        {
            if (IsInSlot)
            {
                _slotRef.ClearItem();
                _slotRef = null;
                _isInSlot = false;
                RevertToOriginalPosition();
            }
        }

        public void SetSlotRef(Slot slotRef)
        {
            _slotRef = slotRef;
            _isInSlot = true;
        }

        private bool IsInsideArea(PointerData pointerData)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, pointerData.PointerPosition, _camera);
        }
    }
}
