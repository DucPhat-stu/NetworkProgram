using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 8888);
        Console.WriteLine("Connected to echo server.");
        var stream = client.GetStream();

        while (true)
        {
            Console.Write("Msg> ");
            string line = Console.ReadLine();
            if (string.IsNullOrEmpty(line)) break;
            byte[] data = Encoding.UTF8.GetBytes(line + "\n");
            await stream.WriteAsync(data, 0, data.Length);
            var buffer = new byte[4096];
            int read = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (read == 0) break;
            Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, read));
        }
    }
}
