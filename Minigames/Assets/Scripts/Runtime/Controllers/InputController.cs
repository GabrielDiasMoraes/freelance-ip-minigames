using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Minigames
{
    public class InputController : ITickable
    {

        private Dictionary<int, PointerData> _lastFramePointers;

        private UnityAction<PointerData[]> _onNewPointer;

        private UnityAction<PointerData[]> _onUpdatePointer;

        private UnityAction<PointerData[]> _onExitPointer;

        public InputController()
        {
            _lastFramePointers = new Dictionary<int, PointerData>();
        }

        public UnityAction<PointerData[]> OnNewPointer { get => _onNewPointer; set => _onNewPointer = value; }
        public UnityAction<PointerData[]> OnUpdatePointer { get => _onUpdatePointer; set => _onUpdatePointer = value; }
        public UnityAction<PointerData[]> OnExitPointer { get => _onExitPointer; set => _onExitPointer = value; }

        public void Tick()
        {
            List<PointerData> pointerDatas = GetPointers();

            List<PointerData> newPointers = new List<PointerData>();
            List<PointerData> removedPointers = new List<PointerData>();

            Dictionary<int, PointerData> framePointers = new Dictionary<int, PointerData>();

            List<int> oldIds = new List<int>();

            for (int i = 0; i < pointerDatas.Count; i++)
            {
                PointerData pointerData = pointerDatas[i];

                framePointers.TryAdd(pointerData.PointerID, pointerData);

                if (_lastFramePointers.ContainsKey(pointerData.PointerID))
                {
                    oldIds.Add(pointerData.PointerID);
                }
                else
                {
                    newPointers.Add(pointerData); 
                }
            }

            _onNewPointer?.Invoke(newPointers.ToArray());

            foreach (var keyVal in _lastFramePointers)
            {
                if (!oldIds.Contains(keyVal.Key))
                {
                    removedPointers.Add(keyVal.Value);
                }
            }

            _onExitPointer?.Invoke(removedPointers.ToArray());

            _onUpdatePointer?.Invoke(pointerDatas.ToArray());

            _lastFramePointers = framePointers;
        }

        private List<PointerData> GetPointers()
        {
            var pointerInputs = new List<PointerData>();
            if (Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                for (int i = 0; i < touches.Length; i++)
                {
                    Touch touch = touches[i];
                    var pointerData = new PointerData
                    {
                        PointerID = touch.fingerId,
                        PointerPosition = touch.position
                    };
                    pointerInputs.Add(pointerData);
                }
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                var pointerData = new PointerData
                {
                    PointerID = 999,
                    PointerPosition = Input.mousePosition
                };
                pointerInputs.Add(pointerData);
            }
            return pointerInputs;
        }
    }

    public struct PointerData
    {
        public int PointerID;
        public Vector2 PointerPosition;
    }



}
