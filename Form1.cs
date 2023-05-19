using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using EI.SI;

namespace ProjetoFinalTS
{
    public partial class Form1 : Form
    {

        private const int PORT = 8500;
        NetworkStream networkStream;
        ProtocolSI protocolSI;
        TcpClient client;

        public Form1()
        {
            InitializeComponent();
            //criar um conjunto ip  +  port do servidor remoto
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            //intanciar o clinte TCP
            client = new TcpClient();
            //efetuar a ligaçao ao servidor
            client.Connect(endPoint);
            networkStream = client.GetStream();
            protocolSI = new ProtocolSI();

        }


        private void CloseClient()
        {
            byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
            networkStream.Write(eot, 0, eot.Length);
            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            networkStream.Close();
            client.Close();

        }

        private void buttonEnviar_Click(object sender, EventArgs e)
        {
            string msg = textBoxMensagem.Text;
            textBoxMensagem.Clear();
            //preparar a mensagem para enviar para o servidor
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msg);
            networkStream.Write(packet, 0, packet.Length);
            //escutar mensagem servidor
            while (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            }
            ligacao(IPAddress.Loopback, PORT);
        }

        private void buttonSair_Click(object sender, EventArgs e)
        {
            CloseClient();
            this.Close();
        }

        private void ligacao(IPAddress ip, int port)
        {

            
            networkStream = client.GetStream();

            byte[] ack;

            string response = "";

            try
            {

                //receber a confirmaçao do servidor
                ack = new byte[client.ReceiveBufferSize];
                //receber resposta do servidor 
                int bytesRead = networkStream.Read(ack, 0, ack.Length);

                response = Encoding.UTF8.GetString(ack, 0, ack.Length);

                if (response == "")
                {
                    textBoxChat.Text = "erro response vazio!";
                    //return "erro";
                }
                //devolve a resposta recebida
                //return response;
                textBoxChat.Text =  response;



            }
            catch (Exception ex)
            {
                //algo estranho aconteceu e devolve o erro
                //return "Erro: " + ex.Message ;
            }
        }

        private void tabPageChat_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {

        }
    }
}
