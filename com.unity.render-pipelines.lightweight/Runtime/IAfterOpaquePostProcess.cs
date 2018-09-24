namespace UnityEngine.Experimental.Rendering.LightweightRP
{
    public interface IAfterOpaquePostProcess
    {
        ScriptableRenderPass GetPassToEnqueue(RenderTextureDescriptor baseDescriptor, RenderTargetHandle colorHandle,
            RenderTargetHandle depthHandle);
    }
}
