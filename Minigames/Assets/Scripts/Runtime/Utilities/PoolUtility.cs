using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    public class PoolUtility
    {
        private readonly Stack<PoolableItem> _poolStack;
        private readonly PoolableItem _referenceItem;
        private readonly Transform parent;

        public PoolUtility(PoolableItem referenceItem, int amount = 0, Transform parent = null)
        {
            _poolStack = new Stack<PoolableItem>();
            _referenceItem = referenceItem;
            this.parent = parent;
            for (int i = 0; i < amount; i++)
            {
                _poolStack.Push(Instantiate());
            }
        }

        private PoolableItem Instantiate()
        {
            var newPoolItem = Object.Instantiate(_referenceItem, parent);
            newPoolItem.SetupPool(this);
            return newPoolItem;
        }

        public PoolableItem GetFromPool()
        {
            if(_poolStack.TryPop(out var item))
            {
                return item;
            }
            return Instantiate();
        }

        public void ReturnToPool(PoolableItem item)
        {
            _poolStack.Push(item);
        }

    }
}
