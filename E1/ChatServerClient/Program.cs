using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 9000);
        Console.WriteLine("Connected to chat server.");
        var stream = client.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);
        var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        // Task to read from server
        var readTask = Task.Run(async () =>
        {
            string? s;
            try
            {
                while ((s = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine("\n[Server] " + s);
                    Console.Write("You> ");
                }
            }
            catch { /* connection closed */ }
        });

        // main thread: read console and send
        while (true)
        {
            Console.Write("You> ");
            string? line = Console.ReadLine();
            if (string.IsNullOrEmpty(line)) break;
            await writer.WriteLineAsync(line);
        }
        client.Close();
        await readTask;
    }
}
