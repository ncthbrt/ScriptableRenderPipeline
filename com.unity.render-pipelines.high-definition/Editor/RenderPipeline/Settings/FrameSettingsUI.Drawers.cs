using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    using CED = CoreEditorDrawer<FrameSettingsUI, SerializedFrameSettings>;

    partial class FrameSettingsUI
    {
        internal static CED.IDrawer Inspector(bool withOverride = true, bool withXR = true)
        {
            return CED.Group(
                    SectionRenderingPasses(withOverride),
                    SectionRenderingSettings(withOverride),
                    CED.FadeGroup(
                        (s, d, o, i) => new AnimBool(withXR),
                        FadeOption.None,
                        SectionXRSettings(withOverride)),
                    SectionLightingSettings(withOverride),
                    CED.Select(
                        (s, d, o) => s.lightLoopSettings,
                        (s, d, o) => d.lightLoopSettings,
                        LightLoopSettingsUI.SectionLightLoopSettings(withOverride)
                        )
                    );
        }

        public static CED.IDrawer SectionRenderingPasses(bool withOverride)
        {
            return CED.FoldoutGroup(
                renderingPassesHeaderContent,
                (s, p, o) => s.isSectionExpandedRenderingPasses,
                FoldoutOption.Indent,
                CED.LabelWidth(200, CED.Action((s, p, o) => Drawer_SectionRenderingPasses(s, p, o, withOverride))),
                CED.space
                );
        }


        public static CED.IDrawer SectionRenderingSettings(bool withOverride)
        {
            return CED.FoldoutGroup(
                renderingSettingsHeaderContent,
                (s, p, o) => s.isSectionExpandedRenderingSettings,
                FoldoutOption.Indent,
                CED.LabelWidth(300, CED.Action((s, p, o) => Drawer_SectionRenderingSettings(s, p, o, withOverride))),
                CED.space
                );
        }

        public static CED.IDrawer SectionXRSettings(bool withOverride)
        {
            return CED.FadeGroup(
                (s, d, o, i) => s.isSectionExpandedXRSupported,
                FadeOption.None,
                CED.FoldoutGroup(
                    xrSettingsHeaderContent,
                    (s, p, o) => s.isSectionExpandedXRSettings,
                    FoldoutOption.Indent,
                    CED.LabelWidth(200, CED.Action((s, p, o) => Drawer_FieldStereoEnabled(s, p, o, withOverride))),
                    CED.space));
        }

        public static CED.IDrawer SectionLightingSettings(bool withOverride)
        {
            return CED.FoldoutGroup(
                lightSettingsHeaderContent,
                (s, p, o) => s.isSectionExpandedLightingSettings,
                FoldoutOption.Indent,
                CED.LabelWidth(250, CED.Action((s, p, o) => Drawer_SectionLightingSettings(s, p, o, withOverride))),
                CED.space);
        }

        static void Drawer_SectionRenderingPasses(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            OverridableSettingsArea area = new OverridableSettingsArea(8);
            area.Add(p.enableTransparentPrepass, transparentPrepassContent, () => p.overridesTransparentPrepass, a => p.overridesTransparentPrepass = a);
            area.Add(p.enableTransparentPostpass, transparentPostpassContent, () => p.overridesTransparentPostpass, a => p.overridesTransparentPostpass = a);
            area.Add(p.enableMotionVectors, motionVectorContent, () => p.overridesMotionVectors, a => p.overridesMotionVectors = a, () => hdrpSettings.supportMotionVectors);
            area.Add(p.enableObjectMotionVectors, objectMotionVectorsContent, () => p.overridesObjectMotionVectors, a => p.overridesObjectMotionVectors = a, () => hdrpSettings.supportMotionVectors);
            area.Add(p.enableDecals, decalsContent, () => p.overridesDecals, a => p.overridesDecals = a, () => hdrpSettings.supportDecals);
            area.Add(p.enableRoughRefraction, roughRefractionContent, () => p.overridesRoughRefraction, a => p.overridesRoughRefraction = a);
            area.Add(p.enableDistortion, distortionContent, () => p.overridesDistortion, a => p.overridesDistortion = a);
            area.Add(p.enablePostprocess, postprocessContent, () => p.overridesPostprocess, a => p.overridesPostprocess = a);
            area.Draw(withOverride);
        }

        static void Drawer_SectionRenderingSettings(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            OverridableSettingsArea area = new OverridableSettingsArea(6);
            area.Add(p.enableForwardRenderingOnly, forwardRenderingOnlyContent, () => p.overridesForwardRenderingOnly, a => p.overridesForwardRenderingOnly = a, () => !GL.wireframe && !hdrpSettings.supportOnlyForward, defaultValue: GL.wireframe || hdrpSettings.supportOnlyForward);
            if (s.isSectionExpandedUseForwardOnly.target)
            {
                area.Add(p.enableDepthPrepassWithDeferredRendering, depthPrepassWithDeferredRenderingContent, () => p.overridesDepthPrepassWithDeferredRendering, a => p.overridesDepthPrepassWithDeferredRendering = a);
            }
            area.Add(p.enableAsyncCompute, asyncComputeContent, () => p.overridesAsyncCompute, a => p.overridesAsyncCompute = a, () => SystemInfo.supportsAsyncCompute, defaultValue: SystemInfo.supportsAsyncCompute);
            area.Add(p.enableOpaqueObjects, opaqueObjectsContent, () => p.overridesOpaqueObjects, a => p.overridesOpaqueObjects = a);
            area.Add(p.enableTransparentObjects, transparentObjectsContent, () => p.overridesTransparentObjects, a => p.overridesTransparentObjects = a);
            area.Add(p.enableMSAA, msaaContent, () => p.overridesMSAA, a => p.overridesMSAA = a, () => hdrpSettings.supportMSAA, defaultValue: hdrpSettings.supportMSAA);
            area.Draw(withOverride);
        }
        
        static void Drawer_SectionLightingSettings(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            RenderPipelineSettings hdrpSettings = (GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset).renderPipelineSettings;
            OverridableSettingsArea area = new OverridableSettingsArea(10);
            area.Add(p.enableShadow, shadowContent, () => p.overridesShadow, a => p.overridesShadow = a);
            area.Add(p.enableContactShadow, contactShadowContent, () => p.overridesContactShadow, a => p.overridesContactShadow = a);
            area.Add(p.enableShadowMask, shadowMaskContent, () => p.overridesShadowMask, a => p.overridesShadowMask = a, () => hdrpSettings.supportShadowMask, defaultValue: hdrpSettings.supportShadowMask);
            area.Add(p.enableSSR, ssrContent, () => p.overridesSSR, a => p.overridesSSR = a, () => hdrpSettings.supportSSR, defaultValue: hdrpSettings.supportSSR);
            area.Add(p.enableSSAO, ssaoContent, () => p.overridesSSAO, a => p.overridesSSAO = a, () => hdrpSettings.supportSSAO, defaultValue: hdrpSettings.supportSSAO);
            area.Add(p.enableSubsurfaceScattering, subsurfaceScatteringContent, () => p.overridesSubsurfaceScattering, a => p.overridesSubsurfaceScattering = a, () => hdrpSettings.supportSubsurfaceScattering, defaultValue: hdrpSettings.supportSubsurfaceScattering);
            area.Add(p.enableTransmission, transmissionContent, () => p.overridesTransmission, a => p.overridesTransmission = a);
            area.Add(p.enableAtmosphericScattering, atmosphericScatteringContent, () => p.overridesAtmosphericScaterring, a => p.overridesAtmosphericScaterring = a);
            area.Add(p.enableVolumetrics, volumetricContent, () => p.overridesVolumetrics, a => p.overridesVolumetrics = a, () => hdrpSettings.supportVolumetrics && p.enableAtmosphericScattering.boolValue, defaultValue: hdrpSettings.supportVolumetrics && p.enableAtmosphericScattering.boolValue, indent: 1);
            area.Add(p.enableReprojectionForVolumetrics, reprojectionForVolumetricsContent, () => p.overridesProjectionForVolumetrics, a => p.overridesProjectionForVolumetrics = a, () => hdrpSettings.supportVolumetrics && p.enableAtmosphericScattering.boolValue, defaultValue: hdrpSettings.supportVolumetrics && p.enableAtmosphericScattering.boolValue, indent: 1);
            area.Add(p.enableLightLayers, lightLayerContent, () => p.overridesLightLayers, a => p.overridesLightLayers = a, () => hdrpSettings.supportLightLayers, defaultValue: hdrpSettings.supportLightLayers);
            area.Draw(withOverride);
        }

        static void Drawer_FieldStereoEnabled(FrameSettingsUI s, SerializedFrameSettings p, Editor owner, bool withOverride)
        {
            OverridableSettingsArea area = new OverridableSettingsArea(1);
            area.Add(p.enableStereo, stereoContent, () => p.overridesStereo, a => p.overridesStereo = a, null);
            area.Add(p.xrGraphicsConfig, xrGraphicConfigContent, () => p.overridesXrGraphicSettings, a => p.overridesXrGraphicSettings = a, () => XRGraphicsConfig.enabled);
            area.Draw(withOverride);
        }
    }
}
