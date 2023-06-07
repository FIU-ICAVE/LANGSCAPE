using UnityEngine;

class RotateCommand : Command {
    private Vector3Int position;
    private Vector3Int size;
    private Vector3Int destination;

    private GridCellData[,,] cut;
    private GridCellData[,,] rotated;
    private GridCellData[,,] paste;

    // r <x0> <y0> <z0> <sx> <sy> <sz>
    public static new readonly char SIGNATURE = 'r';
    public static new readonly int PARAM_COUNT = 6;
    public static new readonly int REQUIRED_PARAMS = 6;

    public RotateCommand(int[] argv) {
        position = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        valid = CODE_VALID;

        destination = new Vector3Int(position.x + (size.x - size.z) / 2, position.y, position.z + (size.z - size.x) / 2);

        if (IsInvalidPosition(position, size) || IsInvalidDestination(destination, new Vector3Int(size.z, size.y, size.x)))
            return;
    }

    public override void Execute() {
        cut = GridMesh.Instance.Cut(position, size);
        rotated = new GridCellData[size.z, size.y, size.x];
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    rotated[z, y, x] = cut[x, y, size.z - z - 1];
                }
            }
        }
        paste = GridMesh.Instance.Paste(destination, rotated);
    }

    public override void Undo() {
        GridMesh.Instance.Set(destination, paste);
        GridMesh.Instance.Set(position, cut);
    }
    public override void Redo() {
        GridMesh.Instance.Fill(position, size, new GridCellData(0, Color.white));
        GridMesh.Instance.Set(destination, rotated);
    }

    public override string ToString() {
        return SIGNATURE +
            " <" + position.x + " " + position.y + " " + position.z + "> " +
            "<" + size.x + " " + size.y + " " + size.z + "> ";
    }
}