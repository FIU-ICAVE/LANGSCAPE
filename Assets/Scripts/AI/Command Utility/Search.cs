using System; // For IsNullOrEmpty()
using System.Linq; // For Count()
using System.Runtime; // For IndexOf() and Substring()
//using System.Collection;

namespace Search
{
    class SearchAlgorithms
    {
        
        /// <summary>
        /// Returns Combined Sentence and Combined Command.
        /// Uses return values of cSplit find beginning of first command
        /// and cSplit2 to find end of Final command
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ind1"></param>
        /// <param name="ind2"></param>"
        /// <returns>Tuple of Two Strings</returns>
        public (string sentence, string command) fullSplit(string message, string[] ind1, string[] ind2)
        {
            // Variables
            /* Indicators for Commands */
            string[] indicator = ind1;
            string[] indicator2 = ind2;
            /* Starts with beginning of Command */
            var modMessage = cSplit(message, indicator2); // Returns Empty Strings if Message is Empty
            var sentence = modMessage.s;
            var command = modMessage.c;
            string finalStart;
            int lastIns = 0;
            int split = 0;
            int far = 0;


            // Returns Empty String
            if (string.IsNullOrEmpty(message)) { return (sentence, command); }

            int num = command.Length;
            int count = indicator2.Count();
            // Loops "count - 1" Times Until an Indicator is Found
            for (int i = 0; i < count; i++)
            {
                // Outputs -1 if Not Found
                lastIns = command.LastIndexOf(indicator2[i], num);

                // If Indicator Found Checks if Furthest Away
                if (lastIns > far) { far = lastIns; }
            }

            // If No Other Indicator except First One Return 0, otherwise Return Farthest Indicator Position
            finalStart = command.Substring(lastIns + 1);

            // Check if Command is First and There is More Than One Command
            if (hasOnlyCommand(finalStart, indicator) && lastIns > 0)
            {
                // Using First Char in finalStart and finalStart String, Output Position at End of Command
                split = cSplit2(finalStart[0], finalStart);

                // Trim Command to End of Final Commands Postion
                command = command.Substring(0, split);
                
                // Returns Updated Sentence and Command
                return (sentence + " " + command.Substring(split) , command);
            }

            // Returns Sentence and Command Modified by cSplit function only
            return (sentence, command);


        }
        
        /// <summary>
        /// True or False, Message Contains Only Command First
        /// </summary>
        /// <param name="message"></param>
        /// <param name="indicator"></param>
        /// <returns>Boolean</returns>
        public bool hasOnlyCommand(string message, string[] indicator)
        {
            // Variables
            bool onlyCommand;
            int count = indicator.Count();

            // If Message is Empty or NULL, return false
            if (string.IsNullOrEmpty(message) == true) { return false; }
            for (int i = 0; i < count; i++)
            {
                onlyCommand = message.StartsWith(indicator[i]);
                // If Message has the indicator at start, return true
                if (onlyCommand == true) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Returns Modified Message in Two Parts : Before and After Indicator
        /// </summary>
        /// <param name="message"></param>
        /// <param name="indicator"></param>
        /// <returns>Tuple of Two Strings</returns>
        public (string s, string c)cSplit(string message, string[] indicator)
        {
            var sentence = string.Empty;
            var command = string.Empty;

            // If Message Declaration is Empty, Don't Check Contents
            if (string.IsNullOrEmpty(message) == true) { return (sentence, command); }
            else
            {
                // Variables
                // Total Amount of Strings in Message Including Spaces and Newlines
                int num = message.Length;
                // Determines How Many Strings From Indicator Need to be Checked
                int count = indicator.Count();
                // Initial String Start Value
                int startFrom = 0;

                // Loops "count - 1" Times Until an Indicator is Found
                for (int i = 0; i < count; i++)
                {
                    // Outputs -1 if Not Found
                    startFrom = message.IndexOf(indicator[i], 0, num);

                    // If No Indicator was Found and This is the Last Loop Start From End
                    if (startFrom == -1 && i == count - 1) { startFrom = num - 1; }
                    
                    // If Indicator Found Stops Loop and Keeps Start From Value
                    if (startFrom != -1) { i = count; }
                }
                // Return Substrings Containing Before and After Indicator
                sentence = message.Substring(0, startFrom);
                command  = message.Substring(startFrom + 1);

                return (sentence, command);
            }
        }

        /// <summary>
        /// Returns Integer Where End of Command.
        /// Part 2 of Command Split Process.
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="message"></param>
        /// <returns>Integer</returns>
        public int cSplit2(char letter, string message)
        {
            /* Max Lengths*/
            int c_hex = 6;
            // c1 Max Mandatory Parameter Amount, c2 Max Optional Parameter Amount
            int c1 = 0, c2 = 0;
            string mess = message;
            // First 50 Chars for Less Memory Usage
            if (message.Length > 50)
            {
                mess = message.Substring(0, 50);
            }
            string[] words = mess.Split(' ');
            int pos = 0;

            switch (letter)
            {
                // 8 Parameters, 1 Optional
                case 'f':
                    c1 = 7; c2 = 8;

                    if (words.Count() >= c2 && words[c2].Length == c_hex && words[c2][0].Equals('#'))
                    {
                        pos = aCount(words, c2);
                    }
                    else
                    {
                        pos = aCount(words, c1);
                    }
                    break;
                // 9 Parameters, 0 Optional
                case 'm':
                    c1 = 9;
                    break;
                // 7 Parameters, 0 Optional
                case 'r':
                    c1 = 7;
                    break;
                // 9 Parameters, 1 Optional
                case 'c':
                    c1 = 9; c2 = 10;

                    if (words.Count() >= c2 && words[c2].Length == c_hex && words[c2][0].Equals('#'))
                    {
                        pos = aCount(words, c2);   
                    }
                    else
                    {
                        pos = aCount(words, c1);
                    }
                    break;
                // 1 Parameter, 0 Optional
                case 'u':
                    c1 = 1;
                    pos = aCount(words, c1);
                    break;
                // 1 Parameter, 0 Optional
                case 'v':
                    c1 = 1;
                    pos = aCount(words, c1);
                    break;
                // 1 Parameter
                case 'q':
                    c1 = 1;
                    pos = aCount(words, c1);
                    break;
                // 2 Parameters, 0 Optional
                case 't':
                    c1 = 3;
                    pos = aCount(words, c1);
                    break;

            }
            return pos;

        }

        /// <summary>
        /// Count Word Array for cSplit2 Function
        /// </summary>
        /// <param name="message"></param>
        /// <param name="n"></param>
        /// <returns>Integer</returns>
        public int aCount(string[] message, int n)
        {
            int count = 0;
            for (int i = 0; i <= n; i++)
            {
                count += message[i].Length;
            }
            return count;
        }

        /// <summary>
        /// True or False, Message contains indicator for switching to 2nd LLM
        /// </summary>
        /// <param name="message"></param>
        /// <param name="indicator"></param>
        /// <returns>Boolean</returns>
        public bool SwitchLLM(string message, string indicator)
        {

            // If Message Declaration is Empty
            if (string.IsNullOrEmpty(message)) { return false; }
            
            // Checks if indicator is in message (case insensitive)
            int check = message.IndexOf(indicator, StringComparison.OrdinalIgnoreCase);
            
            // If Not Found
            if (check == -1) { return false; }

            // If Found
            return true;
        }
    }

    
}