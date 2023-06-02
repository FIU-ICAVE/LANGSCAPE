using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command {
    public struct Syntax {
        public char signature { get; private set; }
        public int[] argc { get; private set; } // Store in decreasing order.

        public Syntax(char signature, int[] argc) { this.signature = signature; this.argc = argc;}
    }

    /*
        LIST OF COMMANDS:
            NULL, FILL, MOVE, ROTATE
        <signature> <params>
        Field: surrounded by <>
        Argument: separated by spaces
     */
    public static Syntax Null = new Syntax('n', null);
    /* 
     *      fill <x0 y0 z0> <sx sy sz> <cell> <texture> <r g b> // 12 arguments, 5 fields
     *      fill <x0 y0 z0> <sx sy sz> <cell> # <r g b>         // 12 arguments, 5 fields
     *      fill <x0 y0 z0> <sx sy sz> <cell> # #               // 10 arguments, 5 fields
     */
    public static Syntax Fill = new Syntax('f', new int[] { 12, 10 });

    // Runs whatever the command does.
    public abstract void Execute();
    // Undes whatever the command does.
    public abstract void Undo();

    public abstract new string ToString();
}
