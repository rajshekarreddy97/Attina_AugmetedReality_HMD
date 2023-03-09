using UnityEngine;

namespace LeTai.TrueShadow
{
public class ShadowContainer
{
    public RenderTexture Texture { get; }

    public int RefCount { get; internal set; }

    public readonly int requestHash;

    public ShadowContainer(RenderTexture texture, ShadowRenderingRequest request)
    {
        Texture     = texture;
        RefCount    = 1;
        requestHash = request.GetHashCode();
    }
}
}
