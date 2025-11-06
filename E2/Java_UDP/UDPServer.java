package E2.Java_UDP;
import java.net.*;

public class UDPServer {
    public static void main(String[] args) throws Exception {
        try (DatagramSocket serverSocket = new DatagramSocket(12345)) {
            byte[] receiveData = new byte[1024];
            byte[] sendData;

            while (true) {
                DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
                serverSocket.receive(receivePacket);

                String received = new String(receivePacket.getData(), 0, receivePacket.getLength());
                String[] parts = received.split("\\|", 2);
                String seqNum = parts[0];
                String message = parts[1];

                System.out.println("Received packet " + seqNum + ": " + message);

                // Gửi ACK
                String ack = "ACK:" + seqNum;
                sendData = ack.getBytes();
                InetAddress clientAddress = receivePacket.getAddress();
                int clientPort = receivePacket.getPort();
                DatagramPacket ackPacket = new DatagramPacket(sendData, sendData.length, clientAddress, clientPort);
                serverSocket.send(ackPacket);
            }
        }
    }
}
