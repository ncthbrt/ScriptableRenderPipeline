using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(EdgeDetectRenderer), PostProcessEvent.BeforeStack, "Custom/EdgeDetectNormals")]
public sealed class EdgeDetect : PostProcessEffectSettings
{
  public FloatParameter edgeExp = new FloatParameter { value = 1.05f };
  public FloatParameter sampleDist = new FloatParameter { value = 1.0f };
  public ColorParameter backgroundColor = new ColorParameter { value = Color.white };
  public ColorParameter foregroundColor = new ColorParameter { value = Color.white };
  public ColorParameter sobelColor = new ColorParameter { value = Color.black };
}

public sealed class EdgeDetectRenderer : PostProcessEffectRenderer<EdgeDetect>
{
  private Shader Shader = Shader.Find("PostProcessing/EdgeDetectNormals");
  public override DepthTextureMode GetCameraFlags()
  {
    return DepthTextureMode.Depth;
  }

  public override void Render(PostProcessRenderContext context)
  {
    var sheet = context.propertySheets.Get(Shader);
    sheet.properties.SetFloat("_SampleDistance", settings.sampleDist);
    sheet.properties.SetVector("_BgColor", settings.backgroundColor.value);
    sheet.properties.SetVector("_FgColor", settings.foregroundColor.value);
    sheet.properties.SetVector("_SobelColor", settings.sobelColor.value);
    sheet.properties.SetFloat("_Exponent", settings.edgeExp);
    context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
  }
}
