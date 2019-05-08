﻿using System;
using System.Collections.Generic;
using System.Text;
using SocketServerLib.Client;
using SocketServerLib.SocketHandler;

namespace AsyncClientServerLib.Client
{
    /// <summary>
    /// Basic Socket Client. Implements the AbstractSocketClient. The socket client handler class is AsyncSocketClientHandlerReceieveQueue.
    /// </summary>
    public class BasicSocketClient : AbstractSocketClient
    {
        /// <summary>
        /// Return a BasicSocketClientHandler instance.
        /// </summary>
        /// <param name="handler">The client socket handler</param>
        /// <param name="stream">The ssl stream</param>
        /// <param name="sendHandleTimeout">The send timeout</param>
        /// <param name="socketSendTimeout">The socket timeout</param>
        /// <returns>The BasicSocketClientHandler instance created</returns>
        protected override AbstractTcpSocketClientHandler GetHandler(System.Net.Sockets.Socket handler, System.Net.Security.SslStream stream)
        {
            return new AsyncClientServerLib.SocketHandler.AsyncSocketClientHandlerReceiveQueue(handler, stream);
        }
    }
}
