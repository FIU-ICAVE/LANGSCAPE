using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;

public class CommandParser 
{
    public static readonly int CODE_SUCCESS = 0;            // Returned if a command was properly parsed. (Ex: fill was successfully created)
    public static readonly int CODE_INVALID_COMMAND = 1;    // Returned if the command returned was null. (Ex: A question was asked or no proper response could be given)
    public static readonly int CODE_INVALID_RESPONSE = 2;   // Returned if a command was improperly parsed. (Ex: fill had the wrong number of arguments/an invalid token)

    private static int lastTexture = 0;
    private static int lastR = 255;
    private static int lastG = 255;
    private static int lastB = 255;

    private static int[] BuildCommand(Command.Syntax syntax, string[] args) {
        int argc = args.Length;

        for (int i = 0; i < syntax.argc.Length; i++) { //finding if there's a matching command syntax
            if (argc == syntax.argc[i]) { //found matching syntax
                int[] argv = new int[syntax.argc[0] - 1];
                int required = (syntax.argc.Length == 1) ? syntax.argc[0] : syntax.argc[0] - 5;
                int j = 0;
                // Parse the parameters known to only be integers.
                for (; j < required; j++) {
                    if (!int.TryParse(args[j + 1], out argv[j]))
                        return null;
                }

                if (syntax.argc.Length == 1)
                    return argv;

                if (args[j + 1].Equals("#"))
                    argv[j] = lastTexture;
                else if (int.TryParse(args[j + 1], out argv[j])) {
                    lastTexture = argv[j];
                } else return null;

                j++;

                if (args[j + 1].Equals("#")) {
                    argv[j] = lastR;
                    j++;
                    argv[j] = lastG;
                    j++;
                    argv[j] = lastB;
                } else if (int.TryParse(args[j + 1], out argv[j++]) && int.TryParse(args[j + 1], out argv[j++]) && int.TryParse(args[j + 1], out argv[j])) {
                    lastB = argv[j];
                    lastG = argv[j - 1];
                    lastR = argv[j - 2];
                } else return null;

                return argv;
            }
        }

        return null;
    }

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
            if (signature == Command.Null.signature)
                return CODE_INVALID_COMMAND;

            // If the signature is equal to the fill command, attempt to parse it.
            if (signature == Command.Fill.signature) {
                argv = BuildCommand(Command.Fill, args);
                if (argv == null) 
                    return CODE_INVALID_RESPONSE;
                cmdList.Add(new FillCommand(argv));
                continue;
            }

            return CODE_INVALID_RESPONSE;
        }

        cmds = cmdList.ToArray();
        return CODE_SUCCESS;
    }

    public static void SetLastColor(int r, int g, int b) {
        lastR = r; lastG = g; lastB = b;
    }

    public static void SetLastTexture(int textureID) {
        lastTexture = textureID;
    }
}
