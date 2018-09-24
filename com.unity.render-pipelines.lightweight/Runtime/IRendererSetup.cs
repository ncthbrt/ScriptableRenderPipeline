namespace UnityEngine.Experimental.Rendering.LightweightRP
{
    public interface IRendererSetup
    {
        void Setup(ScriptableRenderer renderer, ref RenderingData renderingData);
    }
}
