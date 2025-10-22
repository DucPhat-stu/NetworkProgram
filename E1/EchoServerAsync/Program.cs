using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var cts = new CancellationTokenSource();
        var listener = new TcpListener(IPAddress.Loopback, 8888);
        listener.Start();
        Console.WriteLine("Async Echo Server started on 127.0.0.1:8888. Press Enter to stop.");

        var acceptTask = AcceptLoopAsync(listener, cts.Token);

        Console.ReadLine();
        cts.Cancel();
        listener.Stop();
        await acceptTask;
        Console.WriteLine("Server stopped.");
    }

    static async Task AcceptLoopAsync(TcpListener listener, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var acceptTask = listener.AcceptTcpClientAsync();
            var completed = await Task.WhenAny(acceptTask, Task.Delay(-1, ct));
            if (completed != acceptTask) break; // cancelled
            TcpClient client = acceptTask.Result;
            _ = HandleClientAsync(client, ct); // fire-and-forget handler
        }
    }

    static async Task HandleClientAsync(TcpClient client, CancellationToken ct)
    {
        Console.WriteLine("Client connected.");
        using (client)
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    int read = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                    if (read == 0) break; // client closed
                    var received = Encoding.UTF8.GetString(buffer, 0, read);
                    Console.WriteLine("Received: " + received.TrimEnd('\n', '\r'));
                    var response = Encoding.UTF8.GetBytes("Echo: " + received);
                    await stream.WriteAsync(response, 0, response.Length, ct);
                }
            }
            catch (OperationCanceledException) { /* shutting down */ }
            catch (Exception ex) { Console.WriteLine("Client handler error: " + ex.Message); }
            Console.WriteLine("Client disconnected.");
        }
    }
}
