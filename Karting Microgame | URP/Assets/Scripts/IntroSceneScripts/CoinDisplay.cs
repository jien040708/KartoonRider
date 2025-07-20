using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    public TMP_Text coinText;

    private int coins = 100; // user 정보에서 받아온 코인 수

    public void Start()
    {
        SetCoins(coins);
    }

    public void SetCoins(int coins)
    {
        coinText.text = "Coins: " + coins.ToString();
    }
}
