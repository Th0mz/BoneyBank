using Grpc.Core.Interceptors;
using Grpc.Core;

namespace Boney
{
    public class FrozenInterceptor : Interceptor
    {
        private ServerState _serverState;
        private ManualResetEvent _event;

        public FrozenInterceptor(ServerState serverState) {
            _serverState = serverState;
            _event = new ManualResetEvent(!serverState.is_frozen());
        }

        public void toggle_freeze() {
            if (_serverState.is_frozen()) {
                freeze_channel();
            } else {
                unfreeze_channel();
            }
        }

        private void freeze_channel() {
            _event.Reset();
        }

        private void unfreeze_channel() {
            _event.Set();
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation) {

            Console.WriteLine("[client] Intercepted message");

            // freeze channel
            if (_serverState.is_frozen()) {
                Console.WriteLine("Frozen waiting...");
                _event.WaitOne();
            }

            AsyncUnaryCall<TResponse> response = base.AsyncUnaryCall(request, context, continuation);
            return response;
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation) {

            Console.WriteLine("[server] Intercepted message");
            // freeze channel
            if (_serverState.is_frozen()) {
                Console.WriteLine("Frozen waiting...");
                _event.WaitOne();
            }

            return base.UnaryServerHandler(request, context, continuation);

        }
    }
}
