using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Unity.Tutorials.Core.Editor;
using UnityEditor.UIElements;
using UnityEngine;

public class CommandParser 
{
    public static readonly int CODE_SUCCESS = 0;            // Returned if a command was properly parsed. (Ex: fill was successfully created)
    public static readonly int CODE_INVALID_COMMAND = 1;    // Returned if the command returned was null. (Ex: A question was asked or no proper response could be given)
    public static readonly int CODE_INVALID_RESPONSE = 2;   // Returned if a command was improperly parsed. (Ex: fill had the wrong number of arguments/an invalid token)

    private static int lastBlock = 1;
    private static Color lastColor = Color.white;

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
                argv = new int[FillCommand.REQUIRED_PARAMS];
                Color color;
                int i = 0;
                if (!Command.TryBuildRequired(args, FillCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return CODE_INVALID_RESPONSE;
                if (!Command.TryBuildColor(args, out color, ref i))
                    return CODE_INVALID_RESPONSE;
                cmdList.Add(new FillCommand(argv, color));
                continue;
            }


            if (signature == MoveCommand.SIGNATURE) {
                argv = new int[MoveCommand.REQUIRED_PARAMS];
                int i = 0;
                if (!Command.TryBuildRequired(args, FillCommand.REQUIRED_PARAMS, ref argv, ref i))
                    return CODE_INVALID_RESPONSE;
                cmdList.Add(new MoveCommand(argv));
                continue;
            }
            return CODE_INVALID_RESPONSE;
        }

        cmds = cmdList.ToArray();
        return CODE_SUCCESS;
    }

    public static void SetLastBlock(int blockID) {
        lastBlock = blockID;
    }

    public static void SetLastColor(Color color) => lastColor = color;
    public static Color GetLastColor() => lastColor;
}
