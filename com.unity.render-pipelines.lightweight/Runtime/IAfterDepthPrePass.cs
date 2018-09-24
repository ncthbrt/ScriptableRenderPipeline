namespace UnityEngine.Experimental.Rendering.LightweightRP
{
    public interface IAfterDepthPrePass
    {
        ScriptableRenderPass GetPassToEnqueue(RenderTextureDescriptor baseDescriptor, RenderTargetHandle depthAttachmentHandle);
    }
}
