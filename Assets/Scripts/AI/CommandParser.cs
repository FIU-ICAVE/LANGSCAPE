using System.Collections.Generic;
using UnityEngine;

public class CommandParser 
{
    public static readonly int CODE_SUCCESS = 0;            // Returned if a command was properly parsed. (Ex: fill was successfully created)
    public static readonly int CODE_INVALID_COMMAND = 1;    // Returned if the command returned was null. (Ex: A question was asked or no proper response could be given)
    public static readonly int CODE_INVALID_RESPONSE = 2;   // Returned if a command was improperly parsed. (Ex: fill had the wrong number of arguments/an invalid token)

    public static int Parse(string str, char commandSeparator, char argSeparator, out Command[] cmds) {
        cmds = null;

        if (str.Length == 0)
            return CODE_INVALID_RESPONSE;

        List<Command> cmdList = new List<Command>();

        string[] commandStrings = str.Split(commandSeparator);

        // fill 0 0 0 1 10 10 1 #
        foreach (string command in commandStrings) {
            string[] args = command.Split(argSeparator);
            int[] argv;

            // If the signature is greater than 1 character, halt right here.
            if (args[0].Length > 1)
                return CODE_INVALID_RESPONSE;

            // Store the command signature.
            char signature = args[0][0];

            // If the signature is equal to the null command, exit the process with null.
            if (signature == Command.SIGNATURE)
                return CODE_INVALID_COMMAND;

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
                if (args.Length <= MoveCommand.REQUIRED_PARAMS)
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
                if (args.Length <= RotateCommand.REQUIRED_PARAMS)
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

            return CODE_INVALID_RESPONSE;
        }

        cmds = cmdList.ToArray();
        return CODE_SUCCESS;
    }
}
