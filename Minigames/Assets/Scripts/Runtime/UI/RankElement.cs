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

        private int _position;
        public void Configure(int rankPosition, Sprite playerIcon, string playerName, int score)
        {
            _rankPosition.text = $"{rankPosition}º";
            _playerIcon.sprite = playerIcon;
            _playerName.text = playerName;
            _points.text = $"{score}";
            _position = rankPosition;
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
