using System.Globalization;
using TMPro;
using UnityEngine;

namespace Minigames
{
    public class Slot : PoolableItem
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TextMeshProUGUI _positionNumber;
        [SerializeField] private TextMeshProUGUI _avgConsumption;

        private Item _insideItem;
        private int _slotIndex;

        public bool IsDesiredPosition => _insideItem != null && _insideItem.DesiredPosition == _slotIndex;

        public RectTransform RectTransform => _rectTransform;

        public void ConfigureSlot(int slotIndex, float avgConsumption)
        {
            _slotIndex = slotIndex;
            _positionNumber.text = $"{_slotIndex + 1}º";
            _positionNumber.color = Color.black;
            _avgConsumption.text = $"{avgConsumption.ToString("0.00",CultureInfo.GetCultureInfo("pt-BR"))} kWh/mês";
        }

        public void SetItem(Item item)
        {
            if(_insideItem != null && _insideItem != item)
            {
                _insideItem.ClearSlotRef();
            }
            _insideItem = item;
            item.transform.SetParent(RectTransform);
            item.transform.localPosition = Vector3.zero;
        }

        public bool ShowAnswer()
        {
            _positionNumber.color = IsDesiredPosition ? Color.green : Color.red;
            return IsDesiredPosition;
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
