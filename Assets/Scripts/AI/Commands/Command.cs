using UnityEngine;

public abstract class Command {

    // Override in other commands
    public static readonly char SIGNATURE = 'n';
    public static readonly int PARAM_COUNT = 0;
    public static readonly int REQUIRED_PARAMS = 0;

    public static bool TryBuildRequired(string[] args, int count, ref int[] argv, ref int i) {
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

    // Runs whatever the command does.
    public abstract void Execute();
    // Undes whatever the command does.
    public abstract void Undo();
    public abstract void Redo();

    public abstract new string ToString();
}
