using System.Collections;
using Suinet.Rpc;
using UnityEngine;
using UnityEngine.UI;

public class WalletProfile : MonoBehaviour
{
    public Text addressText;
    public Text suiBalanceText;
    public float updateInterval;
    
    void Start()
    {
        StartCoroutine(UpdateText(updateInterval));
    }

    IEnumerator UpdateText(float interval)
    {
        while (true)
        {
            var activeAddress = SuiWallet.GetActiveAddress();
            if (activeAddress != "0x")
            {
                addressText.text = $"{activeAddress.Substring(0, 6)}...{activeAddress.Substring(activeAddress.Length - 4)}";
                var getBalanceTask = SuiHelper.GetBalanceAsync(SuiApi.Client, activeAddress);
                yield return new WaitUntil(() => getBalanceTask.IsCompleted);
                suiBalanceText.text = $"{getBalanceTask.Result} SUI";
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
