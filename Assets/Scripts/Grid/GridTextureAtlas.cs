//
// Authors: Jose Gonzalez
//
// Description: Simply converts an enum to a texture atlas tile and returns
// the (x,y) of that enum from top left to right and bottom, as opposed to bottom
// left to right and up. Also returns the proper UVs for a rectangle.
//

using UnityEngine;

// Add the texture atlas to the project as Sprite and UI
// Modify in the world object the Texture Atlas Size
// Add texture name to GridTexture
// Add models to GridMesh.Awake()
// Add block name to GridCellType in GridCellData.cs
// Add block properties inside GridCellData.properties (KEEP TRACK OF THE INDEX)
// Add block texture associated with it in GridCellData.properties

public enum GridTexture {
    Default = 0,
    Grid = 1,
    Filter = 2,
    Dirt = 3,
    Grass = 4,
    Stone = 5,
    Orange_Flowers = 6,
    Thatch = 7,
    Slate_Roof = 8,
    Submerged_Sand = 9,
    Beach_Sand = 10,
    Volcanic_Rock = 11,
    Cobblestone = 12,
    Red_Brick = 13,
    Regrown_Grass = 14,
    Supported_Thatch = 15,
}

public class GridTextureAtlas
{
    private Vector2 tileSize = Vector2.one;
    private Vector2[] textureUVs;

    public GridTextureAtlas(int tileCount_x, int tileCount_y) {
        tileSize.x = 1.0f / tileCount_x;
        tileSize.y = 1.0f / tileCount_y;

        textureUVs = new Vector2[tileCount_x * tileCount_y];

        int i = 0;
        for (int y = tileCount_y - 1; y >= 0; y--) {
            for (int x = 0; x < tileCount_x; x++) {
                textureUVs[i++] = new Vector2(x, y);
            }
        }
    }

    public Vector2[] GetTextureUVs(Vector2 coordsUV) {
        Vector2 uv0 = new Vector2(tileSize.x * coordsUV.x, tileSize.y * coordsUV.y);
        Vector2 uv1 = uv0 + tileSize;

        return new Vector2[6] {
            uv0,
            new Vector2(uv0.x, uv0.y + tileSize.y),
            uv1,
            uv0,
            uv1,
            new Vector2(uv0.x + tileSize.x, uv0.y)
        };
    }

    public Vector2[] GetTextureUVs(GridTexture texture) => GetTextureUVs(textureUVs[(int)texture]);
}
