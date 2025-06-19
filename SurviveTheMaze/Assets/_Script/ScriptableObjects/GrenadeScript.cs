using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour, IItemAble
{
    public GameObject GrenadePrefab;

    public void UseItem()
    {
        InventoryScript.Instance.inventoryItems.Remove(InventoryScript.Instance.PreviouslySelectedItem.StoredItem);

        Instantiate(GrenadePrefab,transform.position,transform.rotation);

        Destroy(InventoryScript.Instance.PreviouslySelectedItem.gameObject);

        Destroy(gameObject);
    }
}
