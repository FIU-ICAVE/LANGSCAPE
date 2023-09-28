using System; // For IsNullOrEmpty()
using System.Linq; // For Count()
using System.Runtime; // For IndexOf() and Substring()
//using System.Collection;

namespace Search
{
    class SearchAlgorithms
    {
        // True or False, Message Contains Only Command
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

        // Returns Modified Message in Two Parts : Before and After Indicator
        public (string sentence, string command)commandOnly(string message, string[] indicator)
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
                    startFrom = message.IndexOf(indicator[0], 0, num);

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

        // True or False, Message contains indicator for switching to 2nd LLM
        public bool SwitchLLM(string message, string indicator)
        {

            // If Message Declaration is Empty
            if (string.IsNullOrEmpty(message)) { return false; }
            
            // Checks if indicator is in message
            int check = message.IndexOf(indicator, 0);
            
            // If Not Found
            if (check == -1) { return false; }

            // If Found
            return true;
        }
    }
}