using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.SI;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Servidor
{
    internal class Program
    {
        private const int PORT = 8500;

        static void Main(string[] args)
        {
            //criar um conjunto ip + porto para servidor
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            //criar um TCP listener
            TcpListener listener = new TcpListener(endPoint);
            //ficar a escuta de pedidos de ligacao
            listener.Start();
            Console.WriteLine("Servidor Pronto!!!");
            int clientCounter = 0;

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clientCounter++;
                Console.WriteLine("Cliente {0} conectado!!!", clientCounter);
                ClientHandler clientHandler = new ClientHandler(client, clientCounter);
                clientHandler.Handle();
            }

        }

        class ClientHandler
        {
            private TcpClient client;
            private int clientID;

            public ClientHandler(TcpClient client, int clientID)
            {
                this.client = client;
                this.clientID = clientID;
            }

            public void Handle()
            {
                Thread thread = new Thread(threadHandler);
                thread.Start();
            }

            private void threadHandler()
            {
                NetworkStream networkStream = this.client.GetStream();
                ProtocolSI protocolSI = new ProtocolSI();

                while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    byte[] ack;
                    byte[] ack1;
                    switch (protocolSI.GetCmdType())
                    {
                        case ProtocolSICmdType.DATA:
                            Console.WriteLine("Cliente " + clientID + " : " + protocolSI.GetStringFromData());
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            //Criar a resposta
                            ack1 = Encoding.UTF8.GetBytes("Cliente " + clientID + " : " + protocolSI.GetStringFromData());
                            //Enviar a resposta para o cliente
                            networkStream.Write(ack1, 0, ack1.Length);
                            break;
                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("Encerrando a ligação com o cliente " + clientID);
                            ack = protocolSI.Make(ProtocolSICmdType.ACK);
                            networkStream.Write(ack, 0, ack.Length);
                            break;

                    }
                }

                networkStream.Close();
                client.Close();

            }
        }
    }
}
