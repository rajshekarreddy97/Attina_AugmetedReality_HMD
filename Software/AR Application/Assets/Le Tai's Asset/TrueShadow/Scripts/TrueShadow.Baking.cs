#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.U2D;
using UnityEngine.UI;

#endif

namespace LeTai.TrueShadow
{
#if UNITY_EDITOR
class SpriteAtlasCache : AssetPostprocessor
{
    static readonly IndexedSet<SpriteAtlas> ATLASES = new IndexedSet<SpriteAtlas>();

    static SpriteAtlasCache()
    {
        var guids = AssetDatabase.FindAssets("t:SpriteAtlas");
        foreach (var guid in guids)
        {
            ATLASES.AddUnique(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AssetDatabase.GUIDToAssetPath(guid)));
        }
    }

    static readonly Object[] SHARED_ARRAY = new Object[1];

    static IEnumerable<SpriteAtlas> AtlasesOf(Sprite sprite)
    {
        foreach (var atlas in ATLASES)
        {
            var packables = atlas.GetPackables();
            if (!packables.Contains(sprite) && !packables.Contains(sprite.texture))
                continue;

            yield return atlas;
        }
    }

    internal static void AddToSameAtlas(Sprite newSprite, Sprite oldSprite)
    {
        foreach (var atlas in AtlasesOf(oldSprite))
        {
            if (atlas.GetPackables().Contains(newSprite))
                continue;

            SHARED_ARRAY[0] = newSprite;
            atlas.Add(SHARED_ARRAY);
        }
    }

    internal static void RemoveFromAllAtlas(Sprite sprite)
    {
        foreach (var atlas in AtlasesOf(sprite))
        {
            SHARED_ARRAY[0] = sprite;
            atlas.Remove(SHARED_ARRAY);
        }
    }

    static bool IsSpriteAtlasPath(string path)
    {
        return Path.GetExtension(path) == ".spriteatlas";
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
                                       string[] movedFromAssetPaths)
    {
        foreach (var imported in importedAssets)
        {
            if (IsSpriteAtlasPath(imported))
            {
                ATLASES.AddUnique(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(imported));
            }
        }

        foreach (var deleted in deletedAssets)
        {
            if (IsSpriteAtlasPath(deleted))
            {
                ATLASES.Remove(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(deleted));
            }
        }
    }
}
#endif
public partial class TrueShadow
{
    public const string BAKED_SHADOWS_PATH = "Assets/Baked True Shadows/";

    void BakeShadows()
    {
#if UNITY_EDITOR
        ShadowContainer container = null;
        ShadowFactory.Instance.Get(new ShadowRenderingRequest(this), ref container);
        var renderTexture = container.Texture;
        Debug.Assert(renderTexture);
        var texture = new Texture2D(renderTexture.width, renderTexture.height,
                                    TextureFormat.ARGB32, false);

        var activeTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = activeTexture;

        var textureName = $"{SceneManager.GetActiveScene().name} - {gameObject.name} @{size}.png";
        var texturePath = BAKED_SHADOWS_PATH + textureName;
        Directory.CreateDirectory(BAKED_SHADOWS_PATH);
        File.WriteAllBytes(texturePath, texture.EncodeToPNG());
        AssetDatabase.Refresh(ImportAssetOptions.DontDownloadFromCacheServer);

        var importer = (TextureImporter) AssetImporter.GetAtPath(texturePath);

        TextureImporterSettings importSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(importSettings);
        importSettings.textureType                        = TextureImporterType.Sprite;
        importSettings.spriteMeshType                     = SpriteMeshType.Tight;
        importSettings.spriteGenerateFallbackPhysicsShape = false;
        importSettings.alphaSource                        = TextureImporterAlphaSource.FromInput;
        importSettings.alphaIsTransparency                = true;
        importSettings.mipmapEnabled                      = false;
        importer.SetTextureSettings(importSettings);
        importer.SaveAndReimport();

        var bakedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
        bakedShadows.Add(bakedSprite);

        if (Graphic is Image image)
            SpriteAtlasCache.AddToSameAtlas(bakedSprite, image.sprite);

        shadowRenderer.SetSprite(bakedSprite);
#endif
    }

    void RemoveBakedShadow()
    {
#if UNITY_EDITOR
        foreach (var sprite in bakedShadows)
        {
            if (!sprite) continue;

            if (Graphic is Image)
                SpriteAtlasCache.RemoveFromAllAtlas(sprite);

            var path = AssetDatabase.GetAssetPath(sprite);
            AssetDatabase.DeleteAsset(path);
        }

        bakedShadows.Clear();
#endif
    }
}
}
