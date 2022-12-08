using Suinet.Rpc;
using UnityEngine;

public class EnsureWalletExists : MonoBehaviour
{
    // Start is called before the first frame update
    async void Awake()
    {
        if (SuiWallet.GetActiveKeyPair() == null)
        {
            SuiWallet.CreateNewWallet();
            var newAddress = SuiWallet.GetActiveAddress();
            Debug.Log("new wallet address created: " + newAddress);
            await SuiAirdrop.RequestAirdrop(newAddress);
        }
        else
        {
            var address = SuiWallet.GetActiveAddress();
            Debug.Log("Using existing wallet with address: " + address);
            if (await SuiHelper.GetBalanceAsync(SuiApi.Client, address) <= 0L)
            {
                Debug.Log("Requesting airdrop to address: " + address);
                await SuiAirdrop.RequestAirdrop(address);
            }
        }
    }


}
