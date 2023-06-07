using UnityEngine;

class FillCommand : Command {
    private Vector3Int position;
    private Vector3Int size;
    private GridCellData cell;

    private GridCellData[,,] replace;

    // f <x> <y> <z> <sx> <sy> <sz> <block> [color]
    public static new readonly char SIGNATURE = 'f';
    public static new readonly int PARAM_COUNT = 8;
    public static new readonly int REQUIRED_PARAMS = 7;

    public FillCommand(int[] argv, Color color) {
        position = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        cell = new GridCellData(argv[6], color);
        valid = CODE_VALID;

        if (IsInvalidPosition(position, size))
            return;
    }
    public override void Execute() {
        replace = GridMesh.Instance.Replace(position, size, cell);
    }
    public override void Undo() {
        GridMesh.Instance.Set(position, replace);
    }
    public override void Redo() { 
        GridMesh.Instance.Fill(position, size, cell);
    }

    public override string ToString() {
        return SIGNATURE +
            " <" + position.x + " " + position.y + " " + position.z + "> " +
            "<" + size.x + " " + size.y + " " + size.z + "> " +
            "<" + (int)cell.type + "> " +
            "<" + ColorUtility.ToHtmlStringRGB(cell.color) + ">";
    }
}