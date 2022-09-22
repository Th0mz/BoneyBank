// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/ChatServicesServer.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

public static partial class ChatServerService
{
  static readonly string __ServiceName = "ChatServerService";

  static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
  {
    #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
    if (message is global::Google.Protobuf.IBufferMessage)
    {
      context.SetPayloadLength(message.CalculateSize());
      global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
      context.Complete();
      return;
    }
    #endif
    context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
  }

  static class __Helper_MessageCache<T>
  {
    public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
  }

  static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
  {
    #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
    if (__Helper_MessageCache<T>.IsBufferMessage)
    {
      return parser.ParseFrom(context.PayloadAsReadOnlySequence());
    }
    #endif
    return parser.ParseFrom(context.PayloadAsNewBuffer());
  }

  static readonly grpc::Marshaller<global::ChatServerRegisterRequest> __Marshaller_ChatServerRegisterRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ChatServerRegisterRequest.Parser));
  static readonly grpc::Marshaller<global::ChatServerRegisterReply> __Marshaller_ChatServerRegisterReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ChatServerRegisterReply.Parser));
  static readonly grpc::Marshaller<global::ChatServerBroadcastRequest> __Marshaller_ChatServerBroadcastRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ChatServerBroadcastRequest.Parser));
  static readonly grpc::Marshaller<global::ChatServerBroadcastReply> __Marshaller_ChatServerBroadcastReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::ChatServerBroadcastReply.Parser));

  static readonly grpc::Method<global::ChatServerRegisterRequest, global::ChatServerRegisterReply> __Method_Register = new grpc::Method<global::ChatServerRegisterRequest, global::ChatServerRegisterReply>(
      grpc::MethodType.Unary,
      __ServiceName,
      "Register",
      __Marshaller_ChatServerRegisterRequest,
      __Marshaller_ChatServerRegisterReply);

  static readonly grpc::Method<global::ChatServerBroadcastRequest, global::ChatServerBroadcastReply> __Method_Broadcast = new grpc::Method<global::ChatServerBroadcastRequest, global::ChatServerBroadcastReply>(
      grpc::MethodType.Unary,
      __ServiceName,
      "Broadcast",
      __Marshaller_ChatServerBroadcastRequest,
      __Marshaller_ChatServerBroadcastReply);

  /// <summary>Service descriptor</summary>
  public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
  {
    get { return global::ChatServicesServerReflection.Descriptor.Services[0]; }
  }

  /// <summary>Client for ChatServerService</summary>
  public partial class ChatServerServiceClient : grpc::ClientBase<ChatServerServiceClient>
  {
    /// <summary>Creates a new client for ChatServerService</summary>
    /// <param name="channel">The channel to use to make remote calls.</param>
    public ChatServerServiceClient(grpc::ChannelBase channel) : base(channel)
    {
    }
    /// <summary>Creates a new client for ChatServerService that uses a custom <c>CallInvoker</c>.</summary>
    /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
    public ChatServerServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
    {
    }
    /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
    protected ChatServerServiceClient() : base()
    {
    }
    /// <summary>Protected constructor to allow creation of configured clients.</summary>
    /// <param name="configuration">The client configuration.</param>
    protected ChatServerServiceClient(ClientBaseConfiguration configuration) : base(configuration)
    {
    }

    public virtual global::ChatServerRegisterReply Register(global::ChatServerRegisterRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return Register(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual global::ChatServerRegisterReply Register(global::ChatServerRegisterRequest request, grpc::CallOptions options)
    {
      return CallInvoker.BlockingUnaryCall(__Method_Register, null, options, request);
    }
    public virtual grpc::AsyncUnaryCall<global::ChatServerRegisterReply> RegisterAsync(global::ChatServerRegisterRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return RegisterAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual grpc::AsyncUnaryCall<global::ChatServerRegisterReply> RegisterAsync(global::ChatServerRegisterRequest request, grpc::CallOptions options)
    {
      return CallInvoker.AsyncUnaryCall(__Method_Register, null, options, request);
    }
    public virtual global::ChatServerBroadcastReply Broadcast(global::ChatServerBroadcastRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return Broadcast(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual global::ChatServerBroadcastReply Broadcast(global::ChatServerBroadcastRequest request, grpc::CallOptions options)
    {
      return CallInvoker.BlockingUnaryCall(__Method_Broadcast, null, options, request);
    }
    public virtual grpc::AsyncUnaryCall<global::ChatServerBroadcastReply> BroadcastAsync(global::ChatServerBroadcastRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return BroadcastAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual grpc::AsyncUnaryCall<global::ChatServerBroadcastReply> BroadcastAsync(global::ChatServerBroadcastRequest request, grpc::CallOptions options)
    {
      return CallInvoker.AsyncUnaryCall(__Method_Broadcast, null, options, request);
    }
    /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
    protected override ChatServerServiceClient NewInstance(ClientBaseConfiguration configuration)
    {
      return new ChatServerServiceClient(configuration);
    }
  }

}
#endregion
