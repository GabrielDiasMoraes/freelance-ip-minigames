using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class PlayerSelectionView : ViewBase<PlayerSelectionViewData>
    {
        [SerializeField] private PlayerCard _playerCardPrefab;
        [SerializeField] private LevelInfoView _levelInfoCardPrefab;
        [Header("Main View")]
        [SerializeField] private GameObject _mainViewRoot;
        [SerializeField] private Transform _playerListRoot;
        [SerializeField] private TextMeshProUGUI _playerCount;
        [SerializeField] private Button _minusButton;
        [SerializeField] private Button _plusButton;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _configButton;
        [Header("Config View")]
        [SerializeField] private GameObject _configViewRoot;
        [SerializeField] private Button _backButton;
        //Max PLayer
        [SerializeField] private Button _maxPlayerMinusButton;
        [SerializeField] private TextMeshProUGUI _maxPlayerText;
        [SerializeField] private Button _maxPlayerPlusButton;
        //Player Result Time
        [SerializeField] private Button _playerResultTimeMinusButton;
        [SerializeField] private TextMeshProUGUI _playerResultTimeText;
        [SerializeField] private Button _playerResultTimePlusButton;
        //Correct Result Time
        [SerializeField] private Button _correctResultTimeMinusButton;
        [SerializeField] private TextMeshProUGUI _correctResultTimeText;
        [SerializeField] private Button _correctResultTimePlusButton;
        // Round List
        [SerializeField] private RectTransform _levelListRoot;
        [SerializeField] private Button _addNewLevel;

        private PoolUtility _cardPool;
        private PoolUtility _levelPool;
        private List<PlayerCard> _activePlayerCards;
        private List<LevelInfoView> _activeLevelCards;
        private PlayerSelectionViewData _viewData;

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

        public override void CloseView()
        {
            gameObject.SetActive(false);
        }

        public override void OpenView()
        {
            gameObject.SetActive(true);
        }

        public override void UpdateView(PlayerSelectionViewData viewData)
        {
            _viewData = viewData;
            if (_viewData.ViewType == PlayerSelectionViewType.MainView)
            {
                _mainViewRoot.SetActive(true);
                _configViewRoot.SetActive(false);
                ConfigurePlayerCards();
                ConfigureMainButtons();
                _playerCount.text = _viewData.PlayersInfo.Length.ToString();
                return;
            }
            _mainViewRoot.SetActive(false);
            _configViewRoot.SetActive(true);
            ConfigureConfigButtons();
            ConfigureMaxPlayerText();
            ConfigurePlayerScoreTime();
            ConfigureCorrectResultTime();
            ConfigureLevelCards();
        }

        private void ConfigureLevelCards()
        {
            ClearCards(_activeLevelCards);
            float contentHeight = 0f;
            for (int i = 0; i < _viewData.LevelData.Count; i++)
            {
                var levelCard = _levelPool.GetFromPool<LevelInfoView>();
                levelCard.transform.SetSiblingIndex(i);
                LevelData levelData = _viewData.LevelData[i];
                levelCard.Configure(i, levelData, 90f, 8);
                _activeLevelCards.Add(levelCard);
                contentHeight += levelCard.GetHeight() + 15f;
            }

            _levelListRoot.sizeDelta = new Vector2(_levelListRoot.sizeDelta.x, contentHeight);
        }

        private void AddNewLevel()
        {
            _viewData = GetViewData();
            _viewData.LevelData.Add(new LevelData());
            ConfigureLevelCards();
        }

        private void Awake()
        {
            _cardPool = new PoolUtility(_playerCardPrefab, 5, _playerListRoot);
            _levelPool = new PoolUtility(_levelInfoCardPrefab, 3, _levelListRoot);
            _activePlayerCards = new List<PlayerCard>();
            _activeLevelCards = new List<LevelInfoView>();
        }

        private void ClearCards<T>(List<T> cards) where T: PoolableItem
        {
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].gameObject.SetActive(false);
            }
            cards.Clear();
        }

        private void ConfigurePlayerCards()
        {
            ClearCards(_activePlayerCards);
            var playersInfo = _viewData.PlayersInfo;
            for (int i = 0; i < playersInfo.Length; i++)
            {
                PlayerInfo item = playersInfo[i];
                PlayerCard playerCard = (PlayerCard) _cardPool.GetFromPool();
                playerCard.Configure(item.PlayerIcon, item.PlayerName, item.OnChangeText, item.PlayerID);
                playerCard.gameObject.SetActive(true);
                playerCard.transform.SetSiblingIndex(i);
                _activePlayerCards.Add(playerCard);
            }
        }

        

        private void ConfigureMainButtons()
        {
            _minusButton.onClick.RemoveAllListeners();
            _plusButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.RemoveAllListeners();
            _configButton.onClick.RemoveAllListeners();

            _minusButton.onClick.AddListener(_viewData.OnMinusClicked);
            _plusButton.onClick.AddListener(_viewData.OnPlusClicked);
            _confirmButton.onClick.AddListener(_viewData.OnConfirmClicked);
            _configButton.onClick.AddListener(_viewData.OnConfigClicked);

            _minusButton.interactable = _viewData.PlayersInfo.Length > 1;

            _plusButton.interactable = _viewData.PlayersInfo.Length < _viewData.MaxPlayers;
        }

        private void ConfigureConfigButtons()
        {
            _maxPlayerMinusButton.onClick.RemoveAllListeners();
            _maxPlayerPlusButton.onClick.RemoveAllListeners();
            _playerResultTimeMinusButton.onClick.RemoveAllListeners();
            _playerResultTimePlusButton.onClick.RemoveAllListeners();
            _correctResultTimeMinusButton.onClick.RemoveAllListeners();
            _correctResultTimePlusButton.onClick.RemoveAllListeners();
            _backButton.onClick.RemoveAllListeners();
            _addNewLevel.onClick.RemoveAllListeners();

            _backButton.onClick.AddListener(_viewData.OnBackClicked);
            _addNewLevel.onClick.AddListener(AddNewLevel);

            UpdateMaxPlayerButtons();
            UpdatePlayerResultButtons();
            UpdateCorrectResultButtons();

            _maxPlayerMinusButton.onClick.AddListener(delegate ()
            {
                _viewData.MaxPlayers--;
                UpdateMaxPlayerButtons();
            });

            _maxPlayerPlusButton.onClick.AddListener(delegate ()
            {
                _viewData.MaxPlayers++;
                UpdateMaxPlayerButtons();
            });

            _playerResultTimeMinusButton.onClick.AddListener(delegate ()
            {
                _viewData.ResultTimeShow--;
                UpdatePlayerResultButtons();
            });

            _playerResultTimePlusButton.onClick.AddListener(delegate ()
            {
                _viewData.ResultTimeShow++;
                UpdatePlayerResultButtons();
            });

            _correctResultTimeMinusButton.onClick.AddListener(delegate ()
            {
                _viewData.CorrectResultTimeShow--;
                UpdateCorrectResultButtons();
            });

            _correctResultTimePlusButton.onClick.AddListener(delegate ()
            {
                _viewData.CorrectResultTimeShow++;
                UpdateCorrectResultButtons();
            });
        }
        

        private void UpdateMaxPlayerButtons()
        {
            ConfigureMaxPlayerText();
            _maxPlayerMinusButton.interactable = _viewData.MaxPlayers > 1;
            _maxPlayerPlusButton.interactable = _viewData.MaxPlayers < 6;
        }

        private void UpdatePlayerResultButtons()
        {
            ConfigurePlayerScoreTime();
            _playerResultTimeMinusButton.interactable = _viewData.ResultTimeShow > 1;
            _playerResultTimePlusButton.interactable = _viewData.ResultTimeShow < 10;
        }

        private void UpdateCorrectResultButtons()
        {
            ConfigureCorrectResultTime();
            _correctResultTimeMinusButton.interactable = _viewData.CorrectResultTimeShow > 1;
            _correctResultTimePlusButton.interactable = _viewData.CorrectResultTimeShow < 10;
        }

        private void ConfigureMaxPlayerText()
        {
            _viewData.MaxPlayers = Mathf.Clamp(_viewData.MaxPlayers, 1, 6);
            _maxPlayerText.text = $"{_viewData.MaxPlayers}";
        }

        private void ConfigurePlayerScoreTime()
        {
            _viewData.CorrectResultTimeShow = Mathf.Clamp(_viewData.CorrectResultTimeShow, 1, 10);
            _playerResultTimeText.text = $"{_viewData.ResultTimeShow:00}<size=70%>s</size>";
        }

        private void ConfigureCorrectResultTime()
        {
            _viewData.CorrectResultTimeShow = Mathf.Clamp(_viewData.CorrectResultTimeShow, 1, 10);
            _correctResultTimeText.text = $"{_viewData.CorrectResultTimeShow:00}<size=70%>s</size>";
        }

        public PlayerSelectionViewData GetViewData()
        {
            _viewData.LevelData.Clear();
            for (int i = 0; i < _activeLevelCards.Count; i++)
            {
                _viewData.LevelData.Add(_activeLevelCards[i].GetLevelData());
            }

            return _viewData;
        }
    }
}
