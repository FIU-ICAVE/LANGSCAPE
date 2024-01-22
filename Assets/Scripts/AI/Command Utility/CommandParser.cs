using System.Collections.Generic;
using UnityEngine;

public class CommandParser 
{
    /*
    public static readonly int CODE_SUCCESS = 0;            // Returned if a command was properly parsed. (Ex: fill was successfully created)
    public static readonly int CODE_INVALID_COMMAND = 1;    // Returned if the command returned was null. (Ex: A question was asked or no proper response could be given)
    public static readonly int CODE_INVALID_RESPONSE = 2;   // Returned if a command was improperly parsed. (Ex: fill had the wrong number of arguments/an invalid token)
    public static readonly int CODE_EMPTY_COMMAND = 3;      // Returned if a new line was returned with no command.
    */
    public static int Parse(string str, char[] commandSeparator, char argSeparator, out Command[] cmds) {
        cmds = null;

        if (str.Length == 0)
            return LangscapeError.LLM_INVALID_RESPONSE.code;

        List<Command> cmdList = new List<Command>();
        List<Command> utilList = new List<Command>(); // Undo and redo commands

        string[] commandStrings = str.Split(commandSeparator[0],commandSeparator[1]);

        // fill 0 0 0 1 10 10 1 #
        foreach (string command in commandStrings) {
            if (command == "")
                continue;
            
            string[] args = command.Split(argSeparator);
            int[] argv;

            // If the signature is greater than 1 character, halt right here.
            if (args[0].Length > 1)
                return LangscapeError.LLM_INVALID_RESPONSE.code;

            // Store the command signature.
            char signature = args[0][0];

            // If the signature is equal to the null command, exit the process with null.
            if (signature == Command.SIGNATURE)
                return LangscapeError.CMD_VALID.code/*LangscapeError.CMD_NULL.code*/;

            // If the signature is equal to the fill command, attempt to parse it.
            if (signature == FillCommand.SIGNATURE) {
                if (args.Length <= FillCommand.REQUIRED_PARAMS)
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                argv = new int[FillCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildArgs(args, FillCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                if (!Command.TryBuildColor(args, out Color color, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                FillCommand cmd = new FillCommand(argv, color);
                if (cmd.valid != LangscapeError.CMD_VALID.code)
                    return cmd.valid;
                cmdList.Add(cmd);
                continue;
            }

            if (signature == MoveCommand.SIGNATURE) {
                if (args.Length != MoveCommand.REQUIRED_PARAMS + 1)
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                argv = new int[MoveCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildArgs(args, MoveCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                MoveCommand cmd = new MoveCommand(argv);
                if (cmd.valid != LangscapeError.CMD_VALID.code)
                    return cmd.valid;
                cmdList.Add(cmd);
                continue;
            }

            if (signature == RotateCommand.SIGNATURE) {
                if (args.Length != RotateCommand.REQUIRED_PARAMS + 1)
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                argv = new int[RotateCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildArgs(args, RotateCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                RotateCommand cmd = new RotateCommand(argv);
                if (cmd.valid != LangscapeError.CMD_VALID.code)
                    return cmd.valid;
                cmdList.Add(cmd);
                continue;
            }

            if (signature == ModifyCommand.SIGNATURE) {
                if (args.Length <= ModifyCommand.REQUIRED_PARAMS)
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                argv = new int[ModifyCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildArgs(args, ModifyCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                if (!Command.TryBuildColor(args, out Color color, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                ModifyCommand cmd = new ModifyCommand(argv, color);
                if (cmd.valid != LangscapeError.CMD_VALID.code)
                    return cmd.valid;
                cmdList.Add(cmd);
                continue;
            }

            if (signature == UndoCommand.SIGNATURE) {
                if (args.Length != UndoCommand.REQUIRED_PARAMS + 1)
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                argv = new int[1];
                int i = 0;
                if (!Command.TryBuildArgs(args, UndoCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                UndoCommand cmd = new UndoCommand(argv[0]);
                if (cmd.valid != LangscapeError.CMD_VALID.code)
                    return cmd.valid;
                utilList.Add(cmd);
                continue;
            }

            if (signature == RedoCommand.SIGNATURE) {
                if (args.Length != RedoCommand.REQUIRED_PARAMS + 1)
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                argv = new int[1];
                int i = 0;
                if (!Command.TryBuildArgs(args, RedoCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                RedoCommand cmd = new RedoCommand(argv[0]);
                if (cmd.valid != LangscapeError.CMD_VALID.code)
                    return cmd.valid;
                utilList.Add(cmd);
                continue;
            }

            if (signature == QueryCommand.SIGNATURE) {
                if (args.Length <= QueryCommand.REQUIRED_PARAMS)
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                int totalParamCount = args.Length - 1;
                argv = new int[totalParamCount];
                int i = 0;
                if (!Command.TryBuildArgs(args, totalParamCount, ref argv, ref i))
                    return LangscapeError.LLM_INVALID_RESPONSE.code;
                QueryCommand cmd = new QueryCommand(argv);
                if (cmd.valid != LangscapeError.CMD_VALID.code)
                    return cmd.valid;
                utilList.Add(cmd);
                continue;
            }

            return LangscapeError.LLM_INVALID_RESPONSE.code;
        }
        // Undo and Redo
        foreach (Command cmd in utilList)
            cmd.Execute();
        // Regenerate the mesh after undo/redo if that is all there was to do.
        if (cmdList.Count == 0 && utilList.Count != 0)
            GridMesh.Instance.RegenerateMesh();
        cmds = cmdList.ToArray();
        return LangscapeError.CMD_VALID.code;
    }
}
