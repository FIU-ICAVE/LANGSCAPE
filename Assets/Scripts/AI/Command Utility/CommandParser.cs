using System.Collections.Generic;
using UnityEngine;

public class CommandParser 
{
    public static readonly int CODE_SUCCESS = 0;            // Returned if a command was properly parsed. (Ex: fill was successfully created)
    public static readonly int CODE_INVALID_COMMAND = 1;    // Returned if the command returned was null. (Ex: A question was asked or no proper response could be given)
    public static readonly int CODE_INVALID_RESPONSE = 2;   // Returned if a command was improperly parsed. (Ex: fill had the wrong number of arguments/an invalid token)
    public static readonly int CODE_EMPTY_COMMAND = 3;      // Returned if a new line was returned with no command.

    public static int Parse(string str, char commandSeparator, char argSeparator, out Command[] cmds) {
        cmds = null;

        if (str.Length == 0)
        {
            LangscapeError.Instance.ThrowUserError(LangscapeError.CMD_INVALID);
            return CODE_INVALID_COMMAND;
        }

        List<Command> cmdList = new List<Command>();
        List<Command> utilList = new List<Command>(); // Undo and redo commands

        string[] commandStrings = str.Split(commandSeparator);

        // fill 0 0 0 1 10 10 1 #
        foreach (string command in commandStrings) {
            if (command == "")
                continue;
            
            string[] args = command.Split(argSeparator);
            int[] argv;

            // If the signature is greater than 1 character, halt right here.
            if (args[0].Length > 1)
                return CODE_INVALID_RESPONSE;

            // Store the command signature.
            char signature = args[0][0];

            // If the signature is equal to the null command, exit the process with null.
            if (signature == Command.SIGNATURE)
            {
                LangscapeError.Instance.ThrowUserError(LangscapeError.CMD_INVALID);
                return CODE_INVALID_COMMAND;
            }

            // If the signature is equal to the fill command, attempt to parse it.
            if (signature == FillCommand.SIGNATURE) {
                if (args.Length <= FillCommand.REQUIRED_PARAMS)
                    return CODE_INVALID_RESPONSE;
                argv = new int[FillCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildArgs(args, FillCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return CODE_INVALID_RESPONSE;
                if (!Command.TryBuildColor(args, out Color color, ref i))
                    return CODE_INVALID_RESPONSE;
                FillCommand cmd = new FillCommand(argv, color);
                if (cmd.valid != Command.CODE_VALID)
                    return cmd.valid;
                cmdList.Add(cmd);
                continue;
            }

            if (signature == MoveCommand.SIGNATURE) {
                if (args.Length != MoveCommand.REQUIRED_PARAMS + 1)
                    return CODE_INVALID_RESPONSE;
                argv = new int[MoveCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildArgs(args, MoveCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return CODE_INVALID_RESPONSE;
                MoveCommand cmd = new MoveCommand(argv);
                if (cmd.valid != Command.CODE_VALID)
                    return cmd.valid;
                cmdList.Add(cmd);
                continue;
            }

            if (signature == RotateCommand.SIGNATURE) {
                if (args.Length != RotateCommand.REQUIRED_PARAMS + 1)
                    return CODE_INVALID_RESPONSE;
                argv = new int[RotateCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildArgs(args, RotateCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return CODE_INVALID_RESPONSE;
                RotateCommand cmd = new RotateCommand(argv);
                if (cmd.valid != Command.CODE_VALID)
                    return cmd.valid;
                cmdList.Add(cmd);
                continue;
            }

            if (signature == ModifyCommand.SIGNATURE) {
                if (args.Length <= ModifyCommand.REQUIRED_PARAMS)
                    return CODE_INVALID_RESPONSE;
                argv = new int[ModifyCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildArgs(args, ModifyCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return CODE_INVALID_RESPONSE;
                if (!Command.TryBuildColor(args, out Color color, ref i))
                    return CODE_INVALID_RESPONSE;
                ModifyCommand cmd = new ModifyCommand(argv, color);
                if (cmd.valid != Command.CODE_VALID)
                    return cmd.valid;
                cmdList.Add(cmd);
                continue;
            }

            if (signature == UndoCommand.SIGNATURE) {
                if (args.Length != UndoCommand.REQUIRED_PARAMS + 1)
                    return CODE_INVALID_RESPONSE;
                argv = new int[1];
                int i = 0;
                if (!Command.TryBuildArgs(args, UndoCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return CODE_INVALID_RESPONSE;
                UndoCommand cmd = new UndoCommand(argv[0]);
                if (cmd.valid != Command.CODE_VALID)
                    return cmd.valid;
                utilList.Add(cmd);
                continue;
            }

            if (signature == RedoCommand.SIGNATURE) {
                if (args.Length != RedoCommand.REQUIRED_PARAMS + 1)
                    return CODE_INVALID_RESPONSE;
                argv = new int[1];
                int i = 0;
                if (!Command.TryBuildArgs(args, RedoCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return CODE_INVALID_RESPONSE;
                RedoCommand cmd = new RedoCommand(argv[0]);
                if (cmd.valid != Command.CODE_VALID)
                    return cmd.valid;
                utilList.Add(cmd);
                continue;
            }

            return CODE_INVALID_RESPONSE;
        }
        // Undo and Redo
        foreach (Command cmd in utilList)
            cmd.Execute();
        // Regenerate the mesh after undo/redo if that is all there was to do.
        if (cmdList.Count == 0 && utilList.Count != 0)
            GridMesh.Instance.RegenerateMesh();
        cmds = cmdList.ToArray();
        return CODE_SUCCESS;
    }
}
