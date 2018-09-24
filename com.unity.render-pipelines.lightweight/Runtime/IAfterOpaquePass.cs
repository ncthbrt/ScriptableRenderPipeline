namespace UnityEngine.Experimental.Rendering.LightweightRP
{
    public interface IAfterOpaquePass
    {
        ScriptableRenderPass GetPassToEnqueue(
            RenderTextureDescriptor baseDescriptor,
            RenderTargetHandle colorAttachmentHandle,
            RenderTargetHandle depthAttachmentHandle);
    }
}
