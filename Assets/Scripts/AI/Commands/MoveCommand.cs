using UnityEngine;

class MoveCommand : Command {
    private Vector3Int position;
    private Vector3Int size;
    private Vector3Int displacement;

    private GridCellData[,,] cut;

    private GridCellData[,,] paste;

    // Moves a region of the world by a displacement.
    // Syntax Descriptor: <position: p> <size: s> <displacement: d>
    // m <x> <y> <z> <sx> <sy> <sz> <dx> <dy> <dz>
    public static new readonly char SIGNATURE = 'm';
    public static new readonly int REQUIRED_PARAMS = 9;

    public MoveCommand(int[] argv) {
        position = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        displacement = new Vector3Int(argv[6], argv[7], argv[8]);
        valid = LangscapeError.CMD_VALID.code;

        if(IsInvalidPosition(position, size))
        {
            ErrorInterface.Instance.ThrowUserError(LangscapeError.CMD_POSITION_OUT_OF_WORLD);
            return;
        }
        if(IsInvalidDestination(position + displacement, size))
        {
            ErrorInterface.Instance.ThrowUserError(LangscapeError.CMD_DESTINATION_OUT_OF_WORLD);
            return;
        }
    }

    public override void Execute() {
        cut = GridMesh.Instance.Cut(position, size);
        paste = GridMesh.Instance.Paste(position + displacement, cut);
    }

    public override void Undo() {
        GridMesh.Instance.Set(position + displacement, paste);
        GridMesh.Instance.Set(position, cut);
    }
    public override void Redo() {
        GridMesh.Instance.Fill(position, size, new GridCellData(0, Color.white));
        GridMesh.Instance.Set(position + displacement, cut);
    }

    public override string ToString() {
        return SIGNATURE +
            " <" + position.x + " " + position.y + " " + position.z + "> " +
            "<" + size.x + " " + size.y + " " + size.z + "> " +
            "<" + displacement.x + " " + displacement.y + " " + displacement.z + "> ";
    }
}