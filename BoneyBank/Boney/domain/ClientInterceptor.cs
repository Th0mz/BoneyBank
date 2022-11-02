using Grpc.Core.Interceptors;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Grpc.Core.Interceptors.Interceptor;

namespace Boney
{
    public class ClientInterceptor : Interceptor
    {
        private ServerState _serverState;
        private ManualResetEvent _event;

        public ClientInterceptor(ServerState serverState) {
            _serverState = serverState;
            _event = new ManualResetEvent(!serverState.is_frozen());
        }

        public void freeze_channel() {
            _event.Reset();
        }

        public void unfreeze_channel() {
            _event.Set();
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation) {

            //set para dar unfreeze, senao vai reset
            //depois falta codigo para mudar o estado freeze e unfreeze
            if (_serverState.is_frozen()) {
                Console.WriteLine("Frozen waiting...");
                _event.WaitOne();
            }

            AsyncUnaryCall<TResponse> response = base.AsyncUnaryCall(request, context, continuation);
            return response;
        }
    }
}
