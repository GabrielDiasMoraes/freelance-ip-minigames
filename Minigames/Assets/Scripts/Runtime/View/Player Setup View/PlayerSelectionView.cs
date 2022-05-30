using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class PlayerSelectionView : MonoBehaviour
    {
        [SerializeField] private PlayerCard _playerCardPrefab;
        [SerializeField] private Transform _playerListRoot;
        [SerializeField] private TextMeshProUGUI _playerCount;
        [SerializeField] private Button _minusButton;
        [SerializeField] private Button _plusButton;
        [SerializeField] private Button _confirmButton;

        private PoolUtility _cardPool;
        private List<PlayerCard> _activeCards;

#if UNITY_EDITOR
        [SerializeField] private PlayerSelectionViewData MockData;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                UpdateView(MockData);
            }
        }
#endif

        private void Awake()
        {
            _cardPool = new PoolUtility(_playerCardPrefab, 5, _playerListRoot);
            _activeCards = new List<PlayerCard>();
        }

        public void OpenView()
        {
            gameObject.SetActive(true);
        }

        public void CloseView()
        {
            gameObject.SetActive(true);
        }

        public void UpdateView(PlayerSelectionViewData viewData)
        {
            ConfigurePlayerCards(viewData);
            ConfigureButtons(viewData);
            _playerCount.text = viewData.PlayersInfo.Length.ToString();

        }

        private void ClearCards()
        {
            for (int i = 0; i < _activeCards.Count; i++)
            {
                _activeCards[i].gameObject.SetActive(false);
            }
        }

        private void ConfigurePlayerCards(PlayerSelectionViewData viewData)
        {
            ClearCards();
            PlayerInfo[] playersInfo = viewData.PlayersInfo;
            for (int i = 0; i < playersInfo.Length; i++)
            {
                PlayerInfo item = playersInfo[i];
                PlayerCard playerCard = (PlayerCard) _cardPool.GetFromPool();
                playerCard.Configure(item.PlayerIcon, item.PlayerName, item.OnChangeText, item.PlayerID);
                playerCard.gameObject.SetActive(true);
                playerCard.transform.SetSiblingIndex(i);
                _activeCards.Add(playerCard);
            }
        }

        private void ConfigureButtons(PlayerSelectionViewData viewData)
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
