using System.Net;
using System.Net.Sockets;
using System.Text;
using TextCopy;

string version = "1.3.2";
int port = 31125;
bool debug = args.Contains("--debug");

void Debug(string msg)
{
    if (!debug) return;
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine(msg);
    Console.ResetColor();
}

static void Message(string msg)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine(msg);
    Console.ResetColor();
}

static void Error(string msg)
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine(msg);
    Console.ResetColor();
}

Message($"Platinum Kaizo Lua Script - Version {version}:");
Console.WriteLine();

if (!File.Exists("PK_Script.lua"))
{
    Error("Lua script not found, please make sure it exists in the same directory as this script.");
    Message("\nPress any key to close...");
    Console.ReadKey();
    return;
}

string[] script = File.ReadAllLines("PK_Script.lua");
string scriptVersion = script[1].Split("\"")[1];
if (version != scriptVersion)
{
    Error("Version mismatch, please make sure you use the same version as the Lua script.");
    Error($"(Found version {scriptVersion})");
    Message("\nPress any key to close...");
    Console.ReadKey();
    return;
}

Socket client = default;

async void SendMessage(string data)
{
    string response = "HTTP/1.1 200 OK\r\n"
                    + "Access-Control-Allow-Origin: *\r\n"
                    + $"Content-Length: {data.Length}\r\n"
                    + "Content-Type: text/plain; charset=utf-8\r\n"
                    + $"\r\n{data}";
    Debug("Sending HTTP response");
    Debug(response);
    await client.SendAsync(Encoding.UTF8.GetBytes(response));
    Debug("Sent HTTP response");
}

async void SendError(string data, int code)
{
    string description = "";
    switch (code) {
        case 405:
            description = "Method not allowed";
            break;
    }
    string response = $"HTTP/1.1 {code} {description}\r\n"
                    + "Access-Control-Allow-Origin: *\r\n"
                    + $"Content-Length: {data.Length}\r\n"
                    + "Content-Type: text/plain; charset=utf-8\r\n"
                    + $"\r\n{data}";
    Debug("Sending HTTP response");
    Debug(response);
    await client.SendAsync(Encoding.UTF8.GetBytes(response));
    Debug("Sent HTTP response");
}

async void SetupServer()
{
    Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Bind(new IPEndPoint(IPAddress.Any, port));

    Debug("Starting server");
    socket.Listen();
    Debug("Started server");

    while (true) {
        byte[] responseBytes = new byte[1024];
        char[] responseChars = new char[1024];
        client = await socket.AcceptAsync();
        Debug("Accepted client");
        int received = await client.ReceiveAsync(responseBytes);
        Debug("Received HTTP request");
        Encoding.UTF8.GetChars(responseBytes, 0, received, responseChars, 0);
        Debug("Decoded request:");
        Debug(new string(responseChars));
        string head = new string(responseChars).Split("\r\n\r\n")[0];
        Debug("Head -> " + head);
        string method = head.Split("\r\n")[0].Split(" ")[0];
        Debug("Method -> " + method);
        string path = head.Split("\r\n")[0].Split(" ")[1];
        Debug("Path -> " + path);
        if (method == "GET") switch (path)
        {
            case "/ping":
                SendMessage("Pong!");
                Debug("Returned ping");
                break;
            case "/update":
                string export = SendCommand("export", "");
                Debug("Executed command: export");
                if (export.Split(";")[1].Trim() == "export")
                {
                    SendMessage(export.Split(";")[2]);
                }
                else
                {
                    SendError(export.Split(";")[2], 500);
                }
                break;
            case "/version":
                SendMessage(version);
                Debug("Returned version");
                break;
            default:
                SendMessage($"Platinum Kaizo Lua Script - Version {version}");
                Debug("Returned blank message");
                break;
        }
        else
        {
            SendError("Invalid method.", 405);
            continue;
        }
        client.Close();
        Debug("Disconnected client");
    }
}

using FileSystemWatcher watcher = new FileSystemWatcher(".");
watcher.Filter = "temp.txt";

Task.Factory.StartNew(SetupServer, TaskCreationOptions.LongRunning);

Message("Available commands:");
Message("  sleep(slot)          -- Pre-sleeps the chosen slot.");
Message("  poison(slot)         -- Pre-poisons the chosen slot.");
Message("  burn(slot)           -- Pre-burns the chosen slot.");
Message("  paralyze(slot)       -- Pre-paralyzes the chosen slot.");
Message("  freeze(slot)         -- Pre-freezes the chosen slot.");
Message("  bedtime()            -- Pre-sleeps the entire party.");
Message("  export()             -- Exports a Showdown paste of your party and boxes to the clipboard.");
Message("  showabilities()      -- Displays the ability slots.");
Console.WriteLine();

if (File.Exists("temp.txt")) File.Delete("temp.txt");
if (script[2] != "Command = \"\"" || script[3] != "Args = {}")
{
    script[2] = "Command = \"\"";
    script[3] = "Args = {}";
    File.WriteAllLines("pk_script.lua", script);
}

string SendCommand(string command, string arguments)
{
    script[2] = $"Command = \"{command}\"";
    script[3] = $"Args = {{{arguments}}}";
    File.WriteAllLines("pk_script.lua", script);

    WaitForChangedResult result = watcher.WaitForChanged(WatcherChangeTypes.Changed, 2000);

    if (result.TimedOut)
    {
        return ";error;Lua script not detected, please make sure it's running.";
    }
    else
    {
        string response = File.ReadAllText("temp.txt");

        bool cleanupFailed = false;
        cleanup:
        try
        {
            File.Delete("temp.txt");
        }
        catch (IOException)
        {
            if (cleanupFailed)
            {
                Error("Your command has executed correctly, but the script ran into an unexpected error and is forced to close.");
                Message("\nPress any key to close...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                Thread.Sleep(500);
                cleanupFailed = true;
                goto cleanup;   
            }
        }

        script[2] = $"Command = \"\"";
        script[3] = $"Args = {{}}";
        File.WriteAllLines("pk_script.lua", script);

        return response;
    }
}

while (true)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("> ");
    Console.ResetColor();
    string input = Console.ReadLine();
    if (input == "") continue;
    try
    {
        string command = input.Split("(")[0];
        int startIndex = input.IndexOf('(') + 1;
        int endIndex = input.LastIndexOf(')');
        string rawArguments = input.Substring(startIndex, endIndex - startIndex);
        List<string> argList = rawArguments.Split(",").Select(x => x.Trim()).ToList();
        string arguments = "";
        switch (command)
        {
            case "sleep":
            case "poison":
            case "burn":
            case "paralyze":
            case "freeze":
                if (argList.Count != 1 || !int.TryParse(argList[0], out int slot))
                {
                    Error("Invalid arguments.");
                    continue;
                }
                arguments = $"{slot}";
                break;
            case "bedtime":
                if (argList.Count != 0 && !(argList.Count == 1 && argList[0] == ""))
                {
                    Error("Invalid arguments.");
                    continue;
                }
                break;
            case "sethp":
                if (argList.Count != 2 || !int.TryParse(argList[0], out slot) || !int.TryParse(argList[1], out int hp))
                {
                    Error("Invalid arguments.");
                    continue;
                }
                arguments = $"{slot}, {hp}";
                break;
            case "export":
                if (argList.Count != 0 && !(argList.Count == 1 && argList[0] == ""))
                {
                    Error("Invalid arguments.");
                    continue;
                }
                break;
            case "showabilities":
                if (argList.Count != 0 && !(argList.Count == 1 && argList[0] == ""))
                {
                    Error("Invalid arguments.");
                    continue;
                }
                break;
            default:
                Error("Invalid command.");
                continue;
        }

        string response = SendCommand(command, arguments);
        command = response.Split(";")[1].Trim();
        switch (command)
        {
            case "export":
                string export = response.Split(";")[2];
                ClipboardService.SetText(export);
                Message("Copied Showdown paste to clipboard.");
                break;
            case "showabilities":
                export = response.Split(";")[2].Trim();
                Message(export);
                break;
            case "error":
                string error = response.Split(";")[2];
                Error(error);
                break;
        }
    }
    catch
    {
        Error("Unknown error.");
        script[2] = $"Command = \"\"";
        script[3] = $"Args = {{}}";
        File.WriteAllLines("pk_script.lua", script);
        if (File.Exists("temp.txt")) File.Delete("temp.txt");
    }
}
