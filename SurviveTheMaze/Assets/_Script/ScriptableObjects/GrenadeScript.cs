using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour, IItemAble
{
    public GameObject OYBLYATGRENATA;

    public void UseItem()
    {
        InventoryScript.Instance.inventoryItems.Remove(InventoryScript.Instance.PreviouslySelectedItem.StoredItem);

        Instantiate(OYBLYATGRENATA,transform.position,transform.rotation);

        Destroy(InventoryScript.Instance.PreviouslySelectedItem.gameObject);

        Destroy(gameObject);
    }
}
