using System;
using System.Collections.Generic;
using UnityEngine; 

public class BackgroundParser
{

    /// <summary>
    /// // Format of Command Code :: [ErrorCode] [Command Indicator] [Action] [Extra Action]
    /// [Err]   [Ind]   [Action]            [Extra Action]
    /// 0000    n = 0   Uses 0: int
    /// ----    d = 1   Uses State: int   
    /// ----    l = 2   Uses Area: int
    /// ----    o = 3   Uses Item: int      
    /// ----    z = 4   Uses Util: int      Opt: int
    /// </summary>
    public struct b_command
    {
        public int errorCode;
        public int c_ind;
        public int act;
        public bool extra;
        public int act2;

        // When Error Occurs
        public b_command(int eCode)
        {
            this.errorCode = eCode;
            this.c_ind = 0;
            this.act = 0;
            this.extra = false;
            this.act2 = 0;
        }
        // When No Extra Parameter Exists
        public b_command(int eCode, int cCode, int aCode, bool eCheck)
        {
            this.errorCode = eCode;
            this.c_ind = cCode;
            this.act = aCode;
            this.extra = eCheck;
            this.act2 = 0;
        }
        // When Extra Parameter Exists
        public b_command(int eCode, int cCode, int aCode, bool eCheck, int a2Code)
        {
            this.errorCode = eCode;
            this.c_ind = cCode;
            this.act = aCode;
            this.extra = eCheck;
            this.act2 = a2Code;
        }
    }

    // Returns an List of b_commands ( Error and CommandCode)
    // If there is an error return Corresponding Error Code and Empty Integer Array
    // Format of Command Code :: <ErrorCode> <Command Indicator> <Action> <Extra Action>
    // Extra Action has addtional bool indicator
    public static List<b_command> Parse(string input)
    {
        // Variables
        /* Part 1 */
        bool loop = true;

        /* Part 2*/
        List<b_command> bfg = new List<b_command>();
        string[] words = input.Split(' ','\r','\n');
        int k = 0;
        do
        {
            b_command bcom = new b_command(0000);
            int error = LangscapeError.CMD_VALID.code;
            int j = 0;
            switch (words[k])
            {
                case "n":
                    bcom = new b_command(error);
                    break;
                // One Parameter, 0 Optional
                case "d":
                    if (int.TryParse(words[k + 1], out j))
                    {
                        bcom = new b_command(error, 1, j, false);
                    }
                    else
                    {
                        error = LangscapeError.CMD_INVALID_PARAM.code;
                        bcom = new b_command(error);
                    }

                    break;
                // One Parameter, 0 Optional
                case "l":
                    if (int.TryParse(words[k + 1], out j))
                    {
                        bcom = new b_command(error, 2, j, false);
                    }
                    else
                    {
                        error = LangscapeError.CMD_INVALID_PARAM.code;
                        bcom = new b_command(error);
                    }
                    break;
                // One Parameter, 0 Optional
                case "o":
                    if (int.TryParse(words[k + 1], out j))
                    {
                        bcom = new b_command(error, 3, j, false);
                    }
                    else
                    {
                        error = LangscapeError.CMD_INVALID_PARAM.code;
                        bcom = new b_command(error);
                    }
                    break;
                // Two Parameters, 1 Optional
                case "z":
                    if (int.TryParse(words[k + 1], out j))
                    {
                        if (words.Length < k + 3)
                        {
                            bcom = new b_command(error, 4, j, false);
                        }
                        else if (Char.IsLetter(words[k + 2], 0))
                        {
                            // End Utility Command and Move to Next Command
                            bcom = new b_command(error, 4, j, false);
                        }
                        else
                        {
                            int p = 0;
                            if (int.TryParse(words[k + 2], out p))
                            {
                                bcom = new b_command(error, 4, j, true, p);
                            }
                            else
                            {
                                error = LangscapeError.CMD_INVALID_PARAM.code;
                                bcom = new b_command(error);
                            }
                        }
                    }
                    else
                    {
                        error = LangscapeError.CMD_INVALID_PARAM.code;
                        bcom = new b_command(error);
                    }
                    break;
                // <How does one get here?>
                default:
                    error = LangscapeError.CMD_INVALID_PARAM.code;
                    bcom = new b_command(error);
                    break;
            }

            // Command Wrap-Up Corner
            bfg.Add(bcom);
            // Check Error Code
            if (bcom.errorCode != LangscapeError.CMD_VALID.code)
            {
                loop = false;
            }
            else
            {
                // Check for Two Parameters
                if (bcom.extra)
                {
                    if (words.Length > k + 3)
                    {
                        loop = true;
                        k += 3;
                    }
                    else
                    {
                        loop = false;
                    }
                }
                else
                {
                    if (words.Length > k + 2)
                    {
                        loop = true;
                        k += 2;
                    }
                    else
                    {
                        loop = false;
                    }
                }
            }

        } while (loop);

        return bfg;
    }
}