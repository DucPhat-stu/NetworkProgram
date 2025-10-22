using System;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Chọn chế độ (s = Server, c = Client): ");
        var mode = Console.ReadLine();

        if (mode == "s")
        {
            var server = new ChatServer(5000);
            await server.StartAsync();
        }
        else if (mode == "c")
        {
            Console.Write("Nhập tên của bạn: ");
            var name = Console.ReadLine() ?? "Người dùng";

            var client = new ChatClient("127.0.0.1", 5000, name);
            await client.StartAsync();
        }
    }
}
