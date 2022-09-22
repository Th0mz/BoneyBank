// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/ChatServicesServer.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from Protos/ChatServicesServer.proto</summary>
public static partial class ChatServicesServerReflection {

  #region Descriptor
  /// <summary>File descriptor for Protos/ChatServicesServer.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static ChatServicesServerReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Ch9Qcm90b3MvQ2hhdFNlcnZpY2VzU2VydmVyLnByb3RvIjYKGUNoYXRTZXJ2",
          "ZXJSZWdpc3RlclJlcXVlc3QSDAoEbmljaxgBIAEoCRILCgN1cmwYAiABKAki",
          "JQoXQ2hhdFNlcnZlclJlZ2lzdGVyUmVwbHkSCgoCb2sYASABKAgiOwoaQ2hh",
          "dFNlcnZlckJyb2FkY2FzdFJlcXVlc3QSDAoEbmljaxgBIAEoCRIPCgdtZXNz",
          "YWdlGAIgASgJIiYKGENoYXRTZXJ2ZXJCcm9hZGNhc3RSZXBseRIKCgJvaxgB",
          "IAEoCDKaAQoRQ2hhdFNlcnZlclNlcnZpY2USQAoIUmVnaXN0ZXISGi5DaGF0",
          "U2VydmVyUmVnaXN0ZXJSZXF1ZXN0GhguQ2hhdFNlcnZlclJlZ2lzdGVyUmVw",
          "bHkSQwoJQnJvYWRjYXN0EhsuQ2hhdFNlcnZlckJyb2FkY2FzdFJlcXVlc3Qa",
          "GS5DaGF0U2VydmVyQnJvYWRjYXN0UmVwbHliBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::ChatServerRegisterRequest), global::ChatServerRegisterRequest.Parser, new[]{ "Nick", "Url" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::ChatServerRegisterReply), global::ChatServerRegisterReply.Parser, new[]{ "Ok" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::ChatServerBroadcastRequest), global::ChatServerBroadcastRequest.Parser, new[]{ "Nick", "Message" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::ChatServerBroadcastReply), global::ChatServerBroadcastReply.Parser, new[]{ "Ok" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
/// <summary>
/// register
/// </summary>
public sealed partial class ChatServerRegisterRequest : pb::IMessage<ChatServerRegisterRequest>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<ChatServerRegisterRequest> _parser = new pb::MessageParser<ChatServerRegisterRequest>(() => new ChatServerRegisterRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ChatServerRegisterRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ChatServicesServerReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerRegisterRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerRegisterRequest(ChatServerRegisterRequest other) : this() {
    nick_ = other.nick_;
    url_ = other.url_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerRegisterRequest Clone() {
    return new ChatServerRegisterRequest(this);
  }

  /// <summary>Field number for the "nick" field.</summary>
  public const int NickFieldNumber = 1;
  private string nick_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Nick {
    get { return nick_; }
    set {
      nick_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "url" field.</summary>
  public const int UrlFieldNumber = 2;
  private string url_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Url {
    get { return url_; }
    set {
      url_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ChatServerRegisterRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ChatServerRegisterRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Nick != other.Nick) return false;
    if (Url != other.Url) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Nick.Length != 0) hash ^= Nick.GetHashCode();
    if (Url.Length != 0) hash ^= Url.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Nick.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Nick);
    }
    if (Url.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(Url);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Nick.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Nick);
    }
    if (Url.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(Url);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Nick.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Nick);
    }
    if (Url.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Url);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ChatServerRegisterRequest other) {
    if (other == null) {
      return;
    }
    if (other.Nick.Length != 0) {
      Nick = other.Nick;
    }
    if (other.Url.Length != 0) {
      Url = other.Url;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          Nick = input.ReadString();
          break;
        }
        case 18: {
          Url = input.ReadString();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          Nick = input.ReadString();
          break;
        }
        case 18: {
          Url = input.ReadString();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class ChatServerRegisterReply : pb::IMessage<ChatServerRegisterReply>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<ChatServerRegisterReply> _parser = new pb::MessageParser<ChatServerRegisterReply>(() => new ChatServerRegisterReply());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ChatServerRegisterReply> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ChatServicesServerReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerRegisterReply() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerRegisterReply(ChatServerRegisterReply other) : this() {
    ok_ = other.ok_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerRegisterReply Clone() {
    return new ChatServerRegisterReply(this);
  }

  /// <summary>Field number for the "ok" field.</summary>
  public const int OkFieldNumber = 1;
  private bool ok_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Ok {
    get { return ok_; }
    set {
      ok_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ChatServerRegisterReply);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ChatServerRegisterReply other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Ok != other.Ok) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Ok != false) hash ^= Ok.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Ok != false) {
      output.WriteRawTag(8);
      output.WriteBool(Ok);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Ok != false) {
      output.WriteRawTag(8);
      output.WriteBool(Ok);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Ok != false) {
      size += 1 + 1;
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ChatServerRegisterReply other) {
    if (other == null) {
      return;
    }
    if (other.Ok != false) {
      Ok = other.Ok;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          Ok = input.ReadBool();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          Ok = input.ReadBool();
          break;
        }
      }
    }
  }
  #endif

}

/// <summary>
/// broadcast
/// </summary>
public sealed partial class ChatServerBroadcastRequest : pb::IMessage<ChatServerBroadcastRequest>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<ChatServerBroadcastRequest> _parser = new pb::MessageParser<ChatServerBroadcastRequest>(() => new ChatServerBroadcastRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ChatServerBroadcastRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ChatServicesServerReflection.Descriptor.MessageTypes[2]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerBroadcastRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerBroadcastRequest(ChatServerBroadcastRequest other) : this() {
    nick_ = other.nick_;
    message_ = other.message_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerBroadcastRequest Clone() {
    return new ChatServerBroadcastRequest(this);
  }

  /// <summary>Field number for the "nick" field.</summary>
  public const int NickFieldNumber = 1;
  private string nick_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Nick {
    get { return nick_; }
    set {
      nick_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "message" field.</summary>
  public const int MessageFieldNumber = 2;
  private string message_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Message {
    get { return message_; }
    set {
      message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ChatServerBroadcastRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ChatServerBroadcastRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Nick != other.Nick) return false;
    if (Message != other.Message) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Nick.Length != 0) hash ^= Nick.GetHashCode();
    if (Message.Length != 0) hash ^= Message.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Nick.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Nick);
    }
    if (Message.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(Message);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Nick.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Nick);
    }
    if (Message.Length != 0) {
      output.WriteRawTag(18);
      output.WriteString(Message);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Nick.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Nick);
    }
    if (Message.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Message);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ChatServerBroadcastRequest other) {
    if (other == null) {
      return;
    }
    if (other.Nick.Length != 0) {
      Nick = other.Nick;
    }
    if (other.Message.Length != 0) {
      Message = other.Message;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          Nick = input.ReadString();
          break;
        }
        case 18: {
          Message = input.ReadString();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          Nick = input.ReadString();
          break;
        }
        case 18: {
          Message = input.ReadString();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class ChatServerBroadcastReply : pb::IMessage<ChatServerBroadcastReply>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<ChatServerBroadcastReply> _parser = new pb::MessageParser<ChatServerBroadcastReply>(() => new ChatServerBroadcastReply());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ChatServerBroadcastReply> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ChatServicesServerReflection.Descriptor.MessageTypes[3]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerBroadcastReply() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerBroadcastReply(ChatServerBroadcastReply other) : this() {
    ok_ = other.ok_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChatServerBroadcastReply Clone() {
    return new ChatServerBroadcastReply(this);
  }

  /// <summary>Field number for the "ok" field.</summary>
  public const int OkFieldNumber = 1;
  private bool ok_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Ok {
    get { return ok_; }
    set {
      ok_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ChatServerBroadcastReply);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ChatServerBroadcastReply other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Ok != other.Ok) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Ok != false) hash ^= Ok.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Ok != false) {
      output.WriteRawTag(8);
      output.WriteBool(Ok);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Ok != false) {
      output.WriteRawTag(8);
      output.WriteBool(Ok);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Ok != false) {
      size += 1 + 1;
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ChatServerBroadcastReply other) {
    if (other == null) {
      return;
    }
    if (other.Ok != false) {
      Ok = other.Ok;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          Ok = input.ReadBool();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          Ok = input.ReadBool();
          break;
        }
      }
    }
  }
  #endif

}

#endregion


#endregion Designer generated code