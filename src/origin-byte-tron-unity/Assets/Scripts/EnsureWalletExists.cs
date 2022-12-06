using System.Collections;
using System.Collections.Generic;
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
    }


}
