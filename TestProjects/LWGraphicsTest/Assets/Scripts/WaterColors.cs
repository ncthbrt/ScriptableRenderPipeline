using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using System;

namespace WaterColors
{

  [Serializable]
  public class PaperData
  {
    public Texture paperTex;
    public float paperScale = 1f;
    public float paperPower = 1f;

  }


  [Serializable]
  public sealed class PaperParameter : ParameterOverride<PaperData>
  {
    public PaperParameter()
    {
      value = null;
    }

    public PaperParameter(PaperData value)
    {
      this.value = value;
    }

    public static bool IsValid(PaperParameter parameter)
    {
      return parameter != null && parameter.value != null && parameter.value.paperTex != null;
    }
  }


  [Serializable]
  [PostProcess(typeof(WaterColorRenderer), PostProcessEvent.AfterStack, "Custom/WaterColors")]
  public sealed class WaterColors : PostProcessEffectSettings
  {
    public TextureParameter wobbTex = new TextureParameter() { defaultState = TextureParameterDefault.White };
    public FloatParameter wobbScale = new FloatParameter { value = 1f };
    public FloatParameter wobbPower = new FloatParameter { value = 0.01f };
    public FloatParameter edgeSize = new FloatParameter { value = 1f };
    public FloatParameter edgePower = new FloatParameter { value = 3f };
    public PaperParameter paperParameter1 = new PaperParameter();
    public PaperParameter paperParameter2 = new PaperParameter();
    public PaperParameter paperParameter3 = new PaperParameter();
    // public PaperParameter[] paperDataset;
  }

  public sealed class WaterColorRenderer : PostProcessEffectRenderer<WaterColors>
  {
    private Shader Shader = Shader.Find("PostProcessing/WaterColorFilter");
    private const int PASS_WOBB = 0;
    private const int PASS_EDGE = 1;
    private const int PASS_PAPER = 2;
    private PaperData[] _paperData = new PaperData[3];
    private int _paperCount = 0;

    public override DepthTextureMode GetCameraFlags()
    {
      return DepthTextureMode.DepthNormals;
    }

    public override void Render(PostProcessRenderContext context)
    {
      var sheet = context.propertySheets.Get(Shader);
      if (settings.wobbTex.value != null)
      {
        sheet.properties.SetTexture("_WobbTex", settings.wobbTex);
      }
      sheet.properties.SetFloat("_WobbScale", settings.wobbScale);
      sheet.properties.SetFloat("_WobbPower", settings.wobbPower);
      sheet.properties.SetFloat("_EdgeSize", settings.edgeSize);
      sheet.properties.SetFloat("_EdgePower", settings.edgePower);
      var rt0 = RenderTexture.GetTemporary(context.screenWidth, context.screenHeight, 0, context.sourceFormat);
      var rt1 = RenderTexture.GetTemporary(context.screenWidth, context.screenHeight, 0, context.sourceFormat);

      context.command.BlitFullscreenTriangle(context.source, rt1, sheet, PASS_WOBB);
      Swap(ref rt0, ref rt1);
      UpdatePaperData();
      if (_paperCount > 0)
      {
        context.command.BlitFullscreenTriangle(rt0, rt1, sheet, PASS_EDGE);
        Swap(ref rt0, ref rt1);
        for (var i = 0; i < _paperCount; i++)
        {
          var d = _paperData[i];
          sheet.properties.SetFloat("_PaperPower", d.paperPower);
          sheet.properties.SetFloat("_PaperScale", d.paperScale);
          sheet.properties.SetTexture("_PaperTex", d.paperTex);
          context.command.BlitFullscreenTriangle(rt0, (i == (_paperCount - 1) ? context.destination : rt1), sheet, PASS_PAPER);
          Swap(ref rt0, ref rt1);
        }
      }
      else
      {
        context.command.BlitFullscreenTriangle(rt0, context.destination, sheet, PASS_EDGE);
      }
      // if (0 > 0)
      // {
      // context.command.BlitFullscreenTriangle(rt0, rt1, sheet, PASS_EDGE);
      // Swap(ref rt0, ref rt1);

      // for (var i = 0; i < nPapers; i++)
      // {
      //   paperDataset[i].SetProps(_filterMat);
      //   Graphics.Blit(rt0, (i == (nPapers - 1) ? dst : rt1), _filterMat, PASS_PAPER);
      //   Swap(ref rt0, ref rt1);
      // }
      // }
      // else
      // {
      // }
      RenderTexture.ReleaseTemporary(rt0);
      RenderTexture.ReleaseTemporary(rt1);

      // context.command.BlitFullscreenTriangle(context.destination, context.destination, sheet, 1);
    }

    private void Swap(ref RenderTexture src, ref RenderTexture dst)
    {
      var tmp = src;
      src = dst;
      dst = tmp;
    }

    private void UpdatePaperData()
    {
      _paperCount = 0;
      _paperCount += PaperParameter.IsValid(settings.paperParameter1) ? 1 : 0;
      _paperCount += PaperParameter.IsValid(settings.paperParameter2) ? 1 : 0;
      _paperCount += PaperParameter.IsValid(settings.paperParameter3) ? 1 : 0;
      var i = 0;
      if (PaperParameter.IsValid(settings.paperParameter1))
      {
        _paperData[i++] = settings.paperParameter1.value;
      }
      if (PaperParameter.IsValid(settings.paperParameter2))
      {
        _paperData[i++] = settings.paperParameter2.value;
      }
      if (PaperParameter.IsValid(settings.paperParameter3))
      {
        _paperData[i++] = settings.paperParameter3.value;
      }
    }
  }
}