using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static readonly ConcurrentDictionary<int, (TcpClient Client, StreamWriter Writer)> clients = new();
    static int idCounter = 0;

    static async Task Main()
    {
        var listener = new TcpListener(IPAddress.Loopback, 9000);
        listener.Start();
        Console.WriteLine("Chat server started on 127.0.0.1:9000");

        while (true)
        {
            var tcp = await listener.AcceptTcpClientAsync();
            int id = System.Threading.Interlocked.Increment(ref idCounter);
            Console.WriteLine($"Client {id} connected.");
            var stream = tcp.GetStream();
            var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            clients[id] = (tcp, writer);
            _ = HandleClientAsync(id, tcp);
        }
    }

    static async Task HandleClientAsync(int id, TcpClient tcp)
    {
        try
        {
            using var reader = new StreamReader(tcp.GetStream(), Encoding.UTF8);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                Console.WriteLine($"[{id}] {line}");
                await BroadcastLineAsync($"User{id}: {line}", excludeId: id);
            }
        }
        catch (IOException) { /* connection closed */ }
        catch (Exception ex) { Console.WriteLine("Error client: " + ex.Message); }
        finally
        {
            Console.WriteLine($"Client {id} disconnected.");
            if (clients.TryRemove(id, out var entry))
            {
                try { entry.Client.Close(); } catch { }
            }
        }
    }

    static async Task BroadcastLineAsync(string message, int excludeId)
    {
        var tasks = new System.Collections.Generic.List<Task>();
        var snapshot = clients.ToArray();
        foreach (var kv in snapshot)
        {
            if (kv.Key == excludeId) continue;
            try
            {
                tasks.Add(kv.Value.Writer.WriteLineAsync(message));
            }
            catch
            {
                // ignore; removal will happen elsewhere
            }
        }
        await Task.WhenAll(tasks);
    }
}
