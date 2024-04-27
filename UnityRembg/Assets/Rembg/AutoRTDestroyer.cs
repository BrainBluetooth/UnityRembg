using System;
using UnityEngine;

namespace BrainBluetooth.Rembg
{
    public sealed class AutoRTDestroyer : IDisposable
    {
        private readonly RenderTexture _item;
        private bool _disposed = false;

        public AutoRTDestroyer(RenderTexture renderTexture)
        {
            this._item = renderTexture ?? throw new ArgumentNullException(nameof(renderTexture));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_item != null)
            {
                _item.Release();
                UnityEngine.Object.DestroyImmediate(_item);
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~AutoRTDestroyer()
        {
            Dispose();
        }

        public RenderTexture renderTexture
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(AutoRTDestroyer));

                return _item;
            }
        }

        public static implicit operator RenderTexture(AutoRTDestroyer item)
            => item.renderTexture;
    }
}