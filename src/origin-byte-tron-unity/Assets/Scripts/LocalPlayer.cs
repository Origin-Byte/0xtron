using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Suinet.Rpc;
using Suinet.Rpc.Types;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LocalPlayer : MonoBehaviour
{
    public float moveSpeed = 9.0f;

    private Rigidbody2D _rb;
    private Vector2 _lastPosition = Vector2.zero;
    private ulong _sequenceNumber;
    private ExplosionController _explosionController;
    private bool _scoreboardUpdated = false;
    private string _signer;
    private string _onChainPlayerStateObjectId;
    
    async void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _explosionController = GetComponent<ExplosionController>();
        _sequenceNumber = 0;
        _rb.velocity = Vector2.up * moveSpeed;
        _scoreboardUpdated = false;
        _signer = SuiWallet.GetActiveAddress();
        _onChainPlayerStateObjectId = await CreateOnChainPlayerStateAsync();
        StartCoroutine(UpdateOnChainPlayerStateWorker());
    }

    void Update()
    {
        var dir = 0f;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir = -1f;
        }

        if (dir != 0f)
        {
            var currentRot = transform.rotation.eulerAngles;
            currentRot.z += 90.0f * dir;
            transform.rotation = Quaternion.Euler(currentRot);
            _rb.velocity = _rb.velocity.Rotate(90.0f * dir);
        }
    }

    private IEnumerator UpdateOnChainPlayerStateWorker() 
    { 
        while (true)
        {
            var position = _rb.position;
            var velocity = _rb.velocity;
            var task = UpdateOnChainPlayerStateAsync(position, velocity);
            yield return new WaitUntil(()=> task.IsCompleted);
        }
    }
     
    private async Task UpdateOnChainPlayerStateAsync(Vector2 position, Vector2 velocity)
    {
        if (_lastPosition == position && velocity.magnitude < 1.0f)
        {
           return;
        }
        
        if (string.IsNullOrWhiteSpace(_onChainPlayerStateObjectId))
        {
            Debug.LogError("onChainStateObjectId is null UpdateOnChainPlayerStateAsync early return");
            return;
        }

        //Debug.Log($"lp position: {position}, velocity: {velocity}");
        var onChainPosition = new OnChainVector2(position);
        var onChainVelocity = new OnChainVector2(velocity);
       // Debug.Log($"lp onChainPosition.x: {onChainPosition.x}, onChainPosition.y: {onChainPosition.y}, onChainVelocity.x: {onChainVelocity.x}, onChainVelocity.y {onChainVelocity.y}. isExploded: {_explosionController.IsExploded}");

       var moveCallTx = new MoveCallTransaction()
        {
            Signer = _signer,
            PackageObjectId = Constants.PACKAGE_OBJECT_ID,
            Module = Constants.MODULE_NAME,
            Function = "do_update",
            TypeArguments = ArgumentBuilder.BuildTypeArguments(),
            Arguments = ArgumentBuilder.BuildArguments( _onChainPlayerStateObjectId, onChainPosition.x, onChainPosition.y, onChainVelocity.x, onChainVelocity.y, _sequenceNumber++, _explosionController.IsExploded, TimestampService.UtcTimestamp ),
            Gas = await GetGasObjectId(_signer),
            GasBudget = 5000,
            RequestType = SuiExecuteTransactionRequestType.ImmediateReturn
        };
           
        await SuiApi.Signer.SignAndExecuteMoveCallAsync(moveCallTx);
        
        _lastPosition = position;

        if (_explosionController.IsExploded && ! _scoreboardUpdated)
        {
            await UpdateScoreboard(_onChainPlayerStateObjectId);
        }
    }

    private async Task<string> CreateOnChainPlayerStateAsync()
    {
        var moveCallTx = new MoveCallTransaction()
        {
            Signer = _signer,
            PackageObjectId = Constants.PACKAGE_OBJECT_ID,
            Module = Constants.MODULE_NAME,
            Function = "create_playerstate_for_sender",
            TypeArguments = ArgumentBuilder.BuildTypeArguments(),
            Arguments = ArgumentBuilder.BuildArguments( TimestampService.UtcTimestamp ),
            Gas = await GetGasObjectId(_signer),
            GasBudget = 5000,
            RequestType = SuiExecuteTransactionRequestType.WaitForEffectsCert
        };
           
        var txRpcResult = await SuiApi.Signer.SignAndExecuteMoveCallAsync(moveCallTx);
        
        var createdObjectId = ""; 
 
        if (txRpcResult.IsSuccess) 
        { 
            createdObjectId = txRpcResult.Result.EffectsCert.Effects.Effects.Created.First().Reference.ObjectId;
            Debug.Log("CreatedOnChainPlayerStateAsync. createdObjectId: " + createdObjectId);
        } 
        else 
        { 
            Debug.LogError("Something went wrong when executing the transaction: " + txRpcResult.ErrorMessage); 
        }

        return createdObjectId; 
    }
    
    private async Task UpdateScoreboard(string onChainStateObjectId)
    {
        var moveCallTx = new MoveCallTransaction()
        {
            Signer = _signer,
            PackageObjectId = Constants.PACKAGE_OBJECT_ID,
            Module = Constants.MODULE_NAME,
            Function = "add_to_scoreboard",
            TypeArguments = ArgumentBuilder.BuildTypeArguments(),
            Arguments = ArgumentBuilder.BuildArguments( onChainStateObjectId, Constants.SCOREBOARD_OBJECT_ID ),
            Gas = await GetGasObjectId(_signer),
            GasBudget = 5000,
            RequestType = SuiExecuteTransactionRequestType.ImmediateReturn
        };
           
        await SuiApi.Signer.SignAndExecuteMoveCallAsync(moveCallTx);
        
        _scoreboardUpdated = true;
    }
    
    private async Task<string> GetGasObjectId(string address)
    {
        var gasObjectId = ""; 
        if (PlayerPrefs.HasKey("gasObjectId")) 
        { 
            gasObjectId = PlayerPrefs.GetString("gasObjectId"); 
        } 
        else 
        { 
            gasObjectId = (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(SuiApi.Client, address, 1, 10000))[0]; 
            PlayerPrefs.SetString("gasObjectId", gasObjectId); 
        }

        return gasObjectId;
    }
}
