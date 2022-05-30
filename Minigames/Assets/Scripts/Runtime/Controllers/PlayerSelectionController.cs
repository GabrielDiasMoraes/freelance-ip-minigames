using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minigames
{
    public class PlayerSelectionController
    {
        //TODO: Move it to a Scriptable Object
        private List<PlayerData> _players;
        public UnityAction OnEnable { get; set; }
        public UnityAction OnDispatch { get; set; }
        public UnityAction OnPlayerListChange { get; set; }
        public List<PlayerData> Players => _players;

        public PlayerSelectionController()
        {
            _players = new List<PlayerData>();
        }

        public void EnableController()
        {
            OnEnable?.Invoke();
        }

        public void DispatchController()
        {
            OnDispatch?.Invoke();
        }

        public void AddNewPlayer()
        {
            int id = GetID();

            int count = _players.Count;

            PlayerData newPlayer = new PlayerData
            {
                ID = id,
                Name = $"Player {count+1}",
                Score = 0
            };

            _players.Add(newPlayer);
            OnPlayerListChange?.Invoke();
        }

        private int GetID()
        {
            //TODO: Save the generated IDs to not generate the same
            return Random.Range(0, 5);
        }

        public void RemovePlayer()
        {
            RemovePlayer(_players.Count - 1);
        }

        public void RemovePlayer(int index)
        {
            if (index >= _players.Count) return;

            _players.RemoveAt(index);
            OnPlayerListChange?.Invoke();
        }
    }

    public struct PlayerData
    {
        public int ID;
        public string Name;
        public int Score;
    }

}
