using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ChatClient;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class ClientService : ChatClientService.ChatClientServiceBase
{
    Form1 form;
    public ClientService(Form1 form1) { form = form1; }

    // broadcast
    public override Task<ChatClientBroadcastReply> Broadcast(
    ChatClientBroadcastRequest request, ServerCallContext context)
    {
        return Task.FromResult(do_broadcast(request));
    }

    public ChatClientBroadcastReply do_broadcast(ChatClientBroadcastRequest request)
    {
        // update message list 
        form.add_message(request.Message);
        return new ChatClientBroadcastReply { Ok = true };
    }
}
