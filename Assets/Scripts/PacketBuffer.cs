using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


public class PacketBuffer
    {
        // Client socket.  
        public Socket currentSocket = null;

        // Size of receive buffer.  
        public const int BufferSize = 256;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Received data string.  
        public StringBuilder stringBuilder = new StringBuilder();
    }
