using UnityEngine;
using TMPro;



public class CoinDisplay : MonoBehaviour
{
    public TMP_Text coinText;

    private int coins = 100; //user 정보에서 coins 가져오는 로직 추가

    public void SetCoins(coins)
    {
        coinText.text = coins.ToString();
    }
}
