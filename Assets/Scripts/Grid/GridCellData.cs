using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum GridCellType { 
    Empty, 
    Block, 
    Glass 
}

[System.Serializable]
public struct GridCellData
{
    private const byte PROPERTY_NONE = 0;           // Indicates to render all sides
    private const byte PROPERTY_TRANSPARENT = 1;    // Indicates using alpha material and renders surrounding blocks
    private const byte PROPERTY_FULL_BLOCK = 2;     // Indicates to not render any faces between another full block

    public GridCellType type;
    public GridTexture texture;
    public Color color;

    public GridCellData(GridCellType type, GridTexture texture, Color color) {
        this.type = type;
        this.texture = texture;
        this.color = color;
    }

    public GridCellData(int type, int texture, int r, int g, int b) {
        this.type = (GridCellType)type;
        this.texture = (GridTexture)texture;
        this.color = new Color(r / 255.0f, g / 255.0f, b / 255.0f, 1);
        if (this.type == GridCellType.Glass)
            color.a = 0.4f;
    }

    // By value of type
    private static readonly byte[] properties = new byte[]{
        PROPERTY_TRANSPARENT,                           // Empty
        PROPERTY_FULL_BLOCK,                            // Solid Block
        PROPERTY_TRANSPARENT | PROPERTY_FULL_BLOCK,     // Glass
    };

    public bool IsTransparent() => (properties[(int)type] & PROPERTY_TRANSPARENT) != 0;
    public bool IsFullBlock() => (properties[(int)type] & PROPERTY_FULL_BLOCK) != 0;
}
