using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Minigames
{
    public class PoolUtility
    {
        private readonly Stack<PoolableItem> _poolStack;
        private readonly PoolableItem _referenceItem;
        private readonly Transform _parent;
        private readonly DiContainer _diContainer;

        public PoolUtility(PoolableItem referenceItem, int amount = 0, Transform parent = null, DiContainer diContainer = null)
        {
            _poolStack = new Stack<PoolableItem>();
            _referenceItem = referenceItem;
            this._parent = parent;
            _diContainer = diContainer;
            for (int i = 0; i < amount; i++)
            {
                _poolStack.Push(Instantiate());
            }
        }

        private PoolableItem Instantiate()
        {
            PoolableItem newItem = null;
            if (_diContainer == null)
            {
                newItem = GameObject.Instantiate(_referenceItem, _parent);
            }
            else
            {
                newItem = _diContainer.InstantiatePrefabForComponent<PoolableItem>(_referenceItem, _parent);
            }
            newItem.SetupPool(this);
            return newItem;
        }

        public PoolableItem GetFromPool()
        {
            if (!_poolStack.TryPop(out var item))
            {
                item = Instantiate();
            }
            item.Init();
            item.transform.SetParent(_parent);
            return item;
        }

        public T GetFromPool<T>() where T : PoolableItem
        {
            return (T)GetFromPool();
        }

        public void ReturnToPool(PoolableItem item)
        {
            _poolStack.Push(item);
        }

    }
}
