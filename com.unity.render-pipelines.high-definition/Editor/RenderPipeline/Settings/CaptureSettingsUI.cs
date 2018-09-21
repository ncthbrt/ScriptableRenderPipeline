using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    using CED = CoreEditorDrawer<CaptureSettingsUI, SerializedCaptureSettings>;

    partial class CaptureSettingsUI : BaseUI<SerializedFrameSettings>
    {
        const string captureSettingsHeaderContent = "Capture Settings";
        static readonly GUIContent clearColorModeContent = CoreEditorUtils.GetContent("Clear Mode");
        static readonly GUIContent backgroundColorHDRContent = CoreEditorUtils.GetContent("Background Color");
        static readonly GUIContent clearDepthContent = CoreEditorUtils.GetContent("Clear Depth");
        static readonly GUIContent renderingPathContent = CoreEditorUtils.GetContent("Rendering Path");
        static readonly GUIContent volumeLayerMaskContent = CoreEditorUtils.GetContent("Volume Layer Mask");
        static readonly GUIContent volumeAnchorOverrideContent = CoreEditorUtils.GetContent("Volume Anchor Override");
        static readonly GUIContent apertureContent = CoreEditorUtils.GetContent("Aperture");
        static readonly GUIContent shutterSpeedContent = CoreEditorUtils.GetContent("Shutter Speed");
        static readonly GUIContent isoContent = CoreEditorUtils.GetContent("Iso");
        static readonly GUIContent shadowDistanceContent = CoreEditorUtils.GetContent("Shadow Distance");
        static readonly GUIContent farClipPlaneContent = CoreEditorUtils.GetContent("Far Clip Plane");
        static readonly GUIContent nearClipPlaneContent = CoreEditorUtils.GetContent("Near Clip Plane");
        static readonly GUIContent fieldOfviewContent = CoreEditorUtils.GetContent("Field Of View");
        static readonly GUIContent useOcclusionCullingContent = CoreEditorUtils.GetContent("Occlusion Culling");
        static readonly GUIContent cullingMaskContent = CoreEditorUtils.GetContent("Culling Mask");
        
        public static CED.IDrawer SectionCaptureSettings(bool withOverride)
        {
            return CED.FoldoutGroup(
                captureSettingsHeaderContent,
                (s, p, o) => s.isSectionExpandedCaptureSettings,
                FoldoutOption.Indent,
                CED.LabelWidth(250, CED.Action((s, p, o) => Drawer_SectionCaptureSettings(s, p, o, withOverride))));
        }

        public AnimBool isSectionExpandedCaptureSettings { get { return m_AnimBools[0]; } }

        public CaptureSettingsUI()
            : base(1)
        {
        }

        static void Drawer_SectionCaptureSettings(CaptureSettingsUI s, SerializedCaptureSettings p, Editor owner, bool withOverride)
        {
            OverridableSettingsArea area = new OverridableSettingsArea(15);
            area.Add(p.clearColorMode, clearColorModeContent, () => p.overridesClearColorMode, a => p.overridesClearColorMode = a);
            area.Add(p.backgroundColorHDR, backgroundColorHDRContent, () => p.overridesBackgroundColorHDR, a => p.overridesBackgroundColorHDR = a);
            area.Add(p.clearDepth, clearDepthContent, () => p.overridesClearDepth, a => p.overridesClearDepth = a);
            area.Add(p.renderingPath, renderingPathContent, () => p.overridesRenderingPath, a => p.overridesRenderingPath = a);
            area.Add(p.volumeLayerMask, volumeLayerMaskContent, () => p.overridesVolumeLayerMask, a => p.overridesVolumeLayerMask = a);
            area.Add(p.volumeAnchorOverride, volumeAnchorOverrideContent, () => p.overridesVolumeAnchorOverride, a => p.overridesVolumeAnchorOverride = a);
            area.Add(p.aperture, apertureContent, () => p.overridesAperture, a => p.overridesAperture = a);
            area.Add(p.shutterSpeed, shutterSpeedContent, () => p.overridesShutterSpeed, a => p.overridesShutterSpeed = a);
            area.Add(p.iso, isoContent, () => p.overridesIso, a => p.overridesIso = a);
            area.Add(p.shadowDistance, shadowDistanceContent, () => p.overridesShadowDistance, a => p.overridesShadowDistance = a);
            area.Add(p.farClipPlane, farClipPlaneContent, () => p.overridesFarClip, a => p.overridesFarClip = a);
            area.Add(p.nearClipPlane, nearClipPlaneContent, () => p.overridesNearClip, a => p.overridesNearClip = a);
            area.Add(p.fieldOfview, fieldOfviewContent, () => p.overridesFieldOfview, a => p.overridesFieldOfview = a);
            area.Add(p.useOcclusionCulling, useOcclusionCullingContent, () => p.overridesUseOcclusionCulling, a => p.overridesUseOcclusionCulling = a);
            area.Add(p.cullingMask, cullingMaskContent, () => p.overridesCullingMask, a => p.overridesCullingMask = a);
            area.Draw(withOverride);
        }
    }
}
