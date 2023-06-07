using System.Text;
using UnityEngine;

class MoveCommand : Command {
    private Vector3Int position0;
    private Vector3Int size;
    private Vector3Int position2;

    private GridCellData[,,] cut;
    private Vector3Int cutStartOffset;

    private GridCellData[,,] paste;
    private Vector3Int pasteStartOffset;

    // c <x0> <y0> <z0> <x1> <y1> <z1> <x2> <y2> <z2>
    public static new readonly char SIGNATURE = 'm';
    public static new readonly int PARAM_COUNT = 9;
    public static new readonly int REQUIRED_PARAMS = 9;

    public MoveCommand(int[] argv) {
        position0 = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        position2 = position0 + new Vector3Int(argv[6], argv[7], argv[8]);
        valid = CODE_VALID;

        if (IsInvalidPosition(position0, size) || IsInvalidDestination(position2, size))
            return;
    }

    public override void Execute() {
        cut = GridMesh.Instance.Cut(position0, size);
        paste = GridMesh.Instance.Paste(position2, cut);
    }

    public override void Undo() { }
    public override void Redo() { }

    public override string ToString() {
        Vector3Int displacement = position2 - position0;
        return SIGNATURE +
            " <" + position0.x + " " + position0.y + " " + position0.z + "> " +
            "<" + size.x + " " + size.y + " " + size.z + "> " +
            "<" + displacement.x + " " + displacement.y + " " + displacement.z + "> ";
    }
}