using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        int clients = 100;     // số client đồng thời
        int messages = 20;     // tin mỗi client
        string host = "127.0.0.1";
        int port = 8888;       // port server (thay đổi theo server đang chạy)

        var sw = Stopwatch.StartNew();
        var tasks = new Task[clients];
        for (int i = 0; i < clients; i++)
            tasks[i] = RunClient(i, host, port, messages);

        await Task.WhenAll(tasks);
        sw.Stop();
        Console.WriteLine($"All done. Total time: {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task RunClient(int id, string host, int port, int messages)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(host, port);
        var stream = client.GetStream();
        var buffer = new byte[4096];
        for (int i = 0; i < messages; i++)
        {
            string msg = $"C{id}-msg{i}";
            var data = Encoding.UTF8.GetBytes(msg + "\n");
            await stream.WriteAsync(data, 0, data.Length);
            int read = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (read == 0) break;
            // optionally verify response
        }
    }
}
