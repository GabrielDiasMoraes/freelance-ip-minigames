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
        private Sprite[] _playerIcons;

        [SerializeField]
        private KeyValuePair<int, Sprite>[] _playerBackground;

        [SerializeField]
        private KeyValuePair<int, Sprite>[] _playerButtonBackground;

        public Sprite[] PlayerIcons => _playerIcons;

        public KeyValuePair<int, Sprite>[] PlayerBackground => _playerBackground;

        public KeyValuePair<int, Sprite>[] PlayerButtonBackground => _playerButtonBackground;

    }

    [Serializable]
    public struct KeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
    }

    public static class KeyValuePairUtility
    {
        public static bool TryGetValue<TKey, TValue>(this KeyValuePair<TKey, TValue>[] array, TKey key, out TValue value)
        {
            value = default;
            for (int i = 0; i < array.Length; i++)
            {
                var keyValuePair = array[i];
                if (keyValuePair.Key.Equals(key))
                {
                    value = keyValuePair.Value;
                    return true;
                }
            }

            return false;
        }
    }

}
