using System;
using Unity.Barracuda;
using UnityEngine;

namespace BrainBluetooth.Rembg
{
    public sealed class U2netpSession : BaseSession
    {
        public override string GetName() => "u2netp";

        [HideInInspector, SerializeField] private NNModel modelAsset;
        private Model m_RuntimeModel;
        private IWorker worker;

        private AutoRTDestroyer rtInput;
        private AutoRTDestroyer rtOutput;

        private void Start()
        {
            m_RuntimeModel = ModelLoader.Load(modelAsset);
            worker = WorkerFactory.CreateWorker(m_RuntimeModel);

            RenderTexture _rtInput = new RenderTexture(320, 320, 0, RenderTextureFormat.ARGBFloat, 0);
            _rtInput.Create();
            rtInput = new AutoRTDestroyer(_rtInput);

            RenderTexture _rtOutput = new RenderTexture(320, 320, 0, RenderTextureFormat.RFloat, 0);
            _rtOutput.Create();
            rtOutput = new AutoRTDestroyer(_rtOutput);
        }

        private void OnDestroy()
        {
            worker?.Dispose();
            rtInput?.Dispose();
            rtOutput?.Dispose();
        }

        public override void Predict(Texture inputImage, RenderTexture outputMask)
        {
            const string outputName = "1960";

            Normalize(inputImage,
                new Vector3(0.485f, 0.456f, 0.406f),
                new Vector3(0.229f, 0.224f, 0.225f),
                rtInput);

            float[] arr;
            using (Tensor input = new Tensor(rtInput, 3))
            {
                worker.Execute(input);
                Tensor output = worker.PeekOutput(outputName);
                output.ToRenderTexture(rtOutput);
                arr = output.ToReadOnlyArray();
                // output.Dispose(); // you don't need to call
            }

            Array.Sort(arr);
            Material matOutputPostprocess = MaterialCache.Get("Hidden/Rembg/Output-Postprocess");
            matOutputPostprocess.SetFloat("_Min", arr[0]);
            matOutputPostprocess.SetFloat("_Max", arr[arr.Length - 1]);
            Graphics.Blit(rtOutput, outputMask, matOutputPostprocess);
        }
    }
}