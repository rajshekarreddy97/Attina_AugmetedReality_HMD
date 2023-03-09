using System;
using UnityEngine;

namespace LeTai.TrueShadow
{
public enum BlendMode
{
    Normal,
    Additive,
    Multiply,
}

public static class BlendModeExtensions
{
    internal static Material materialAdditive;
    internal static Material materialMultiply;

    public static Material GetMaterial(this BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Normal:
                return null; // should use graphic.materialForRendering
            case BlendMode.Additive:
                if (!materialAdditive) materialAdditive = new Material(Shader.Find("UI/TrueShadow-Additive"));
                return materialAdditive;
            case BlendMode.Multiply:
                if (!materialMultiply) materialMultiply = new Material(Shader.Find("UI/TrueShadow-Multiply"));
                return materialMultiply;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
}
