using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectZ.Code.Runtime.UI
{
    public abstract class ViewBase : MonoBehaviour
    {
        protected List<IDisposable> disposables = new List<IDisposable>();

        private void OnDestroy()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}