using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Minigames
{
    public class PlayerCard : PoolableItem
    {
        [SerializeField] private Image _playerIcon;
        [SerializeField] private TMP_InputField _playerName;
        private UnityAction<int, string> _onNameChange;
        private int _playerID;

        public void Configure(Sprite playerIcon, string playerName, UnityAction<int, string> onNameChange, int playerID = -1)
        {
            _playerIcon.sprite = playerIcon;
            _playerName.text = playerName;
            _playerName.onEndEdit.RemoveAllListeners();
            _playerName.onEndEdit.AddListener(OnEndEdit);
            _playerID = playerID;
        }

        private void OnEndEdit(string value)
        {
            _onNameChange?.Invoke(_playerID, value);
        }

    }
}
