using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridTexture {
    Default = 0,
    Grid = 1,
    Filter = 2,
    Hollow = 3,
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
