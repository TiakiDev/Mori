using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TreePlacerWindow : EditorWindow
{
    private List<GameObject> treePrefabs = new List<GameObject>();
    private int numberOfTrees = 100;
    private Vector2 areaSize = new Vector2(50, 50);
    private Terrain terrain;
    private float yOffset = 0.1f;

    [MenuItem("Tools/Terrain Tree Placer")]
    public static void ShowWindow()
    {
        GetWindow<TreePlacerWindow>("Drzewa na Terenie");
    }

    void OnGUI()
    {
        GUILayout.Label("Konfiguracja Drzew", EditorStyles.boldLabel);

        // Automatyczne wyszukiwanie terenu
        if (terrain == null)
        {
            terrain = FindObjectOfType<Terrain>();
            if (terrain != null)
            {
                TerrainData data = terrain.terrainData;
                areaSize = new Vector2(data.size.x, data.size.z);
                Debug.Log("Znaleziono teren automatycznie!");
            }
        }

        terrain = (Terrain)EditorGUILayout.ObjectField("Teren", terrain, typeof(Terrain), true);

        // Lista prefabów drzew
        GUILayout.Label("Prefaby drzew:");
        for (int i = 0; i < treePrefabs.Count; i++)
        {
            treePrefabs[i] = (GameObject)EditorGUILayout.ObjectField(
                $"Drzewo {i + 1}", 
                treePrefabs[i], 
                typeof(GameObject), 
                false
            );
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Dodaj drzewo")) treePrefabs.Add(null);
        if (GUILayout.Button("Usuń ostatnie") && treePrefabs.Count > 0) treePrefabs.RemoveAt(treePrefabs.Count - 1);
        EditorGUILayout.EndHorizontal();

        // Parametry
        if (terrain != null)
        {
            TerrainData data = terrain.terrainData;
            GUILayout.Label($"Rozmiar terenu: {data.size.x}x{data.size.z}");
        }
        
        areaSize = EditorGUILayout.Vector2Field("Obszar generowania", areaSize);
        numberOfTrees = EditorGUILayout.IntField("Liczba drzew", numberOfTrees);
        yOffset = EditorGUILayout.FloatField("Offset Y", yOffset);

        if (GUILayout.Button("Umieść drzewa!")) PlaceTrees();
    }

    void PlaceTrees()
    {
        if (terrain == null)
        {
            Debug.LogError("Nie przypisano terenu!");
            return;
        }

        if (treePrefabs.Count == 0)
        {
            Debug.LogError("Brak prefabów drzew!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrainData.size;

        // Oblicz środek terenu
        Vector3 terrainCenter = new Vector3(
            terrainPos.x + terrainSize.x / 2,
            terrainPos.y,
            terrainPos.z + terrainSize.z / 2
        );

        Undo.RegisterCompleteObjectUndo(terrain.gameObject, "Place Trees");

        for (int i = 0; i < numberOfTrees; i++)
        {
            // Losowa pozycja względem środka terenu
            Vector3 localPos = new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0,
                Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );

            // Pozycja światowa z uwzględnieniem granic terenu
            Vector3 worldPos = terrainCenter + localPos;
            worldPos.x = Mathf.Clamp(worldPos.x, terrainPos.x, terrainPos.x + terrainSize.x);
            worldPos.z = Mathf.Clamp(worldPos.z, terrainPos.z, terrainPos.z + terrainSize.z);
            worldPos.y = terrain.SampleHeight(worldPos) + yOffset;

            // Losowe parametry drzewa
            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Count)];
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);

            // Utwórz instancję drzewa
            GameObject tree = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            tree.transform.position = worldPos;
            tree.transform.rotation = rotation;
            tree.transform.localScale = scale;
            tree.transform.parent = terrain.transform;

            Undo.RegisterCreatedObjectUndo(tree, "Create Tree");
        }

        Debug.Log($"Umieszczono {numberOfTrees} drzew!");
    }
}