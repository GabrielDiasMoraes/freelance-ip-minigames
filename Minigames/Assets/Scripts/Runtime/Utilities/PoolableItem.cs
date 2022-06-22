using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    public class PoolableItem : MonoBehaviour
    {
        PoolUtility _pool;

        public void SetupPool(PoolUtility pool)
        {
            _pool = pool;
        }

        public virtual void Init()
        {
            gameObject.SetActive(true);
            enabled = true;
        }
        protected virtual void OnDisable()
        {
            if (_pool != null)
            {
                _pool.ReturnToPool(this);
            }
            gameObject.SetActive(false);
        }
    }
}
