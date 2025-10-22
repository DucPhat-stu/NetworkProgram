using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class ChatServer
{
    private readonly TcpListener _listener;
    private readonly ConcurrentBag<TcpClient> _clients = new();

    // ✅ Constructor khởi tạo cổng
    public ChatServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    // ✅ Hàm chạy server bất đồng bộ
    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("Server đang chạy trên cổng 5000...");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            _clients.Add(client);
            Console.WriteLine("Client mới kết nối!");
            _ = HandleClientAsync(client);
        }
    }

    // ✅ Xử lý từng client
    private async Task HandleClientAsync(TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];

        while (true)
        {
            int byteCount;
            try
            {
                byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount == 0) break;
            }
            catch
            {
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            Console.WriteLine($"Tin nhắn nhận được: {message}");
            await BroadcastAsync(message, client);
        }

        _clients.TryTake(out _);
        client.Close();
    }

    // ✅ Gửi tin cho tất cả client
    private async Task BroadcastAsync(string message, TcpClient sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (var client in _clients)
        {
            if (client == sender) continue;
            try
            {
                await client.GetStream().WriteAsync(data, 0, data.Length);
            }
            catch
            {
                // Nếu client bị ngắt kết nối, bỏ qua
            }
        }
    }
}
