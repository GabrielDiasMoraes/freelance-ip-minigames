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

        private void OnDisable()
        {
            _pool.ReturnToPool(this);
        }
    }
}
