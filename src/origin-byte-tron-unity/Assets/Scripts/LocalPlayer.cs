using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Suinet.Rpc;
using Suinet.Rpc.Types;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class LocalPlayer : MonoBehaviour
{
    public float moveSpeed = 9.0f;
    public float enableMovementDelay = 11;

    private Rigidbody2D _rb;
    private Vector2 _lastPosition = Vector2.zero;
    private ulong _sequenceNumber;
    private ExplosionController _explosionController;
    private bool _scoreboardUpdated = false;
    private string _signer;
    private string _onChainPlayerStateObjectId;
    private bool _isInitialized;
    private float _delayCounter;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _explosionController = GetComponent<ExplosionController>();
        _sequenceNumber = 0;
        _scoreboardUpdated = false;
        _signer = SuiWallet.GetActiveAddress();
        _isInitialized = false;
        _onChainPlayerStateObjectId = string.Empty;
        Random.InitState((int)(TimestampService.UtcTimestamp % Int32.MaxValue));
        _delayCounter = enableMovementDelay;
        StartCoroutine(Countdown());
        StartCoroutine(InitializePlayerStateWithRetry(0.1f));
    }

    void Update()
    {
        if (!_isInitialized) return;
        
        var dir = 0f;
        if (CameraManager.IsTopDownCameraMode)
        {
            var currentRot = transform.rotation.eulerAngles;
            
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (FastApproximately(currentRot.z, 0.0f))
                {
                    dir = 1f;
                }
                else if (FastApproximately(currentRot.z, 180.0f) || FastApproximately(currentRot.z, -180.0f))
                {
                    dir = -1f;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (FastApproximately(currentRot.z, 0.0f))
                {
                    dir = -1f;
                }
                else if (FastApproximately(currentRot.z, 180.0f) || FastApproximately(currentRot.z, -180.0f))
                {
                    dir = 1f; 
                }
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (FastApproximately(currentRot.z, 90.0f) || FastApproximately(currentRot.z, -270.0f))
                {
                    dir = -1f;
                }
                else if (FastApproximately(currentRot.z, 270.0f) || FastApproximately(currentRot.z, -90.0f))
                {
                    dir = 1f; 
                }
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) && (FastApproximately(currentRot.z,0.0f) || FastApproximately(currentRot.z,180.0f)))
            {
                if (FastApproximately(currentRot.z, 90.0f) || FastApproximately(currentRot.z, -270.0f))
                {
                    dir = 1f;
                }
                else if (FastApproximately(currentRot.z, 270.0f) || FastApproximately(currentRot.z, -90.0f))
                {
                    dir = -1f; 
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                dir = 1f;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                dir = -1f;
            }
        }

        if (dir != 0f)
        {
            var currentRot = transform.rotation.eulerAngles;
            currentRot.z += 90.0f * dir;
            transform.rotation = Quaternion.Euler(currentRot);
            _rb.velocity = _rb.velocity.Rotate(90.0f * dir);
        }
    }
    
    public static bool FastApproximately(float a, float b, float threshold = 0.01f)
    {
        if (threshold > 0f)
        {
            return Mathf.Abs(a - b) <= threshold;
        }
        else
        {
            return FastApproximately(a, b);
        }
    }
    
    private IEnumerator Countdown()
    {
        while (_delayCounter > 0)
        {
            _delayCounter--;
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator InitializePlayerStateWithRetry(float retryPeriod)
    {
        do
        {
            PlayerPrefs.DeleteKey(Constants.GAS_OBJECT_ID_KEY);
            var task = CreateOnChainPlayerStateAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            _onChainPlayerStateObjectId = task.Result;
            if (string.IsNullOrWhiteSpace(_onChainPlayerStateObjectId))
            {
                yield return new WaitForSeconds(retryPeriod);
            }
        } 
        while (string.IsNullOrWhiteSpace(_onChainPlayerStateObjectId));

        // wait for countdown
        do
        {
            yield return new WaitForSeconds(retryPeriod);
        } while (_delayCounter > 0);
        
        _rb.velocity = Vector2.up * moveSpeed;
        _isInitialized = true;
        StartCoroutine(UpdateOnChainPlayerStateWorker());
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
        if (string.IsNullOrWhiteSpace(_onChainPlayerStateObjectId))
        {
            Debug.LogError("onChainStateObjectId is null UpdateOnChainPlayerStateAsync early return");
            return;
        }

        //Debug.Log($"lp position: {position}, velocity: {velocity}");
        var onChainPosition = new OnChainVector2(position);
        var onChainVelocity = new OnChainVector2(velocity);
       // Debug.Log($"UpdateOnChainPlayerStateAsync onChainPosition.x: {onChainPosition.x}, onChainPosition.y: {onChainPosition.y}, onChainVelocity.x: {onChainVelocity.x}, onChainVelocity.y {onChainVelocity.y}. isExploded: {_explosionController.IsExploded}");

       var moveCallTx = new MoveCallTransaction()
        {
            Signer = _signer,
            PackageObjectId = Constants.PACKAGE_OBJECT_ID,
            Module = Constants.MODULE_NAME,
            Function = "do_update",
            TypeArguments = ArgumentBuilder.BuildTypeArguments(),
            Arguments = ArgumentBuilder.BuildArguments( _onChainPlayerStateObjectId, onChainPosition.x, onChainPosition.y, onChainVelocity.x, onChainVelocity.y, _sequenceNumber++, _explosionController.IsExploded, TimestampService.UtcTimestamp ),
            Gas = await GetGasObjectIdAsync(_signer),
            GasBudget = 5000,
            RequestType = SuiExecuteTransactionRequestType.ImmediateReturn
        };
           
        await SuiApi.Signer.SignAndExecuteMoveCallAsync(moveCallTx);
        
        if (_explosionController.IsExploded && ! _scoreboardUpdated)
        {
            //await UpdateScoreboardAsync(_onChainPlayerStateObjectId);
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
            Gas = await GetGasObjectIdAsync(_signer),
            GasBudget = 5000,
            RequestType = SuiExecuteTransactionRequestType.WaitForEffectsCert
        };
           
        var txRpcResult = await SuiApi.Signer.SignAndExecuteMoveCallAsync(moveCallTx);
        
        var createdObjectId = ""; 
 
        if (txRpcResult != null && txRpcResult.IsSuccess) 
        { 
            createdObjectId = txRpcResult.Result.EffectsCert.Effects.Effects.Created.First().Reference.ObjectId;
            Debug.Log("CreatedOnChainPlayerStateAsync. createdObjectId: " + createdObjectId);
        } 
        else 
        { 
            Debug.LogError("Something went wrong when executing the transaction: " + txRpcResult?.ErrorMessage); 
        }

        return createdObjectId; 
    }
    
    private async Task UpdateScoreboardAsync(string onChainStateObjectId)
    {
        Debug.Log("UpdateScoreBoard");
        var moveCallTx = new MoveCallTransaction()
        {
            Signer = _signer,
            PackageObjectId = Constants.PACKAGE_OBJECT_ID,
            Module = Constants.MODULE_NAME,
            Function = "add_to_scoreboard",
            TypeArguments = ArgumentBuilder.BuildTypeArguments(),
            Arguments = ArgumentBuilder.BuildArguments( onChainStateObjectId, Constants.SCOREBOARD_OBJECT_ID ),
            Gas = await GetGasObjectIdAsync(_signer),
            GasBudget = 5000,
            RequestType = SuiExecuteTransactionRequestType.ImmediateReturn
        };
           
        var result = await SuiApi.Signer.SignAndExecuteMoveCallAsync(moveCallTx);
        Debug.Log("UpdateScoreBoard result: " + JsonConvert.SerializeObject(result)); 

        _scoreboardUpdated = true;
    }
    
    private async Task<string> GetGasObjectIdAsync(string address)
    {
        var gasObjectId = ""; 
        if (PlayerPrefs.HasKey(Constants.GAS_OBJECT_ID_KEY)) 
        { 
            gasObjectId = PlayerPrefs.GetString(Constants.GAS_OBJECT_ID_KEY); 
        }
        
        if (string.IsNullOrWhiteSpace(gasObjectId))
        {
            var gasObjects =
                (await SuiHelper.GetCoinObjectIdsAboveBalancesOwnedByAddressAsync(SuiApi.Client, address, 30, 10000));
            // sometimes locks get stuck for SUI objects, so try to get random object each time
            var randomRange = Random.Range(0, gasObjects.Count);
            gasObjectId = gasObjects[randomRange];
            PlayerPrefs.SetString(Constants.GAS_OBJECT_ID_KEY, gasObjectId); 
        }

        return gasObjectId;
    }
}
