//
// Authors: Jose Gonzalez
//
// Description: Using a single mesh and an array of cell data, this script is able
// to generate a mesh and modify it at runtime. It removes uneccessary cell faces
// and applies textures using only two different materials: a solid one which is
// rendered first, and a transparent one which is rendered second. Transparent objects
// need to be rendered second to prevent alpha sorting issues. This script loads the models
//

using System.Collections.Generic;
using UnityEngine;

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
#if UNITY_EDITOR
    [SerializeField] private bool drawInEditor = true;

    [Header("Test Commands")]
    [TextArea(1, 20)]
    [SerializeField] private string testCommands;
    [SerializeField] private int testOutputCode;
#endif

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
#if UNITY_EDITOR
        drawInEditor = false;
#endif
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }

        size = initialWorldSize;
        data = new GridCellData[size.x, size.y, size.z];

        Instance = this;

        // TODO: Move this into GridModel.cs
        models = new GridModel[(int)GridCellType.TYPE_MAX];
        models[(int)GridCellType.Block] = GridModel.Load("block");
        if (models[(int)GridCellType.Block] == null) {
            Debug.LogError("Model Loader: Failed to load block model.");
        }
        models[(int)GridCellType.Glass] = models[(int)GridCellType.Block];
        models[(int)GridCellType.Outline] = models[(int)GridCellType.Block];
        models[(int)GridCellType.Filter] = models[(int)GridCellType.Block];

        textureAtlas = new GridTextureAtlas(textureAtlasSize.x, textureAtlasSize.y);

        vertices = new List<Vector3>();
        opaqueTriangles = new List<int>();
        transparentTriangles = new List<int>();
        colors = new List<Color>();
        uvs = new List<Vector2>();

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

        int xMax = size.x - 1;
        int yMax = size.y - 1;
        int zMax = size.z - 1;

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
                    if (x == xMax || isPartiallyFull || CheckNeighbor(cell, data[x + 1, y, z]))
                        AddFace(cell, model.vertices, model.right, position);
                    if (y == 0 || isPartiallyFull || CheckNeighbor(cell, data[x, y - 1, z]))
                        AddFace(cell, model.vertices, model.bottom, position);
                    if (y == yMax || isPartiallyFull || CheckNeighbor(cell, data[x, y + 1, z]))
                        AddFace(cell, model.vertices, model.top, position);
                    if (z == 0 || isPartiallyFull || CheckNeighbor(cell, data[x, y, z - 1]))
                        AddFace(cell, model.vertices, model.front, position);
                    if (z == zMax || isPartiallyFull || CheckNeighbor(cell, data[x, y, z + 1]))
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

#if UNITY_EDITOR
    public static void TestCommandViaEditor() {
        Command[] cmds;
        Instance.testOutputCode = CommandParser.Parse(Instance.testCommands, '\n', ' ', out cmds);
        
        if (Instance.testOutputCode == 0) {
            foreach (Command cmd in cmds)
                cmd.Execute();
        }
    }
#endif

    // TODO: Reduce the number of vertices needed, and therefore reduce the number of colors. 6 to 4 is possible.
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

    // Export the overwritten data
    public void Set(Vector3Int pos, GridCellData cell) => data[pos.x, pos.y, pos.z] = cell;

    // Export the overwritten data
    public void Fill(Vector3Int pos0, Vector3Int pos1, GridCellData cell) {
        for (int x = pos0.x; x < pos1.x; x++) {
            for (int y = pos0.y; y < pos1.y; y++) {
                for (int z = pos0.z; z < pos1.z; z++) {
                    if (x >= 0 && x < size.x && y >= 0 && y < size.y && z >= 0 && z < size.z)
                        data[x, y, z] = cell;
                }
            }
        }
    }

    public GridCellData[,,] Cut(Vector3Int position0, Vector3Int position1) {
        int startX = position0.x, endX = position1.x;
        int startY = position0.y, endY = position1.y;
        int startZ = position0.z, endZ = position1.z;

        // Establish start and end positions
        if (position1.x < position0.x) {
            startX = position1.x;
            endX = position0.x;
        }
        if (position1.y < position0.y) {
            startY = position1.y;
            endY = position0.y;
        }
        if (position1.z < position0.z) {
            startZ = position1.z;
            endZ = position0.z;
        }

        // Clip content outside
        if (endX < 0 || endY < 0 || endZ < 0)
            return null;

        // Clip content outside
        if (startX >= size.x && startY >= size.y || startZ >= size.z)
            return null;

        // Cut upper bound to grid end
        if (endX >= size.x)
            endX = size.x;
        if (endY >= size.y)
            endY = size.y;
        if (endZ >= size.z)
            endZ = size.z;

        // Cut lower bound to grid start
        if (startX < 0)
            startX = 0;
        if (startY < 0)
            startY = 0;
        if (startZ < 0)
            startZ = 0;

        endX += 1;
        endY += 1;
        endZ += 1;

        GridCellData[,,] copy = new GridCellData[endX - startX, endY - startY, endZ - startZ];

        for (int x = startX; x < endX; x++) {
            for (int y = startY; y < endY; y++) {
                for (int z = startZ; z < endZ; z++) {
                    copy[x - startX, y - startY, z - startZ] = data[x, y, z];
                    data[x, y, z].type = GridCellType.Empty;
                }
            }
        }

        return data;
    }

    public GridCellData[,,] Copy(Vector3Int position0, Vector3Int position1) {
        int startX = position0.x, endX = position1.x;
        int startY = position0.y, endY = position1.y;
        int startZ = position0.z, endZ = position1.z;

        // Establish start and end positions
        if (position1.x < position0.x) {
            startX = position1.x;
            endX = position0.x;
        }
        if (position1.y < position0.y) {
            startY = position1.y;
            endY = position0.y;
        }
        if (position1.z < position0.z) {
            startZ = position1.z;
            endZ = position0.z;
        }

        // Clip content outside
        if (endX < 0 || endY < 0 || endZ < 0)
            return null;

        // Clip content outside
        if (startX >= size.x && startY >= size.y || startZ >= size.z)
            return null;

        // Cut upper bound to grid end
        if (endX >= size.x)
            endX = size.x;
        if (endY >= size.y)
            endY = size.y;
        if (endZ >= size.z)
            endZ = size.z;

        // Cut lower bound to grid start
        if (startX < 0)
            startX = 0;
        if (startY < 0)
            startY = 0;
        if (startZ < 0)
            startZ = 0;

        endX += 1;
        endY += 1;
        endZ += 1;

        GridCellData[,,] copy = new GridCellData[endX - startX, endY - startY, endZ - startZ];

        for (int x = startX; x < endX; x++) {
            for (int y = startY; y < endY; y++) {
                for (int z = startZ; z < endZ; z++) {
                    copy[x - startX, y - startY, z - startZ] = data[x, y, z];
                }
            }
        }

        return data;
    }

    public GridCellData[,,] Paste(Vector3Int position, GridCellData[,,] copy) {
        if (copy == null)
            return null;
        if (position.x >= size.x || position.y >= size.y || position.z >= size.z)
            return null;

        // The length of the copy array
        int lengthX = copy.GetLength(0);
        int lengthY = copy.GetLength(1);
        int lengthZ = copy.GetLength(2);

        // The end of the area in worldspace
        int endX = position.x + lengthX;
        int endY = position.y + lengthY;
        int endZ = position.z + lengthZ;

        // Find if the area is inside the grid
        if (endX - 1 < 0 || endY - 1 < 0 || endZ - 1 < 0)
            return null;

        // The starting index of the copy array
        int indexX = 0;
        int indexY = 0;
        int indexZ = 0;

        // Clip the area outside the grid.
        if (position.x < 0) {
            indexX = -position.x;
        }
        if (position.y < 0) {
            indexY = -position.y;
        }
        if (position.z < 0) {
            indexZ = -position.z;
        }
        if (endX > size.x) {
            lengthX -= endX - size.x;
        }
        if (endY > size.y) {
            lengthY -= endY - size.y;
        }
        if (endZ > size.z) {
            lengthZ -= endZ - size.z;
        }

        GridCellData[,,] overwrite = new GridCellData[lengthX - indexX, lengthY - indexY, lengthZ - indexZ];

        for (int x = 0; x < lengthX; x++) { 
            for (int y = 0; y < lengthY; y++) {
                for (int z = 0; z < lengthZ; z++) {
                    overwrite[x - indexX, y - indexY, z - indexZ] = data[position.x + x, position.y + y, position.z + z];
                    data[position.x + x, position.y + y, position.z + z] = copy[x, y, z];
                }
            }
        }

        return overwrite;
    }

    public static string GetDimensions() => "" + Instance.size.x + "x" + Instance.size.y + "x" + Instance.size.z;

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
