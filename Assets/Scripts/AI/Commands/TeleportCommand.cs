using UnityEngine;

public class TeleportCommand : Command {
    private Vector3Int displacement;
    private Vector3 originalPosition;
    private Vector3 finalPosition;

    // Fills the selected region with a specific block.
    // Syntax Descriptor: <displacement: d>
    // t <dx> <dy> <dz>
    public static new readonly char SIGNATURE = 't';
    public static new readonly int REQUIRED_PARAMS = 3;
    public Transform player;

    public TeleportCommand(Transform player, int[] argv) {
        this.player = player;
        displacement = new Vector3Int(argv[0], argv[1], argv[2]);
        originalPosition = player.position;
        finalPosition = originalPosition + displacement;
        valid = LangscapeError.CMD_VALID.code;
    }
    public override void Execute() {
        player.transform.position = finalPosition;
    }
    public override void Undo() {
        player.position = originalPosition;
    }
    public override void Redo() {
        player.position = finalPosition;
    }

    public override string ToString() {
        return SIGNATURE +
        " <" + displacement.x + " " + displacement.y + " " + displacement.z + ">";
    }
}
