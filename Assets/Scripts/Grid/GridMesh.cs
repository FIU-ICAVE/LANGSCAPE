using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class GridMesh : MonoBehaviour
{
    // For global reference
    public static GridMesh Instance = null;

    [Header("Configure")]
    [SerializeField] private Vector3Int initialWorldSize;
    [SerializeField] private Vector2Int textureAtlasSize;
    [SerializeField] private bool drawInEditor = true;

    [Header("Test Placement")]
    [SerializeField] private GridCellData testCellData;
    [SerializeField] private Vector3Int testPosition;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh worldMesh;

    private GridCellData[,,] data;
    private List<Vector3> vertices;
    private List<int> opaqueTriangles;
    private List<int> transparentTriangles;
    private List<Color> colors;
    private List<Vector2> uvs;

    public Vector3Int size { get; private set; }

    private GridModel[] models;
    private GridTextureAtlas textureAtlas;


    // Start is called before the first frame update
    private void Awake()
    {
        drawInEditor = false;
        meshRenderer = GetComponent<MeshRenderer>();

        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }

        size = initialWorldSize;
        data = new GridCellData[size.x, size.y, size.z];

        Instance = this;

        models = new GridModel[3];
        models[(int)GridCellType.Block] = GridModel.Load("block");
        if (models[(int)GridCellType.Block] == null) {
            Debug.LogError("Model Loader: Failed to load block model.");
        }
        models[(int)GridCellType.Glass] = models[(int)GridCellType.Block];

        textureAtlas = new GridTextureAtlas(textureAtlasSize.x, textureAtlasSize.y);

        vertices = new List<Vector3>();
        opaqueTriangles = new List<int>();
        transparentTriangles = new List<int>();
        colors = new List<Color>();
        uvs = new List<Vector2>();

        meshFilter = GetComponent<MeshFilter>();

        worldMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = worldMesh;

        RegenerateMesh();
    }

    public void RegenerateMesh() {
        vertices.Clear();
        opaqueTriangles.Clear();
        transparentTriangles.Clear();
        colors.Clear();
        uvs.Clear();

        GridCellData floorCell = new GridCellData(GridCellType.Block, GridTexture.Grid, Color.white);

        for (int y = 0; y < size.y; y++) {
            for (int x = 0; x < size.x; x++) {
                for (int z = 0; z < size.z; z++) {
                    GridCellData cell = data[x, y, z];
                    if (y == 0 && cell.IsTransparent()) {
                        AddFace(floorCell, models[1].vertices, models[1].top, new Vector3Int(x, -1, z));
                    }

                    if (cell.type == GridCellType.Empty)
                        continue;

                    GridModel model = models[(int)cell.type];
                    Debug.Assert(model != null, "Model missing: " + ((int)cell.type).ToString());
                    Vector3Int position = new Vector3Int(x, y, z);
                    bool isPartiallyFull = !cell.IsFullBlock();

                    if (x == 0 || isPartiallyFull || CheckNeighbor(cell, data[x - 1, y, z]))
                        AddFace(cell, model.vertices, model.left, position);
                    if (x == size.x - 1 || isPartiallyFull || CheckNeighbor(cell, data[x + 1, y, z]))
                        AddFace(cell, model.vertices, model.right, position);
                    if (y == 0 || isPartiallyFull || CheckNeighbor(cell, data[x, y - 1, z]))
                        AddFace(cell, model.vertices, model.bottom, position);
                    if (y == size.y - 1 || isPartiallyFull || CheckNeighbor(cell, data[x, y + 1, z]))
                        AddFace(cell, model.vertices, model.top, position);
                    if (z == 0 || isPartiallyFull || CheckNeighbor(cell, data[x, y, z - 1]))
                        AddFace(cell, model.vertices, model.front, position);
                    if (z == size.z - 1 || isPartiallyFull || CheckNeighbor(cell, data[x, y, z + 1]))
                        AddFace(cell, model.vertices, model.back, position);
                }
            }
        }


        worldMesh.Clear();
        worldMesh.subMeshCount = 2;
        worldMesh.vertices = vertices.ToArray();
        worldMesh.SetTriangles(opaqueTriangles, 0);
        worldMesh.SetTriangles(transparentTriangles, 1);
        worldMesh.colors = colors.ToArray();
        worldMesh.SetUVs(0, uvs);
        worldMesh.RecalculateNormals();
    }

    // Check if neighbor is transparent or if there is a gap between the neighbor and cell.
    public bool CheckNeighbor(GridCellData cell, GridCellData neighbor) => !neighbor.IsFullBlock() || neighbor.IsTransparent() && !cell.IsTransparent();

    public static void TestPlacement() => Instance.Place(Instance.testCellData, Instance.testPosition);

    // Reduce the number of vertices needed, and therefore reduce the number of colors.
    private void AddFace(GridCellData cell, Vector3[] verts, int[] indices, Vector3Int position) {
        int v = vertices.Count;
        int count = indices.Length;
        if (cell.IsTransparent()) {
            for (int t = 0; t < count; t++) {
                vertices.Add(verts[indices[t]] + position);
                transparentTriangles.Add(v + t);
                colors.Add(cell.color);
                uvs.Add(Vector2.zero);
            }
            return;
        }
        // For non-transparent objects
        for (int t = 0; t < count; t++) {
            vertices.Add(verts[indices[t]] + position);
            opaqueTriangles.Add(v + t);
            colors.Add(cell.color);
        }
        uvs.AddRange(textureAtlas.GetTextureUVs(cell.texture));
    }

    public void Place(GridCellData cell, Vector3Int position) {
        data[position.x, position.y, position.z] = cell;

        RegenerateMesh();
    }

    public void Multiplace(CommandObject[] objects) {
        for (int i = 0; i < objects.Length; i++) {
            CommandObject obj = objects[i];
            Debug.Log(i);
            if (obj.original_position.x >= 0 && obj.original_position.x < size.x &&
                obj.original_position.y >= 0 && obj.original_position.y < size.y &&
                obj.original_position.z >= 0 && obj.original_position.z < size.z)
                data[obj.original_position.x, obj.original_position.y, obj.original_position.z].type = GridCellType.Empty;
            // Out of bounds into the negative
            if (obj.new_position.x < 0 || obj.new_position.y < 0 || obj.new_position.z < 0)
                continue;
            // Greater than the size of the world
            if (obj.new_position.x >= size.x || obj.new_position.y >= size.y || obj.new_position.z >= size.z)
                continue;
            // Set the cell to the new type
            data[obj.new_position.x, obj.new_position.y, obj.new_position.z].type = obj.type;
            data[obj.new_position.x, obj.new_position.y, obj.new_position.z].color = obj.color;
        }

        RegenerateMesh();
    }

    // Draws the outline of the world in the inspector
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (drawInEditor) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(initialWorldSize.x / 2.0f, initialWorldSize.y / 2.0f, initialWorldSize.z / 2.0f), initialWorldSize);
            Handles.DrawAAConvexPolygon(new Vector3[] {
                Vector3.zero, 
                new Vector3(initialWorldSize.x, 0, 0), 
                new Vector3(initialWorldSize.x, 0, initialWorldSize.z), 
                new Vector3(0, 0, initialWorldSize.z) 
            });
        }
    }
#endif
}
