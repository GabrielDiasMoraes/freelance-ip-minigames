using TMPro;
using UnityEngine;

namespace Minigames
{
    public class Slot : PoolableItem
    {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private TextMeshProUGUI _positionNumber;

        private Item _insideItem;
        private int _slotIndex;

        public RectTransform RectTransform => _rectTransform;

        public void ConfigureSlot(int slotIndex)
        {
            _slotIndex = slotIndex;
            _positionNumber.text = $"{_slotIndex + 1}º";
            _positionNumber.color = Color.black;
        }

        public void SetItem(Item item)
        {
            if(_insideItem != null && _insideItem != item)
            {
                _insideItem.ClearSlotRef();
                _insideItem.RevertToOriginalPosition();
            }
            _insideItem = item;
            item.transform.SetParent(RectTransform);
            item.transform.localPosition = Vector3.zero;
        }

        public bool ShowAnswer()
        {
            _positionNumber.color = _insideItem.DesiredPosition == _slotIndex ? Color.green : Color.red;
            return _insideItem.DesiredPosition == _slotIndex;
        }


        public void ClearItem()
        {
            _insideItem = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

    }
}
