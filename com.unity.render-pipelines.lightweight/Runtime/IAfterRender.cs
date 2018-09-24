namespace UnityEngine.Experimental.Rendering.LightweightRP
{
    public interface IAfterRender
    {
        ScriptableRenderPass GetPassToEnqueue();
    }
}
