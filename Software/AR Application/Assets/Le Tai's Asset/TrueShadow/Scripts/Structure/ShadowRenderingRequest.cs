using System;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.TrueShadow
{
public readonly struct ShadowRenderingRequest
{
    const int DIMENSIONS_HASH_STEP = 1;

    public readonly TrueShadow shadow;
    public readonly float      shadowSize;
    public readonly Vector2    shadowOffset;
    public readonly Vector2    dimensions;

    readonly int hash;

    public ShadowRenderingRequest(TrueShadow shadow)
    {
        this.shadow  = shadow;
        shadowSize   = shadow.Size;
        shadowOffset = shadow.Offset.Rotate(-shadow.RectTransform.eulerAngles.z);

        var canvas = shadow.Graphic.canvas;

        switch (shadow.Graphic)
        {
        case Text _:
            dimensions = shadow.SpriteMesh.bounds.size;
            break;
        default:
            dimensions = shadow.RectTransform.rect.size;
            break;
        }

        float casterScale;
        try
        {
            casterScale = canvas.scaleFactor;
        }
        catch (NullReferenceException)
        {
            // ?. chain will bypass GameObject lifetime check. Try catch is cleaner than 2 if
            casterScale = 1f;
        }

        dimensions *= casterScale;

        // Tiled type cannot be batched by similar size
        int dimensionHash = shadow.Graphic is Image image && image.type == Image.Type.Tiled
                                ? dimensions.GetHashCode()
                                : HashUtils.CombineHashCodes(
                                    Mathf.CeilToInt(dimensions.x / DIMENSIONS_HASH_STEP) * DIMENSIONS_HASH_STEP,
                                    Mathf.CeilToInt(dimensions.y / DIMENSIONS_HASH_STEP) * DIMENSIONS_HASH_STEP
                                );

        hash = HashUtils.CombineHashCodes(
            Mathf.CeilToInt(shadowSize * 100),
            dimensionHash,
            shadow.ContentHash
        );
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        return GetHashCode() == obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        return hash;
    }
}
}
