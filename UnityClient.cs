using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using TestPhotonLib.Common;
using TestPhotonLib.Operations;

namespace TestPhotonLib
{
    public class UnityClient : ClientPeer
    {
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();
        public string Charactername { get; private set; }


        public UnityClient(InitRequest initRequest) : base(initRequest)
        {
            Log.Info("Player connection ip:" + initRequest.RemoteIP);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            World.Inctance.RemoveClient(this);
            Log.Debug("Disconnected");
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case (byte)OperationCode.Login:

                    var loginRequest = new Login(Protocol, operationRequest);
                    if (!loginRequest.IsValid)
                    {
                        SendOperationResponse(loginRequest.GetResponse(ErrorCode.InvalidParameters), sendParameters);
                        return;
                    }
                    Charactername = loginRequest.CharacterName;
                    //proof for user with that name already exists
                    if (World.Inctance.IsContain(Charactername))
                    {
                        SendOperationResponse(loginRequest.GetResponse(ErrorCode.NameIsExist), sendParameters);
                        return;
                    }
                    //add new client
                    World.Inctance.AddClient(this);
                    //answer to client if client was added
                    var response = new OperationResponse(operationRequest.OperationCode);
                    SendOperationResponse(response, sendParameters);
                    Log.Info("user with name: " + Charactername);
                    //if (operationRequest.Parameters.ContainsKey(1))
                    //{
                    //    Log.Debug("rect: " + operationRequest.Parameters[1]);
                    //    OperationResponse response = new OperationResponse(operationRequest.OperationCode)
                    //    {
                    //        Parameters = new Dictionary<byte, object> { { 1, "response message" } }
                    //    };
                    //    SendOperationResponse(response, sendParameters);
                    //}
                    break;
                case (byte)OperationCode.SendChatMessage:
                    {
                        var chatRequest = new ChatMessage(Protocol, operationRequest);
                        if (!chatRequest.IsValid)
                        {
                            SendOperationResponse(chatRequest.GetResponse(ErrorCode.InvalidParameters), sendParameters);
                            return;
                        }
                        string message = chatRequest.Message;
                        string[] param = message.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        //private messages don't saves in log
                        //if 2 param - than it's PRIVATE MESSAGE
                        if (param.Length == 2)
                        {
                            string targetName = param[0];
                            message = param[1];
                            if (World.Inctance.IsContain(targetName))
                            {
                                var targetClient = World.Inctance.TryGetByName(targetName);
                                if (targetClient == null)
                                    return;
                                message = Charactername + "[PM]:" + message;
                                var personalEventData = new EventData((byte)EventCode.ChatMessage);
                                personalEventData.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.ChatMessage, message } };
                                personalEventData.SendTo(new UnityClient[] { this, targetClient }, sendParameters);
                            }
                            return;
                        }

                        message = Charactername + ": " + message;
                        //private messages don't saves in log
                        Chat.Instance.AddMessage(message);

                        var eventData = new EventData((byte)EventCode.ChatMessage);
                        eventData.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.ChatMessage, message } };
                        //SendEvent(eventData, sendParameters);//send only for current (one) client
                        eventData.SendTo(World.Inctance.Clients, sendParameters);
                    }
                    break;
                case (byte)OperationCode.GetRecentChatMessages:
                    {
                        //get messages with reverce order....easy to fix....
                        string message = Chat.Instance.GetResentMesseges().Aggregate((i, j) => i + "\r\n" + j);
                        var eventData = new EventData((byte)EventCode.ChatMessage);
                        eventData.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.ChatMessage, message } };
                        eventData.SendTo(new UnityClient[] { this }, sendParameters); 

                    }

                    break;
                default:
                    Log.Debug("Unknown OperationRequest recieved!:"+ operationRequest.OperationCode);
                    break;
                    
            }
        }
    }
}
