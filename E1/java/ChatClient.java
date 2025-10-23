import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.ByteBuffer;
import java.nio.channels.SocketChannel;
import java.util.Scanner;

public class ChatClient {
    public static void main(String[] args) throws IOException {
        SocketChannel client = SocketChannel.open(new InetSocketAddress("localhost", 5000));
        client.configureBlocking(false);
        System.out.println("Connected to chat server!");

        Thread reader = new Thread(() -> {
            ByteBuffer buffer = ByteBuffer.allocate(256);
            try {
                while (true) {
                    int bytes = client.read(buffer);
                    if (bytes > 0) {
                        buffer.flip();
                        System.out.println("💬 " + new String(buffer.array(), 0, bytes));
                        buffer.clear();
                    }
                }
            } catch (IOException e) {
                System.out.println("Connection closed.");
            }
        });
        reader.start();

        Scanner scanner = new Scanner(System.in);
        while (true) {
            String msg = scanner.nextLine();
            client.write(ByteBuffer.wrap(msg.getBytes()));
        }
    }
}
