using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

namespace Minigames
{
    public class ResultScreenView : ViewBase<ResultScreenViewData>
    {

        [SerializeField] private RectTransform _playerResultRoot;
        [SerializeField] private RankElement _rankingElementPrefab;
        [SerializeField] private Button _contineButton;
        [SerializeField] Camera _gameViewCamera;
        [SerializeField] private RectTransform _context;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;

        private PoolUtility _rankElementPool;
        private List<RankElement> _spawnedElements;

#if UNITY_EDITOR
        [SerializeField] private ResultScreenViewData MockData;

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
            _rankElementPool = new PoolUtility(_rankingElementPrefab, 3, _playerResultRoot);
            _spawnedElements = new List<RankElement>();
        }

        public override void OpenView()
        {
            gameObject.SetActive(true);
        }

        public override void CloseView()
        {
            gameObject.SetActive(false);
        }

        public override void UpdateView(ResultScreenViewData viewData)
        {
            _contineButton.onClick.RemoveAllListeners();
            _contineButton.onClick.AddListener(viewData.ContinueAction);
            ClearElements();
            ConfigureElements(viewData.Results);
            _context.rotation = new Quaternion(0, 0, viewData.CanvasRotationZ, 0);
            _gameViewCamera.rect = viewData.CameraData;
            StartCoroutine(ConfigureCellSize(viewData));
            
        }

        private IEnumerator ConfigureCellSize(ResultScreenViewData viewData)
        {
            yield return new WaitForEndOfFrame();
            float cellHeight = (_playerResultRoot.rect.height / viewData.PlayerCount) - 20;
            _gridLayoutGroup.cellSize = new Vector2(_playerResultRoot.rect.width, cellHeight);
        }

        private void ClearElements()
        {
            for (int i = _spawnedElements.Count; i > 0 ; i--)
            {
                _spawnedElements[i-1].enabled = false;
            }
            _spawnedElements.Clear();
        }

        private void ConfigureElements(PlayerResultInfo[] playerResultInfos)
        {
            playerResultInfos = playerResultInfos.OrderBy(item => item.PlayerSpentTime).ToArray();
            IOrderedEnumerable<PlayerResultInfo> orderedList = playerResultInfos.OrderByDescending(item => item.PlayerScore);

            int rank = 1;
            foreach (var playerResultInfo in orderedList)
            {
                RankElement rankElement = _rankElementPool.GetFromPool<RankElement>();
                rankElement.Configure(rank, playerResultInfo.PlayerIcon, playerResultInfo.PlayerName, playerResultInfo.PlayerScore, playerResultInfo.PlayerSpentTime);
                _spawnedElements.Add(rankElement);
                rank++;
            }

            for (int i = 0; i < _spawnedElements.Count; i++)
            {
                _spawnedElements[i].Reorder();
            }

        }


    }
}
