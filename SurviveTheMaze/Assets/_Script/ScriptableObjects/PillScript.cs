using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillScript : MonoBehaviour, IItemAble
{
    public float HealAmount;

    public void UseItem()
    {
        FindFirstObjectByType<PlayerController>().TakeDamage(-HealAmount);
        
        InventoryScript.Instance.inventoryItems.Remove(InventoryScript.Instance.PreviouslySelectedItem.StoredItem);

        Destroy(InventoryScript.Instance.PreviouslySelectedItem.gameObject);

        Destroy(transform.parent.gameObject);
    }
}
