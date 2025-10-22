using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class ChatClient
{
    private readonly TcpClient _client = new();
    private readonly string _name;

    public ChatClient(string host, int port, string name)
    {
        _name = name;
        _client.Connect(host, port);
    }

    public async Task StartAsync()
    {
        Console.WriteLine("Đã kết nối đến server. Bắt đầu chat...");

        var stream = _client.GetStream();

        // Nhận tin nhắn từ server
        _ = Task.Run(async () =>
        {
            var buffer = new byte[1024];
            while (true)
            {
                int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount == 0) break;
                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Console.WriteLine(message);
            }
        });

        // Gửi tin nhắn
        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;
            string message = $"{_name}: {input}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
