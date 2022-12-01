using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Suinet.Rpc;
using Suinet.Rpc.Client;
using Suinet.Rpc.Types;
using UnityEngine;

public class OnChainStateStore : MonoBehaviour
{
    public static OnChainStateStore Instance { get; private set; }
    public readonly Dictionary<string, OnChainPlayerState> States = new Dictionary<string, OnChainPlayerState>();
    public Transform playersParent;
    public Transform explosionsParent;
    public Transform trailCollidersParent;
    public OnChainPlayer remotePlayerPrefab;
    public TrailCollider trailColliderPrefab;
    
    private readonly Dictionary<string, OnChainPlayer> _remotePlayers = new Dictionary<string, OnChainPlayer>();
    private string _localPlayerAddress;
    private WebSocketService _webSocketService;
    //private const string WebsocketEndpoint = "ws://pubsub.devnet.sui.io:80";
    private const string WebsocketEndpoint = "wss://pubsub.devnet.sui.io:443";
    
    private void Awake()
    {
        Instance = this;
        _webSocketService = new WebSocketService();
        _webSocketService.StartConnection(WebsocketEndpoint);
        WebSocketActions.WebSocketEventAction += OnWebSocketEvent;
        WebSocketActions.CloseWebSocketConnectionAction += OnCloseWebSocketConnection;
    }

    private void Start()
    {
        SetLocalPlayerAddress();
        _webSocketService.SubscribeToEvents("{\"MoveEventType\":\"" + Constants.PACKAGE_OBJECT_ID + "::playerstate_module::PlayerStateUpdatedEvent\"}");
    }
    
    private void SetLocalPlayerAddress()
    {
        if (string.IsNullOrWhiteSpace(_localPlayerAddress))
        {
            _localPlayerAddress = SuiWallet.GetActiveAddress();
        }
        if (string.IsNullOrWhiteSpace(_localPlayerAddress))
        {
            Debug.LogError("Could not retrieve active Sui address");
        }
    }
    
    private void OnCloseWebSocketConnection()
    {
        Debug.Log("OnCloseWebSocketConnection");

    }

    private void OnWebSocketEvent(string message)
    {
//        Debug.Log(message);
        SetLocalPlayerAddress();

        var messageData = JsonConvert.DeserializeObject<WebsocketMessage>(message);
        var eventData = messageData?.Params?.Result;
        if (eventData?.Event.MoveEvent == null) return;

        var sender = eventData.Event.MoveEvent.Sender;
        var bcs = eventData.Event.MoveEvent.Bcs;

        // BCS conversion
        var bytes = Convert.FromBase64String(bcs);
        var posX64 = BitConverter.ToUInt64(bytes, 0);
        var posY64 = BitConverter.ToUInt64(bytes, 8);
        var velX64 = BitConverter.ToUInt64(bytes, 16);
        var velY64 = BitConverter.ToUInt64(bytes, 24);
        var sequenceNumber = BitConverter.ToUInt64(bytes, 32);
        var isExploded = BitConverter.ToBoolean(bytes, 40);

        var position = new OnChainVector2(posX64, posY64);
        var velocity = new OnChainVector2(velX64, velY64);

        var state = new OnChainPlayerState(position, velocity, sequenceNumber, isExploded);

        var isLocalSender = sender == _localPlayerAddress;

        if (States.ContainsKey(sender))
        {
            if (isLocalSender && isExploded)
            {
                States.Remove(sender);
            }
            else if (sequenceNumber > States[sender].SequenceNumber)
            {
                States[sender] = state;
            }
        }
        else if (!isExploded)
        {
            States.Add(sender, state);
        }

        UpdateRemotePlayers();
        // Debug.Log($"OnChainUpdate: {position.ToVector3()}. sequenceNumber: {sequenceNumber}. sender: {sender}. isExploded:{ isExploded}. States.ContainsKey(sender): {States.ContainsKey(sender)} ");
        // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // cube.transform.position = position.ToVector3() + Vector3.back;
    }

    // todo move this to sdk
    public class WebsocketMessage
    {
        public string Method { get; set; }

        public WebsocketMessageParams Params { get; set; }
    }

    public class WebsocketMessageParams
    {
        public ulong Subscription { get; set; }
        public SuiEventEnvelope Result { get; set; }
    }
    
    private async Task GetOnChainUpdateEventsAsync()
    {
//        Debug.Log("GetOnChainUpdateEventsAsync");
        SetLocalPlayerAddress();
        var query = new SuiMoveEventEventQuery()
        {
            MoveEvent = $"{Constants.PACKAGE_OBJECT_ID}::playerstate_module::PlayerStateUpdatedEvent"
        };
        
        RpcResult<SuiPage_for_EventEnvelope_and_EventID> rpcResult;

        // if (_latestEvent != null)
        // {
        //     rpcResult = await SuiApi.Client.GetEventsAsync(query, _nextCursor, 20, false);
        // }
        // else
        { 
            // start from the latest event
            rpcResult = await SuiApi.Client.GetEventsAsync(query, null, 20, true);
        }
        
        Debug.Log(JsonConvert.SerializeObject(rpcResult));
        if (rpcResult != null && rpcResult.IsSuccess)
        {
            foreach (var eventData in rpcResult.Result.Data)
            {
                if (eventData.Event.MoveEvent != null)
                {
                    //Debug.Log("GetOnChainUpdateEventsAsync: " + JsonConvert.SerializeObject(eventData.Event.MoveEvent));

                    var sender = eventData.Event.MoveEvent.Sender;
                    var bcs = eventData.Event.MoveEvent.Bcs;
                    var timeStamp = eventData.Timestamp;

                    // BCS conversion
                    var bytes = Convert.FromBase64String(bcs);
                    var posX64 = BitConverter.ToUInt64(bytes, 0);
                    var posY64 = BitConverter.ToUInt64(bytes, 8);
                    var velX64 = BitConverter.ToUInt64(bytes, 16);
                    var velY64 = BitConverter.ToUInt64(bytes, 24);
                    var sequenceNumber = BitConverter.ToUInt64(bytes, 32);
                    var isExploded = BitConverter.ToBoolean(bytes, 40);

                    var position = new OnChainVector2(posX64, posY64);
                    var velocity = new OnChainVector2(velX64, velY64);

                    var state = new OnChainPlayerState(position, velocity, sequenceNumber, isExploded);

                    bool isLocalSender = sender == _localPlayerAddress;
                    
                    if (States.ContainsKey(sender)) 
                    {
                        if (isLocalSender && isExploded)
                        {
                            States.Remove(sender);
                        }
                        else if (sequenceNumber > States[sender].SequenceNumber)
                        {
                            States[sender] = state;
                        }
                    }
                    else if (!isExploded)
                    {
                        States.Add(sender, state);
                    }
                    
                   // Debug.Log($"OnChainUpdate: {position.ToVector3()}. sequenceNumber: {sequenceNumber}. sender: {sender}. isExploded:{ isExploded}. States.ContainsKey(sender): {States.ContainsKey(sender)} ");
                    // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // cube.transform.position = position.ToVector3() + Vector3.back;

                }
            }
        }

        UpdateRemotePlayers();
    }

    private void UpdateRemotePlayers()
    {
        foreach (var state in States)
        {
            if (state.Key != _localPlayerAddress)
            {
                if (!_remotePlayers.ContainsKey(state.Key))
                {
                    var remotePlayerGo = Instantiate(remotePlayerPrefab, playersParent);
                    remotePlayerGo.ownerAddress = state.Key;
                    remotePlayerGo.GetComponent<ExplosionController>().explosionRoot = explosionsParent;
                    remotePlayerGo.gameObject.SetActive(true);
                    _remotePlayers.Add(state.Key, remotePlayerGo);
                    
                    var trailColliderGo =  Instantiate(trailColliderPrefab, trailCollidersParent);
                    trailColliderGo.ownerAddress = state.Key;
                    trailColliderGo.gameObject.SetActive(true);
                }
            }
        }
    }

    public void RemoveRemotePlayer(string address)
    {
        States.Remove(address);
        _remotePlayers.Remove(address);
    }
}
