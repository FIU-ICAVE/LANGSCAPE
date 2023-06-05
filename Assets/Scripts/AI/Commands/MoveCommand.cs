using System.Text;
using UnityEngine;

class MoveCommand : Command {
    private Vector3Int position0;
    private Vector3Int position1;
    private Vector3Int position2;

    private GridCellData[,,] cut;
    private GridCellData[,,] overwritten;

    // c <x0> <y0> <z0> <x1> <y1> <z1> <x2> <y2> <z2>
    public static new readonly char SIGNATURE = 'm';
    public static new readonly int PARAM_COUNT = 9;
    public static new readonly int REQUIRED_PARAMS = 9;

    public MoveCommand(int[] argv) {
        position0 = new Vector3Int(argv[0], argv[1], argv[2]);
        position1 = new Vector3Int(argv[3], argv[4], argv[5]);
        position2 = new Vector3Int(argv[6], argv[7], argv[8]);
    }

    public override void Execute() {
        cut = GridMesh.Instance.Cut(position0, position1);
        overwritten = GridMesh.Instance.Paste(position2, cut);
        GridMesh.Instance.RegenerateMesh();
    }

    public override void Undo() { }
    public override void Redo() { }

    public override string ToString() {
        return SIGNATURE +
            "<" + position0.x + " " + position0.y + " " + position0.z + "> " +
            "<" + position1.x + " " + position1.y + " " + position1.z + "> " +
            "<" + position2.x + " " + position2.y + " " + position2.z + "> ";
    }
}