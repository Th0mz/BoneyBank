using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ChatClient;
using System.Windows.Forms;

public class ClientService : ChatClientService.ChatClientServiceBase
{
    public ClientService() { }

    // broadcast
    public override Task<ChatClientBroadcastReply> Broadcast(
    ChatClientBroadcastRequest request, ServerCallContext context)
    {
        return Task.FromResult(do_broadcast(request));
    }

    public ChatClientBroadcastReply do_broadcast(ChatClientBroadcastRequest request)
    {
        // update message list 
        
        return new ChatClientBroadcastReply { Ok = true };
    }
}
