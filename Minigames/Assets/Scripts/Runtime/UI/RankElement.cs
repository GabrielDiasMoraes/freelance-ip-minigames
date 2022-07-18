using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class RankElement : PoolableItem
    {
        [SerializeField] private TextMeshProUGUI _rankPosition;
        [SerializeField] private Image _playerIcon;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _points;
        [SerializeField] private TextMeshProUGUI _spentTime;
        [SerializeField] private Image _background;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _selectedColor;

        private int _position;
        public void Configure(int rankPosition, Sprite playerIcon, string playerName, int score, float spentTime, bool showHightlighted)
        {
            _rankPosition.text = $"{rankPosition}º";
            _playerIcon.sprite = playerIcon;
            _playerName.text = playerName;
            _points.text = $"{score}";
            _position = rankPosition;
            _spentTime.text = $"{spentTime:00.0}s";


            _background.color = showHightlighted ? _selectedColor : _normalColor;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _position = -1;
        }

        public void Reorder()
        {
            transform.SetSiblingIndex(_position - 1);
        }


    }
}
