using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class PlayerCard : MonoBehaviour
    {
        [SerializeField] private Image _playerIcon;
        [SerializeField] private TMP_InputField _playerName;

        public void Configure(Sprite playerIcon, string playerName)
        {
            _playerIcon.sprite = playerIcon;
            _playerName.text = playerName;
        }

    }
}
