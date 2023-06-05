using UnityEngine;

class FillCommand : Command {
    private Vector3Int position;
    private Vector3Int size;
    private GridCellData cell;

    // f <x> <y> <z> <sx> <sy> <sz> <block> [color]
    public static new readonly char SIGNATURE = 'f';
    public static new readonly int PARAM_COUNT = 8;
    public static new readonly int REQUIRED_PARAMS = 7;

    public FillCommand(int[] argv, Color color) {
        position = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        cell = new GridCellData(argv[6], color);
    }

    public override void Execute() {
        GridMesh.Instance.Fill(position, position + size, cell);
        GridMesh.Instance.RegenerateMesh();
    }

    public override void Undo() { }
    public override void Redo() { }

    public override string ToString() {
        return SIGNATURE +
            position.x + " " +
            position.y + " " +
            position.z + " " +
            size.x + " " +
            size.y + " " +
            size.z + " " +
            (int)cell.type + " " +
            ColorUtility.ToHtmlStringRGB(cell.color);
    }
}