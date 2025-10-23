import java.io.*;
import java.net.*;
import java.util.concurrent.*;
// using CompletableFuture for async handling
public class AsyncEchoServer {
    public static void main(String[] args) throws Exception {
        ExecutorService pool = Executors.newFixedThreadPool(4);
        ServerSocket server = new ServerSocket(5000);
        System.out.println("Async Server running on port 5000");

        while (true) {
            Socket client = server.accept();
            CompletableFuture.runAsync(() -> handleClient(client), pool);
        }
    }

    static void handleClient(Socket client) {
        try (BufferedReader in = new BufferedReader(new InputStreamReader(client.getInputStream()));
             PrintWriter out = new PrintWriter(client.getOutputStream(), true)) {
            String msg;
            while ((msg = in.readLine()) != null) {
                System.out.println("Received: " + msg);
                out.println("Echo: " + msg);
            }
        } catch (IOException e) {
            System.err.println("Client disconnected");
        }
    }
}
