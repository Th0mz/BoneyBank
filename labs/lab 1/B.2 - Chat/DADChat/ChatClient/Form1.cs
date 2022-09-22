using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grpc.Net.Client;
using Grpc.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        // client info
        string name;
        string port;

        // services and communication channel
        private GrpcChannel channel;
        private ChatServerService.ChatServerServiceClient client;

        Server server;

        public Form1()
        {
            InitializeComponent();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress("http://localhost:1001");
            client = new ChatServerService.ChatServerServiceClient(channel);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox2.Text;
            port = textBox1.Text;

            if (name == "" || port == "")
            {
                MessageBox.Show("Invalid name or prot", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var reply = client.Register(
             new ChatServerRegisterRequest { Nick = name, Url = "http://localhost:" + port });

            if (reply == null || !reply.Ok)
            {
                MessageBox.Show("User with name " + name + " already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = false;

            button1.Enabled = false;
            button3.Enabled = true;

            // open message recieving port 
            server = new Server
            {
                Services = { ChatClientService.BindService(new ClientService()) },
                Ports = { new ServerPort("localhost", Int32.Parse(port), ServerCredentials.Insecure) }
            };
            server.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string text = $"[ {name} ] : {textBox3.Text}";

            if (textBox3.Text == "")
            {
                return;
            }

            var reply = client.Broadcast(
                new ChatServerBroadcastRequest { Nick = name, Message = text });

            if (reply.Ok)
            {
                listBox1.Items.Add(text);

                textBox3.Text = "";
            }
        }
    }
}
