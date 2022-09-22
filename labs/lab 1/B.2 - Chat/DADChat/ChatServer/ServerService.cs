using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading.Channels;
using Grpc.Net.Client;
using static System.Net.Mime.MediaTypeNames;

public class ServerService : ChatServerService.ChatServerServiceBase
{
    private Dictionary<string, string> clientMap = new Dictionary<string, string>();
    
    // messaging channell 
    private GrpcChannel channel;
    private ChatClientService.ChatClientServiceClient client;

    public ServerService()
    {
    }

    // register 
    public override Task<ChatServerRegisterReply> Register(
        ChatServerRegisterRequest request, ServerCallContext context)
    {
        return Task.FromResult(do_register(request));
    }

    public ChatServerRegisterReply do_register(ChatServerRegisterRequest request)
    {
        lock (this)
        {
            if (clientMap.ContainsKey(request.Nick)) {
                return new ChatServerRegisterReply { Ok = false };
            }
            
            clientMap.Add(request.Nick, request.Url);
        }

        Console.WriteLine($"Registered client {request.Nick} with URL {request.Url}");
        return new ChatServerRegisterReply { Ok = true };
    }


    // broadcast
    public override Task<ChatServerBroadcastReply> Broadcast (
    ChatServerBroadcastRequest request, ServerCallContext context)
    {
        return Task.FromResult(do_broadcast(request));
    }

    public ChatServerBroadcastReply do_broadcast (ChatServerBroadcastRequest request)
    {
        // send the message to all the users in the chat
        foreach (string name in clientMap.Keys) {
            if (name == request.Nick) {
                continue;
            }

            string address = clientMap[name];

            Console.WriteLine($"creating channel with {address}");
            channel = GrpcChannel.ForAddress(address);

            Console.WriteLine($"client is being created");
            client = new ChatClientService.ChatClientServiceClient(channel);

            Console.WriteLine($"sent broadcast message");
            var reply = client.Broadcast(
                new ChatClientBroadcastRequest { Nick = request.Nick, Message = request.Message });

            Console.WriteLine($"recieved confirmation {reply.Ok}");


        }

        Console.WriteLine($"sent message to all {clientMap.Count - 1} online users");
        return new ChatServerBroadcastReply { Ok = true };
    }
}