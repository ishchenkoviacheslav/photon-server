using System.Collections.Generic;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace TestPhotonLib
{
    public class UnityClient : ClientPeer
    {
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public UnityClient(InitRequest initRequest) : base(initRequest)
        {
            Log.Info("Player connection ip:" + initRequest.RemoteIP);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Debug("Disconnected");
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case 1:
                    if (operationRequest.Parameters.ContainsKey(1))
                    {
                        Log.Debug("rect: " + operationRequest.Parameters[1]);
                        OperationResponse response = new OperationResponse(operationRequest.OperationCode)
                        {
                            Parameters = new Dictionary<byte, object> { { 1, "response message" } }
                        };
                        SendOperationResponse(response, sendParameters);
                    }
                    break;
                case 2:
                    if (operationRequest.Parameters.ContainsKey(1))
                    {
                        Log.Debug("rect: " + operationRequest.Parameters[1]);
                        EventData eventData = new EventData(1)
                        {
                            Parameters = new Dictionary<byte, object> {{1, "response for event"}}
                        };
                        SendEvent(eventData, sendParameters);
                    }
                    break;
                default:
                    Log.Debug("Unknown OperationRequest recieved!:"+ operationRequest.OperationCode);
                    break;
                    
            }
        }
    }
}
