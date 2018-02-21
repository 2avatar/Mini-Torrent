using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Peer2Peer
{
    public class StateObject1
    {
        public Socket WorkSocket = null;

        public const int BufferSize = 5242880;

        public byte[] Buffer = new byte[BufferSize];

        public bool Connected = false;

    }

    public class StateObject2 
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
}
