using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AirdropButton : MonoBehaviour
{
    private void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(async () =>
        {
            await SuiAirdrop.RequestAirdrop(SuiWallet.GetActiveAddress());
        });
    }
}
