using UnityEngine;

namespace Minigames
{
    public abstract class ViewBase<T> : PoolableItem where T : IViewData
    {
        T _view;

        public abstract void CloseView();

        public abstract void OpenView();

        public abstract void UpdateView(T viewData);
    }
}