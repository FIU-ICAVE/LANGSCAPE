using UnityEngine;

public class TeleportCommand : Command {
    private Vector3Int displacement;

    // Fills the selected region with a specific block.
    // Syntax Descriptor: <displacement: d>
    // t <dx> <dy> <dz>
    public static new readonly char SIGNATURE = 't';
    public static new readonly int REQUIRED_PARAMS = 3;

    public TeleportCommand(int[] argv) {
        displacement = new Vector3Int(argv[0], argv[1], argv[2]);
        valid = LangscapeError.CMD_VALID.code;
    }
    public override void Execute() {
        // Set player position

        //updating world stats
        WorldStateManager.Instance.UpdateTotalWorldObjects();
    }
    public override void Undo() {
        //undo player position
    }
    public override void Redo() {
        //redo player position
    }

    public override string ToString() {
        return SIGNATURE +
        " <" + displacement.x + " " + displacement.y + " " + displacement.z + ">";
    }
}
