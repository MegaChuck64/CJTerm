namespace CJTerm;

public class Terminal
{
    public TerminalState State { get; set; }
    int currentLine = 0;
    int currentCol = 2;
    public Terminal(int w, int h)
    {
        Console.SetWindowSize(w, h);
        Console.SetBufferSize(w, h);
        Console.CursorVisible = false;
        FastConsole.Init(w, h);
        Console.SetCursorPosition(currentCol, currentLine);

        State = TerminalState.Starting;
    }

    public void Write(string msg)
    {
        for (int i = 0; i < msg.Length; i++)
        {
            FastConsole.WriteToBuffer(i, currentLine, msg[i], ConsoleColor.White);
            currentCol++;
        }


    }

    public void WriteLine(string msg)
    {
        if (currentLine + 1 >= Console.BufferHeight)
            Clear();

        Write(msg);
        currentLine++;
        currentCol = 2;
        Console.SetCursorPosition(currentCol, currentLine);

    }

    public void Draw()
    {
        FastConsole.Draw();
    }

    public void Run()
    {
        if (State == TerminalState.Starting)
            State = TerminalState.Running;

        do
        {
            switch (State)
            {
                case TerminalState.Running:
                    HandleCommands();
                    break;
                case TerminalState.Powershell:
                    HandlePowershell();
                    break;
            }
            Draw();
        } while (State != TerminalState.Exiting);
    }


    void HandlePowershell()
    {
        if (Console.KeyAvailable)
        {
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                var parms = input.ToLower().Trim().Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                List<string> output = new();
                switch (parms[0])
                {
                    case "exit":
                        State = TerminalState.Running;
                        output.Add("Exiting powershell mode...");
                        break;
                    case "clear":
                        Clear();
                        break;
                    default:
                        output.AddRange(PowerhshellHandler.Command(input));
                        break;
                }
                foreach (var o in output)
                {
                    WriteLine(o);
                }
            }
        }
    }

    void HandleCommands()
    {
        Write("?>");
        if (Console.KeyAvailable)
        {
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                var parms = input.ToLower().Trim().Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                string output;
                switch (parms[0])
                {
                    case "ps":
                        State = TerminalState.Powershell;
                        output = "entering powershell mode";
                        break;
                    case "clear":
                        Clear();
                        output = string.Empty;
                        break;
                    case "exit":
                        State = TerminalState.Exiting;
                        output = "Exiting...";
                        break;
                    default:
                        output = "unknown command: " + input;
                        break;
                }
                WriteLine(output);
            }
        }
    }

    public void Clear()
    {
        FastConsole.ClearBuffer();
        currentLine = -1;
        currentCol = 0;
    }
}

public enum TerminalState
{
    Starting,
    Running,
    Powershell,
    Exiting,
}