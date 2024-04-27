using UnityEngine;
using UnityEngine.UI;

namespace BrainBluetooth.Rembg.Example
{
    [RequireComponent(typeof(RawImage))]
    internal class ImageDisplayer : MonoBehaviour
    {
        public Material postProcess;
        public RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
        private RawImage com;
        private RenderTexture rt;
        private Texture _texture;

        public Texture texture
        {
            get => _texture;
            set
            {
                this._texture = value;
                CreateRT(value);
                if (postProcess)
                    Graphics.Blit(value, rt, postProcess);
                else
                    Graphics.Blit(value, rt);
            }
        }

        private void CreateRT(Texture tex)
        {
            if (rt != null)
            {
                if (rt.width == tex.width && rt.height == tex.height)
                    return;
                DisposeRT();
            }
            rt = new RenderTexture(tex.width, tex.height, 0, format);
            rt.Create();

            Initialize();
            this.com.texture = rt;
        }

        private void DisposeRT()
        {
            if (rt == null) return;
            rt.Release();
            DestroyImmediate(rt);
            rt = null;
        }

        private bool hasInitialized = false;
        private void Initialize()
        {
            if (hasInitialized) return;
            this.com = base.GetComponent<RawImage>();
        }

        private void OnDestroy()
        {
            DisposeRT();
        }
    }
}