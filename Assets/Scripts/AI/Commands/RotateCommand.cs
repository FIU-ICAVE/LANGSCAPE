using UnityEngine;

class RotateCommand : Command {
    private Vector3Int position;
    private Vector3Int size;
    private Vector3Int destination;
    private int rotation;

    private GridCellData[,,] cut;
    private GridCellData[,,] rotated;
    private GridCellData[,,] paste;

    public const int ROTATE_CCW_0 = 0;
    public const int ROTATE_CCW_90 = 1;
    public const int ROTATE_CCW_180 = 2;
    public const int ROTATE_CCW_270 = 3;

    // Rotates the selected region of the worldspace by multiples of 90 degrees counterclockwise.
    // Syntax Description: <position: p> <size: s> <rotation: 0-3>
    // r <px> <py> <pz> <sx> <sy> <sz> <rotation>
    public static new readonly char SIGNATURE = 'r';
    public static new readonly int PARAM_COUNT = 7;
    public static new readonly int REQUIRED_PARAMS = 7;

    public RotateCommand(int[] argv) {
        position = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        rotation = argv[6];
        valid = CODE_VALID;

        destination = (rotation == ROTATE_CCW_0 || rotation == ROTATE_CCW_180) ? position : new Vector3Int(position.x + (size.x - size.z) / 2, position.y, position.z + (size.z - size.x) / 2);

        if (IsInvalidPosition(position, size) || IsInvalidDestination(destination, (rotation == ROTATE_CCW_0 || rotation == ROTATE_CCW_180) ? size : new Vector3Int(size.z, size.y, size.x)))
            return;
        if (rotation < ROTATE_CCW_0 || rotation > ROTATE_CCW_270) {
            valid = CODE_INVALID_VALUE;
            return;
        }
    }

    public override void Execute() {
        cut = GridMesh.Instance.Cut(position, size);
        rotated = RotateCounterClockwise(cut, rotation);
        paste = GridMesh.Instance.Paste(destination, rotated);
    }

    public static GridCellData[,,] RotateCounterClockwise(GridCellData[,,] data, int rotation) {
        Vector3Int size = new Vector3Int(data.GetLength(0), data.GetLength(1), data.GetLength(2));
        if (rotation == ROTATE_CCW_90) {
            GridCellData[,,] rotated = new GridCellData[size.z, size.y, size.x];
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    for (int z = 0; z < size.z; z++) {
                        rotated[z, y, x] = data[x, y, size.z - z - 1];
                    }
                }
            }
            return rotated;
        }

        if (rotation == ROTATE_CCW_270) {
            GridCellData[,,] rotated = new GridCellData[size.z, size.y, size.x];
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    for (int z = 0; z < size.z; z++) {
                        rotated[z, y, x] = data[size.x - x - 1, y, z];
                    }
                }
            }
            return rotated;
        }

        if (rotation == ROTATE_CCW_180) {
            GridCellData[,,] rotated = new GridCellData[size.x, size.y, size.y];
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    for (int z = 0; z < size.z; z++) {
                        rotated[x, y, z] = data[size.x - x - 1, y, size.z - z - 1];
                    }
                }
            }
            return rotated;
        }

        return data;
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