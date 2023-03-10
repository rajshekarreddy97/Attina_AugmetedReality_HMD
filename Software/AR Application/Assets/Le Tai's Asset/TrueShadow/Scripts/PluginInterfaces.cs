using UnityEngine;
using UnityEngine.UI;

namespace LeTai.TrueShadow.PluginInterfaces
{
public interface ITrueShadowCasterMaterialProvider
{
    Material GetTrueShadowCasterMaterial();
}

public interface ITrueShadowCasterMeshModifier
{
    void ModifyTrueShadowCasterMesh(Mesh mesh);
}

public interface ITrueShadowCasterMaterialPropertiesModifier
{
    void ModifyTrueShadowCasterMaterialProperties(MaterialPropertyBlock propertyBlock);
}

public interface ITrueShadowCasterClearColorProvider
{
    Color GetTrueShadowCasterClearColor();
}

public interface ITrueShadowRendererNormalMaterialProvider
{
    Material GetTrueShadowRendererNormalMaterial();
}

public interface ITrueShadowRendererMaterialModifier
{
    void ModifyTrueShadowRendererMaterial(Material baseMaterial);
}

public interface ITrueShadowRendererMeshModifier
{
    void ModifyTrueShadowRenderMesh(VertexHelper vertexHelper);
}
}
