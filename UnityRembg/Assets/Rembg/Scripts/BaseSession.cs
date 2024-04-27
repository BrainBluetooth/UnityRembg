using System;
using UnityEngine;

namespace BrainBluetooth.Rembg
{
    public abstract class BaseSession : MonoBehaviour
    {
        public abstract string GetName();

        public abstract void Predict(Texture input, RenderTexture output);

        private static float Max(Color c)
        {
            return Math.Max(c.r, Math.Max(c.g, c.b));
        }
        private static float Max(RenderTexture rt)
        {
            Color[] pixels;

            RenderTexture old = RenderTexture.active;
            RenderTexture.active = rt;
            {
                Texture2D tex = new Texture2D(rt.width, rt.height);
                try
                {
                    tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                    tex.Apply();
                    pixels = tex.GetPixels();
                }
                finally
                {
                    if (tex != null)
                        DestroyImmediate(tex);
                }
            }
            RenderTexture.active = old;

            Array.Sort(pixels, (Color a, Color b) => Max(a).CompareTo(Max(b)));
            return Max(pixels[pixels.Length - 1]);
        }
        public static void Normalize(Texture image, Vector3 mean, Vector3 std, RenderTexture output)
        {
            // check output(rt) is rgbafloat
            if (output.format != RenderTextureFormat.ARGBFloat)
                throw new ArgumentException("output's format is not ARGBFloat", nameof(output));

            Graphics.Blit(image, output);
            float max = Max(output);

            Material matNormalize = MaterialCache.Get("Hidden/Rembg/Normalize");
            matNormalize.SetFloat("_Max", max);
            matNormalize.SetVector("_Mean", mean);
            matNormalize.SetVector("_Std", std);
            Graphics.Blit(image, output, matNormalize);
        }
    }
}