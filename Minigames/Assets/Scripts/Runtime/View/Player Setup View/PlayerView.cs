using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private GameObject _playerCardPrefab;
        [SerializeField] private Transform _playerListRoot;
        [SerializeField] private TextMeshProUGUI _playerCount;
        [SerializeField] private Button _minusButton;
        [SerializeField] private Button _plusButton;
        [SerializeField] private Button _confirmButton;

        private PoolUtility<PlayerCard> 

#if UNITY_EDITOR
        [SerializeField] private PlayerViewData MockData;

        private void Start()
        {
            UpdateView(MockData);
        }
#endif

        public void UpdateView(PlayerViewData viewData)
        {
            ConfigurePlayerCards(viewData);
            ConfigureButtons(viewData);
            _playerCount.text = viewData.PlayersInfo.Length.ToString();

        }

        private void ConfigurePlayerCards(PlayerViewData viewData)
        {
            PlayerInfo[] playersInfo = viewData.PlayersInfo;
            foreach (var item in playersInfo)
            {
                
            }
        }

        private void ConfigureButtons(PlayerViewData viewData)
        {
            _minusButton.onClick.RemoveAllListeners();
            _plusButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.RemoveAllListeners();

            _minusButton.onClick.AddListener(viewData.OnMinusClicked);
            _plusButton.onClick.AddListener(viewData.OnPlusClicked);
            _confirmButton.onClick.AddListener(viewData.OnConfirmClicked);

            _minusButton.interactable = viewData.PlayersInfo.Length > 1;

            //TODO: Change it to a variable in the GameSetupData
            _plusButton.interactable = viewData.PlayersInfo.Length < 6;

        }
    }
}
