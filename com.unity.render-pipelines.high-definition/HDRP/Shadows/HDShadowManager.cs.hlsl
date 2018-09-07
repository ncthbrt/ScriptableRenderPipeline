//
// This file was automatically generated. Please don't edit by hand.
//

#ifndef HDSHADOWMANAGER_CS_HLSL
#define HDSHADOWMANAGER_CS_HLSL
//
// UnityEngine.Experimental.Rendering.HDPipeline.HDShadowFlag:  static fields
//
#define HDSHADOWFLAG_SAMPLE_BIAS_SCALE (1)
#define HDSHADOWFLAG_EDGE_LEAK_FIXUP (2)
#define HDSHADOWFLAG_EDGE_TOLERANCE_NORMAL (4)

// Generated from UnityEngine.Experimental.Rendering.HDPipeline.HDShadowData
// PackingRules = Exact
struct HDShadowData
{
    float4x4 viewProjection;
    float2 atlasOffset;
    float4 zBufferParam;
    float4 shadowMapSize;
    float4 viewBias;
    float3 normalBias;
    int flags;
    float4 shadowFilterParams0;
    float4x4 shadowToWorld;
};

// Generated from UnityEngine.Experimental.Rendering.HDPipeline.HDDirectionalShadowData
// PackingRules = Exact
struct HDDirectionalShadowData
{
    float4 sphereCascades[4];
    float4 cascadeDirection;
    float cascadeBorders[4];
};

//
// Accessors for UnityEngine.Experimental.Rendering.HDPipeline.HDShadowData
//
float4x4 GetViewProjection(HDShadowData value)
{
    return value.viewProjection;
}
float2 GetAtlasOffset(HDShadowData value)
{
    return value.atlasOffset;
}
float4 GetZBufferParam(HDShadowData value)
{
    return value.zBufferParam;
}
float4 GetShadowMapSize(HDShadowData value)
{
    return value.shadowMapSize;
}
float4 GetViewBias(HDShadowData value)
{
    return value.viewBias;
}
float3 GetNormalBias(HDShadowData value)
{
    return value.normalBias;
}
int GetFlags(HDShadowData value)
{
    return value.flags;
}
float4 GetShadowFilterParams0(HDShadowData value)
{
    return value.shadowFilterParams0;
}
float4x4 GetShadowToWorld(HDShadowData value)
{
    return value.shadowToWorld;
}

//
// Accessors for UnityEngine.Experimental.Rendering.HDPipeline.HDDirectionalShadowData
//
float4 GetSphereCascades(HDDirectionalShadowData value, int index)
{
    return value.sphereCascades[index];
}
float4 GetCascadeDirection(HDDirectionalShadowData value)
{
    return value.cascadeDirection;
}
float GetCascadeBorders(HDDirectionalShadowData value, int index)
{
    return value.cascadeBorders[index];
}


#endif
