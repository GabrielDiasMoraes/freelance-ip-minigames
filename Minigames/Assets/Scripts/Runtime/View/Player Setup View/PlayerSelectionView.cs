using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class PlayerSelectionView : ViewBase<PlayerSelectionViewData>
    {
        [SerializeField] private IconCard _playerCardPrefab;
        [SerializeField] private LevelInfoView _levelInfoCardPrefab;
        [Header("Main View")]
        [SerializeField] private GameObject _mainViewRoot;
        [SerializeField] private RectTransform _playerListRoot;
        [SerializeField] private RectTransform _playerListContentRoot;
        [SerializeField] private TextMeshProUGUI _playerCount;
        [SerializeField] private TextMeshProUGUI _playerInfoIconText;
        [SerializeField] private GameObject _buttonsRoot;
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

        private PoolUtility _iconCardPool;
        private PoolUtility _levelPool;
        private List<IconCard> _activeIconCards;
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
            switch (_viewData.ViewType)
            {
                case PlayerSelectionViewType.PlayerQuantityView:
                    _mainViewRoot.SetActive(true);
                    _configViewRoot.SetActive(false);
                    _buttonsRoot.SetActive(true);
                    _playerListRoot.gameObject.SetActive(false);
                    ConfigureMainButtons();
                    _playerCount.text = _viewData.PlayersIds.Length.ToString();
                    break;
                case PlayerSelectionViewType.PlayerIconSelectView:
                    _mainViewRoot.SetActive(true);
                    _configViewRoot.SetActive(false);
                    _buttonsRoot.SetActive(false);
                    _playerListRoot.gameObject.SetActive(true);
                    float contentWidht = Mathf.Max(400 * viewData.PlayersIds.Length, _playerListRoot.sizeDelta.x);
                    _playerListContentRoot.sizeDelta = new Vector2(contentWidht, 300);
                    _playerInfoIconText.text = $"<nobr>Jogador {_viewData.CurrentPlayerIndex + 1} Toque e escolha o seu avatar!";
                    ConfigureIconCards();
                    UpdateArrowIndicator();
                    ConfigureMainButtons();
                    break;
                case PlayerSelectionViewType.ConfigView:
                    _mainViewRoot.SetActive(false);
                    _configViewRoot.SetActive(true);
                    ConfigureConfigButtons();
                    ConfigureMaxPlayerText();
                    ConfigurePlayerScoreTime();
                    ConfigureCorrectResultTime();
                    ConfigureLevelCards();
                    break;
            }
            if (_viewData.ViewType == PlayerSelectionViewType.PlayerQuantityView)
            {
                
                return;
            }
            
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
            _iconCardPool = new PoolUtility(_playerCardPrefab, 5, _playerListContentRoot);
            _levelPool = new PoolUtility(_levelInfoCardPrefab, 3, _levelListRoot);
            _activeIconCards = new List<IconCard>();
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

        private void ConfigureIconCards()
        {
            ClearCards(_activeIconCards);
            var iconList = _viewData.IconList;
            for (int i = 0; i < iconList.Length; i++)
            {
                var iconSprite = iconList[i];
                IconCard iconCard = (IconCard) _iconCardPool.GetFromPool();
                iconCard.Configure(iconSprite, i, _viewData.CurrentPlayerColor, OnIconClicked);
                iconCard.gameObject.SetActive(true);
                iconCard.transform.SetSiblingIndex(i);
                _activeIconCards.Add(iconCard);
            }
        }

        private void OnIconClicked(int iconIndex)
        {
            _viewData.SelectedIcon = iconIndex;
            UpdateArrowIndicator();
        }
        
        public int GetSelectedIcon()
        {
            return _viewData.SelectedIcon;
        }

        private void UpdateArrowIndicator()
        {
            for (int i = 0; i < _activeIconCards.Count; i++)
            {
                _activeIconCards[i].EnableArrow(_viewData.SelectedIcon == i);
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

            _minusButton.interactable = _viewData.PlayersIds.Length > 1;

            _plusButton.interactable = _viewData.PlayersIds.Length < _viewData.MaxPlayers;
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
