import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.ByteBuffer;
import java.nio.channels.*;
import java.util.*;

public class ChatServer {
    private static final int PORT = 5000;
    private Selector selector;
    private ServerSocketChannel serverChannel;
    private final Map<SocketChannel, String> clients = new HashMap<>();

    public void start() throws IOException {
        selector = Selector.open();
        serverChannel = ServerSocketChannel.open();
        serverChannel.bind(new InetSocketAddress(PORT));
        serverChannel.configureBlocking(false);
        serverChannel.register(selector, SelectionKey.OP_ACCEPT);

        System.out.println("💬 Chat Server running on port " + PORT);

        while (true) {
            selector.select(); // đợi sự kiện
            Iterator<SelectionKey> keys = selector.selectedKeys().iterator();

            while (keys.hasNext()) {
                SelectionKey key = keys.next();
                keys.remove();

                if (key.isAcceptable()) handleAccept();
                else if (key.isReadable()) handleRead(key);
            }
        }
    }

    private void handleAccept() throws IOException {
        SocketChannel client = serverChannel.accept();
        client.configureBlocking(false);
        client.register(selector, SelectionKey.OP_READ);
        clients.put(client, "User" + clients.size());
        System.out.println("✅ " + clients.get(client) + " connected.");
    }

    private void handleRead(SelectionKey key) {
        SocketChannel client = (SocketChannel) key.channel();
        ByteBuffer buffer = ByteBuffer.allocate(256);
        int bytesRead;

        try {
            bytesRead = client.read(buffer);
            if (bytesRead == -1) {
                clients.remove(client);
                client.close();
                System.out.println("❌ A client disconnected.");
                return;
            }
        } catch (IOException e) {
            clients.remove(client);
            try { client.close(); } catch (IOException ignored) {}
            return;
        }

        buffer.flip();
        String message = new String(buffer.array(), 0, bytesRead).trim();
        System.out.println("📩 Received: " + message);
        broadcast(message, client);
    }

    private void broadcast(String message, SocketChannel sender) {
        for (SocketChannel client : clients.keySet()) {
            if (client != sender && client.isConnected()) {
                try {
                    client.write(ByteBuffer.wrap((message + "\n").getBytes()));
                } catch (IOException e) {
                    System.err.println("⚠️ Error sending message to " + clients.get(client));
                }
            }
        }
    }

    public static void main(String[] args) throws IOException {
        new ChatServer().start();
    }
}
