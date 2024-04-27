using UnityEngine;

namespace BrainBluetooth.Rembg.Example
{
    internal sealed class RembgExample : MonoBehaviour
    {
        public Texture input;
        public U2netpSession remover;

        private RenderTexture mask;

        public ImageDisplayer inputDisplayer;
        public ImageDisplayer maskDisplayer;

        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
                Remove();
        }

        private void Remove()
        {
            inputDisplayer.texture = input;
            DisposeRT(ref mask);
            (mask = new RenderTexture(input.width, input.height, 0, RenderTextureFormat.ARGBFloat)).Create();
            remover.Predict((Texture2D)input, mask);
            maskDisplayer.texture = mask;
        }

        private static void DisposeRT(ref RenderTexture rt)
        {
            if (rt == null) return;

            rt.Release();
            DestroyImmediate(rt);
            rt = null;
        }

        private void OnDestroy()
        {
            DisposeRT(ref mask);
        }
    }
}