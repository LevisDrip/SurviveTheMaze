using UnityEngine;

public class CoinCode : MonoBehaviour, IPickUpAble
{
    public void GetCoin()
    {
        InventoryScript.Instance.Coins++;
        Destroy(gameObject);
    } 
}
