public class UdpClient
{
    private string serverAddress;
    private int serverPort;

    public UdpClient(string address, int port)
    {
        serverAddress = address;
        serverPort = port;
    }

    public void SendData(byte[] data)
    {
        // Code to send data via UDP
        Console.WriteLine($"Sending data to {serverAddress}:{serverPort}");
    }

    public byte[] ReceiveData()
    {
        // Code to receive data via UDP
        Console.WriteLine($"Receiving data from {serverAddress}:{serverPort}");
        return new byte[0]; // Placeholder for received data
    }
}