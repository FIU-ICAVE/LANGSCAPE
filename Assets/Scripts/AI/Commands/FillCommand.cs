using UnityEngine;
using UnityEngine.UIElements;

class FillCommand : Command {
    private Vector3Int position0;
    private Vector3Int position1;
    private GridCellData cell;

    // f <x> <y> <z> <sx> <sy> <sz> <block> [color]
    public static new readonly char SIGNATURE = 'f';
    public static new readonly int PARAM_COUNT = 8;
    public static new readonly int REQUIRED_PARAMS = 7;

    public FillCommand(int[] argv, Color color) {
        position0 = new Vector3Int(argv[0], argv[1], argv[2]);
        position1 = position0 + new Vector3Int(argv[3], argv[4], argv[5]);
        cell = new GridCellData(argv[6], color);
    }

    public override void Execute() {
        GridMesh.Instance.Fill(position0, position1, cell);
    }

    public override void Undo() { }
    public override void Redo() { }

    public override string ToString() {
        Vector3Int size = position1 - position0;
        return SIGNATURE +
            " <" + position0.x + " " + position0.y + " " + position0.z + "> " +
            "<" + size.x + " " + size.y + " " + size.z + "> " +
            "<" + (int)cell.type + "> " +
            "<" + ColorUtility.ToHtmlStringRGB(cell.color) + ">";
    }
}