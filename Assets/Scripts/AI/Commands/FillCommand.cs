using System.Text;
using UnityEngine;

class FillCommand : Command {
    private Vector3Int position;
    private Vector3Int size;
    private GridCellData cell;

    public FillCommand(int[] argv) {
        position = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        cell = new GridCellData(argv[6], argv[7], argv[8], argv[9], argv[10]);
    }

    public override void Execute() {
        GridMesh.Instance.Fill(position, position + size, cell);
        GridMesh.Instance.RegenerateMesh();
    }

    public override void Undo() { }

    public override string ToString() {
        return Fill.signature +
            position.x + " " +
            position.y + " " +
            position.z + " " +
            size.x + " " +
            size.y + " " +
            size.z + " " +
            (int)cell.type + " " +
            (int)cell.texture + " " +
            (int)(cell.color.r * 255) + " " +
            (int)(cell.color.g * 255) + " " +
            (int)(cell.color.b * 255);
    }
}