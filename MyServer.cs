﻿using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using Photon.SocketServer;
using System;
using System.IO;
using TestPhotonLib.Common;

namespace TestPhotonLib
{
    public class MyServer : ApplicationBase
    {

        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new UnityClient(initRequest);
        }

        protected override void Setup()
        {
            var file = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if(file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }

            Log.Info("Server is ready!");


        }

        protected override void TearDown()
        {
            Log.Debug("Server is stoped!");
        }
    }
}
