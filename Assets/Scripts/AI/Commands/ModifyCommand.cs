using UnityEngine;

class ModifyCommand : Command {
    private Vector3Int position;
    private Vector3Int size;
    private GridCellData target;
    private GridCellData cell;

    private GridCellData[,,] replaced;
    private GridCellData[,,] modification;

    // Fills the selected region with a specific block.
    // Syntax Descriptor: <position: p> <size: s> <block: 0-4> [color: hexadecimal]
    // c <x> <y> <z> <sx> <sy> <sz> <target> <block> [color]
    public static new readonly char SIGNATURE = 'c';
    public static new readonly int REQUIRED_PARAMS = 8;

    public ModifyCommand(int[] argv, Color color) {
        position = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        target = new GridCellData(argv[6], Color.white);
        cell = new GridCellData(argv[7], color);
        valid = CODE_VALID;

        if (IsInvalidPosition(position, size))
            return;
    }
    public override void Execute() {
        replaced = GridMesh.Instance.Replace(position, size, cell, target, out modification);
    }
    public override void Undo() {
        GridMesh.Instance.Set(position, replaced);
    }
    public override void Redo() {
        GridMesh.Instance.Set(position, modification);
    }

    public override string ToString() {
        return SIGNATURE +
            " <" + position.x + " " + position.y + " " + position.z + "> " +
            "<" + size.x + " " + size.y + " " + size.z + "> " +
            "<" + (int)target.type + "> " +
            "<" + (int)cell.type + "> " +
            "<" + ColorUtility.ToHtmlStringRGB(cell.color) + ">";
    }
}