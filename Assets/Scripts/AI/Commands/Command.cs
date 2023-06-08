using UnityEngine;

public abstract class Command {
    public static readonly int CODE_VALID = 0;
    public static readonly int CODE_UNINITIALIZED = 1;
    public static readonly int CODE_POSITION_OUT_OF_WORLD = 2;
    public static readonly int CODE_DESTINATION_OUT_OF_WORLD = 3;
    public static readonly int CODE_INVALID_BLOCK = 4;
    public static readonly int CODE_INVALID_VALUE = 4;

    // Override in other commands
    public static readonly char SIGNATURE = 'n';
    public static readonly int REQUIRED_PARAMS = 0;

    public int valid = CODE_UNINITIALIZED;

    public static bool TryBuildArgs(string[] args, int count, ref int[] argv, ref int i) {
        int argc = args.Length;
        int end = count + i;

        if (argc < end)
            return false;

        // Parse the parameters known to only be integers.
        for (; i < count; i++) {
            if (!int.TryParse(args[i + 1], out argv[i]))
                return false;
        }

        return true;
    }

    public static bool TryBuildColor(string[] args, out Color color, ref int i) {
        // Parse hexadecimal into unity color
        if (i < args.Length - 1) {
            // Add # if it is missing it
            if (args[i + 1][0] != '#')
                args[i + 1] = "#" + args[i + 1];
            // Return false if failed
            if (!ColorUtility.TryParseHtmlString(args[i + 1], out color))
                return false;
            // Else set alpha to 1 and return true
            color.a = 1f;
            CommandParser.SetLastColor(color);
            return true;
        }
        // Otherwise set the color to the last stored.
        color = CommandParser.GetLastColor();
        return true;
    }
    public bool IsInvalidPosition(Vector3Int pos, Vector3Int size) {
        if (pos.x < 0 || pos.x >= GridMesh.Instance.size.x || pos.y < 0 || pos.y >= GridMesh.Instance.size.y || pos.z < 0 || pos.z >= GridMesh.Instance.size.z) {
            valid = CODE_POSITION_OUT_OF_WORLD;
            return true;   
        }
        Vector3Int end = pos + size;
        if (end.x <= 0 || end.x > GridMesh.Instance.size.x || end.y <= 0 || end.y > GridMesh.Instance.size.y || end.z <= 0 || end.z > GridMesh.Instance.size.z) {
            valid = CODE_POSITION_OUT_OF_WORLD;
            return true;
        }
        return false;
    }

    public bool IsInvalidDestination(Vector3Int pos, Vector3Int size) {
        if (pos.x < 0 || pos.x >= GridMesh.Instance.size.x || pos.y < 0 || pos.y >= GridMesh.Instance.size.y || pos.z < 0 || pos.z >= GridMesh.Instance.size.z) {
            valid = CODE_DESTINATION_OUT_OF_WORLD;
            return true;
        }
        Vector3Int end = pos + size;
        if (end.x <= 0 || end.x > GridMesh.Instance.size.x || end.y <= 0 || end.y > GridMesh.Instance.size.y || end.z <= 0 || end.z > GridMesh.Instance.size.z) {
            valid = CODE_DESTINATION_OUT_OF_WORLD;
            return true;
        }
        return false;
    }

    // Runs whatever the command does.
    public abstract void Execute();
    // Undes whatever the command does.
    public abstract void Undo();
    public abstract void Redo();

    public abstract new string ToString();
}
