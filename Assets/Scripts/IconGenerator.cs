using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
#endif

public class IconGenerator : MonoBehaviour
{
    [Header("Settings")]
    public GameObject targetPrefab;
    public string savePath = "Assets/Art/Icons";
    public string iconName = "item_icon";
    public int resolution = 512;
    public Color backgroundColor = new Color(0, 0, 0, 0);

    [Header("Camera")]
    public float size = 1.5f;
    public Vector3 cameraOffset = new Vector3(0, 0, -2);
    public LayerMask renderLayer;

    private Camera renderCamera;
    private GameObject currentModel;

#if UNITY_EDITOR
    [ContextMenu("Generate Icon")]
    public void GenerateIcon()
    {
        if (targetPrefab == null)
        {
            Debug.LogError("Przypisz prefab!");
            return;
        }

        SetupScene();
        RenderIcon();
        Cleanup();
    }

    private void SetupScene()
    {
        GameObject cameraGO = new GameObject("RenderCamera");
        renderCamera = cameraGO.AddComponent<Camera>();
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = backgroundColor;
        renderCamera.cullingMask = renderLayer;
        renderCamera.orthographic = true;
        renderCamera.orthographicSize = size;

        currentModel = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
        currentModel.layer = LayerMask.NameToLayer("IconRender");

        cameraGO.transform.position = currentModel.transform.position + cameraOffset;
        cameraGO.transform.LookAt(currentModel.transform);
    }

    private void RenderIcon()
    {
        RenderTexture renderTexture = null;
        RenderTexture previousActive = RenderTexture.active;
        
        try
        {
            // Utwórz RenderTexture
            renderTexture = new RenderTexture(resolution, resolution, 24);
            renderCamera.targetTexture = renderTexture;
            renderCamera.Render();

            // Zapisz do Texture2D
            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            texture.Apply();

            // Zapisz jako PNG
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string fullPath = Path.Combine(savePath, $"{iconName}.png");
            File.WriteAllBytes(fullPath, texture.EncodeToPNG());
            AssetDatabase.Refresh();
            ConfigureSpriteImportSettings(fullPath);
        }
        finally
        {
            // Przywróć poprzedni stan i wyczyść
            RenderTexture.active = previousActive;
            renderCamera.targetTexture = null;
            
            if (renderTexture != null)
            {
                renderTexture.Release();
                DestroyImmediate(renderTexture);
            }
        }
    }

    private void ConfigureSpriteImportSettings(string assetPath)
    {
        assetPath = assetPath.Replace("\\", "/");
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.spritePivot = new Vector2(0.5f, 0.5f);
            importer.SaveAndReimport();
        }
    }

    private void Cleanup()
    {
        if (renderCamera != null) DestroyImmediate(renderCamera.gameObject);
        if (currentModel != null) DestroyImmediate(currentModel);
    }
#endif
}