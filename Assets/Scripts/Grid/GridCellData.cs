using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType { 
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

    public BlockType type;
    public Color color;

    public GridCellData(BlockType type, Color color) {
        this.type = type;
        this.color = color;
    }

    // By value of type
    private static readonly byte[] properties = new byte[]{
        PROPERTY_TRANSPARENT,                           // Empty
        PROPERTY_FULL_BLOCK,                            // Solid Block
        PROPERTY_TRANSPARENT | PROPERTY_FULL_BLOCK,     // Glass
    };

    public bool IsTransparent() => (properties[(int)type] & PROPERTY_TRANSPARENT) != 0;
    public bool IsFullBlock() => (properties[(int)type] & PROPERTY_TRANSPARENT) != 0;
}
