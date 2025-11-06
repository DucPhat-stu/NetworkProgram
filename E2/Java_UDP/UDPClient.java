package E2.Java_UDP;
import java.net.*;

public class UDPClient {
    public static void main(String[] args) throws Exception {
        DatagramSocket clientSocket = new DatagramSocket();
        clientSocket.setSoTimeout(2000); // timeout 2 giây

        InetAddress IPAddress = InetAddress.getByName("localhost");
        String[] messages = {"Hello", "from", "UDP", "Client"};

        for (int i = 0; i < messages.length; i++) {
            String packet = i + "|" + messages[i];
            byte[] sendData = packet.getBytes();
            byte[] receiveData = new byte[1024];

            while (true) {
                DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, IPAddress, 12345);
                clientSocket.send(sendPacket);

                try {
                    DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
                    clientSocket.receive(receivePacket);
                    String ack = new String(receivePacket.getData(), 0, receivePacket.getLength());

                    if (ack.equals("ACK:" + i)) {
                        System.out.println("Received ACK for packet " + i);
                        break;
                    }
                } catch (SocketTimeoutException e) {
                    System.out.println("Timeout for packet " + i + ", resending...");
                }
            }
        }

        clientSocket.close();
    }
}
