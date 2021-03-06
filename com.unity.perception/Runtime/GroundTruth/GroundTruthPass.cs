#if HDRP_PRESENT

using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEngine.Perception.GroundTruth
{
    public abstract class GroundTruthPass : CustomPass, IGroundTruthGenerator
    {
        public Camera targetCamera;

        bool m_IsActivated;
        public abstract void SetupMaterialProperties(MaterialPropertyBlock mpb, Renderer meshRenderer, Labeling labeling, uint instanceId);

        protected GroundTruthPass(Camera targetCamera)
        {
            this.targetCamera = targetCamera;
        }

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            if (targetCamera == null)
                throw new InvalidOperationException("targetCamera may not be null");

            // If we are forced to activate here we will get zeroes in the first frame.
            EnsureActivated();

            targetColorBuffer = TargetBuffer.Custom;
            targetDepthBuffer = TargetBuffer.Custom;
        }

        protected sealed override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
        {
            // CustomPasses are executed for each camera. We only want to run for the target camera
            if (hdCamera.camera != targetCamera)
                return;

            ExecutePass(renderContext, cmd, hdCamera, cullingResult);
        }

        protected abstract void ExecutePass(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult);

        protected void EnsureActivated()
        {
            if (!m_IsActivated)
            {
                LabeledObjectsManager.singleton.Activate(this);
                m_IsActivated = true;
            }
        }

        protected override void Cleanup()
        {
            LabeledObjectsManager.singleton.Deactivate(this);
        }
    }
}
#endif
