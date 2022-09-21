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

namespace ChatClient {
    public partial class Form1 : Form {
        private GrpcChannel channel;
        private ChatServerService.ChatServerServiceClient client;
        public Form1() {
            InitializeComponent();

            AppContext.SetSwitch(
    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress("http://localhost:1001");
            client = new ChatServerService.ChatServerServiceClient(channel);

        }

        private void button1_Click(object sender, EventArgs e) {
            var reply = client.Register(
                         new ChatClientRegisterRequest { Nick = textBox2.Text, Url = "http://localhost:....." });
            textBox1.Text = reply.Ok.ToString();
        }
    }
}
