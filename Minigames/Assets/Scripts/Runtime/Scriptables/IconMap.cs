using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    [CreateAssetMenu(menuName = "Data/IconMap")]
    public class IconMap : ScriptableObject
    {
        [SerializeField]
        private KeyValuePair<int, Sprite>[] _playerIcons;


        public bool TryGetPlayerIcon(int id, out Sprite sprite)
        {
            for (int i = 0; i < _playerIcons.Length; i++)
            {
                var keyValuePair = _playerIcons[i];
                if (keyValuePair.Key == id)
                {
                    sprite = keyValuePair.Value;
                    return true;
                }
            }

            sprite = null;
            return false;
        }


    }

    [Serializable]
    struct KeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
    }



}
