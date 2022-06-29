using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace Minigames
{
    public class ResultScreenView : ViewBase<ResultScreenViewData>
    {

        [SerializeField] private Transform _playerResultRoot;
        [SerializeField] private RankElement _rankingElementPrefab;
        [SerializeField] private Button _contineButton;

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
            IOrderedEnumerable<PlayerResultInfo> orderedList = playerResultInfos.OrderByDescending(item => item.PlayerScore);

            int rank = 1;
            foreach (var playerResultInfo in orderedList)
            {
                RankElement rankElement = _rankElementPool.GetFromPool<RankElement>();
                rankElement.Configure(rank, playerResultInfo.PlayerIcon, playerResultInfo.PlayerName, playerResultInfo.PlayerScore);
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
