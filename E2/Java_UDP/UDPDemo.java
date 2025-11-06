package E2.Java_UDP;
import java.awt.*;
import java.net.*;
import javax.swing.*;

public class UDPDemo extends JFrame {
    private JTextArea chatArea;
    private JTextField inputField;
    private JButton sendButton;

    private DatagramSocket socket;
    private InetAddress serverAddress;
    private int port = 12345;
    private int sequence = 0;

    public UDPDemo() {
        setTitle("UDP Chat");
        setSize(400, 500);
        setDefaultCloseOperation(EXIT_ON_CLOSE);
        setLayout(new BorderLayout());

        chatArea = new JTextArea();
        chatArea.setEditable(false);
        chatArea.setFont(new Font("Monospaced", Font.PLAIN, 14));
        JScrollPane scrollPane = new JScrollPane(chatArea);

        inputField = new JTextField();
        sendButton = new JButton("Send");

        JPanel bottomPanel = new JPanel(new BorderLayout());
        bottomPanel.add(inputField, BorderLayout.CENTER);
        bottomPanel.add(sendButton, BorderLayout.EAST);

        add(scrollPane, BorderLayout.CENTER);
        add(bottomPanel, BorderLayout.SOUTH);

        try {
            socket = new DatagramSocket();
            socket.setSoTimeout(2000);
            serverAddress = InetAddress.getByName("localhost");
        } catch (Exception e) {
            appendChat("⚠️ Error initializing socket: " + e.getMessage());
        }

        sendButton.addActionListener(e -> sendMessage());
        inputField.addActionListener(e -> sendMessage()); // Enter key
    }

    private void sendMessage() {
        String message = inputField.getText().trim();
        if (message.isEmpty()) return;

        String packet = sequence + "|" + message;
        byte[] sendData = packet.getBytes();
        byte[] receiveData = new byte[1024];

        appendChat("[Client]: " + message);

        while (true) {
            try {
                DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, serverAddress, port);
                socket.send(sendPacket);

                DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
                socket.receive(receivePacket);
                String ack = new String(receivePacket.getData(), 0, receivePacket.getLength());

                appendChat("[Server]: " + ack);
                sequence++;
                break;
            } catch (SocketTimeoutException e) {
                appendChat("⏳ Timeout for packet " + sequence + ", resending...");
            } catch (Exception e) {
                appendChat("❌ Error: " + e.getMessage());
                break;
            }
        }

        inputField.setText("");
    }

    private void appendChat(String text) {
        chatArea.append(text + "\n");
    }

    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> new UDPDemo().setVisible(true));
    }
}
