using UnityEngine;
using Mirror.Discovery;
using System;
using UnityEngine.Events;
using System.Net;
using Mirror;

[Serializable]
public class CustomServerFoundUnityEvent : UnityEvent<CustomServerResponse> { };

public class CustomNetworkDiscovery : NetworkDiscoveryBase<ServerRequest, CustomServerResponse>
{
    private string hostName = string.Empty;

    #region Server

    public long ServerId { get; private set; }

    [Tooltip("Transport to be advertised during discovery")]
    public Transport transport;

    [Tooltip("Invoked when a server is found")]
    public CustomServerFoundUnityEvent OnServerFound;

    public override void Start()
    {
        ServerId = RandomLong();

        if (transport == null)
            transport = Transport.activeTransport;

        base.Start();
    }

    public void AdvertiseServer(string hostName)
    {
        this.hostName = hostName;
        EventSystems.EventManager.Instance.StartListening<RoomWindowToggleData>(Handle);
        AdvertiseServer();
    }

    private void Handle(RoomWindowToggleData data)
    {
        Debug.Log("HANDLE");
        StopAdvertisingServer();
        EventSystems.EventManager.Instance.StopListening<RoomWindowToggleData>(Handle);
    }

    public void StopAdvertisingServer()
    {
        if (serverUdpClient == null) return;
            
        try
        {
            serverUdpClient.Close();
        }
        catch (Exception)
        {
            // it is just close, swallow the error
        }

        serverUdpClient = null;           
    }

    protected override CustomServerResponse ProcessRequest(ServerRequest request, IPEndPoint endpoint)
    {
        try
        {
            return new CustomServerResponse
            {
                serverId = ServerId,
                uri = transport.ServerUri(),
                hostName = hostName
            };
        }
        catch (NotImplementedException)
        {
            Debug.LogError($"Transport {transport} does not support network discovery");
            throw;
        }
    }

    #endregion

    #region Client

        /// <summary>
        /// Create a message that will be broadcasted on the network to discover servers
        /// </summary>
        /// <remarks>
        /// Override if you wish to include additional data in the discovery message
        /// such as desired game mode, language, difficulty, etc... </remarks>
        /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
        protected override ServerRequest GetRequest() => new ServerRequest();

        /// <summary>
        /// Process the answer from a server
        /// </summary>
        /// <remarks>
        /// A client receives a reply from a server, this method processes the
        /// reply and raises an event
        /// </remarks>
        /// <param name="response">Response that came from the server</param>
        /// <param name="endpoint">Address of the server that replied</param>
        protected override void ProcessResponse(CustomServerResponse response, IPEndPoint endpoint)
        {
            // we received a message from the remote endpoint
            response.EndPoint = endpoint;

            // although we got a supposedly valid url, we may not be able to resolve
            // the provided host
            // However we know the real ip address of the server because we just
            // received a packet from it,  so use that as host.
            UriBuilder realUri = new UriBuilder(response.uri)
            {
                Host = response.EndPoint.Address.ToString()
            };
            response.uri = realUri.Uri;

            OnServerFound.Invoke(response);
        }

        #endregion
}
