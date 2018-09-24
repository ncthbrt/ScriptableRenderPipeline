namespace UnityEngine.Experimental.Rendering.LightweightRP
{
    public enum ShaderPathID
    {
        PhysicallyBased,
        SimpleLit,
        Unlit,
        TerrainPhysicallyBased,
        ParticlesPhysicallyBased,
        ParticlesSimpleLit,
        ParticlesUnlit,
        Count
    }

    public static class ShaderUtils
    {
        static readonly string[] s_ShaderPaths  =
        {
            "LightweightRenderPipeline/Lit",
            "LightweightRenderPipeline/Simple Lit",
            "LightweightRenderPipeline/Unlit",
            "LightweightRenderPipeline/Terrain/Lit",
            "LightweightRenderPipeline/Particles/Lit",
            "LightweightRenderPipeline/Particles/Simple Lit",
            "LightweightRenderPipeline/Particles/Unlit",
        };

        public static string GetShaderPath(ShaderPathID id)
        {
            int index = (int)id;
            if (index < 0 && index >= (int)ShaderPathID.Count)
            {
                Debug.LogError("Trying to access lightweight shader path out of bounds");
                return "";
            }

            return s_ShaderPaths[index];
        }
    }
}
