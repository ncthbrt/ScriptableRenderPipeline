namespace UnityEngine.Experimental.Rendering.LightweightRP
{
    public interface IAfterSkyboxPass
    {
        ScriptableRenderPass GetPassToEnqueue(RenderTextureDescriptor baseDescriptor, RenderTargetHandle colorHandle, RenderTargetHandle depthHandle);
    }
}
