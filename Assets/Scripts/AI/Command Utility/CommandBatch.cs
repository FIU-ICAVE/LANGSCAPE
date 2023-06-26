public class CommandBatch {
    public Command[] commands;
    public string request;
    public string response;

    public CommandBatch(Command[] commands, string request, string response) { 
        this.commands = commands;
        this.request = request;
        this.response = response;
    }

    public void Execute() {
        foreach (Command c in commands)
            c.Execute();
    }

    public void Redo() {
        foreach (Command c in commands)
            c.Redo();
    }

    public void Undo() {
        for (int i = commands.Length - 1; i >= 0; i--)
            commands[i].Undo();
    }
}
