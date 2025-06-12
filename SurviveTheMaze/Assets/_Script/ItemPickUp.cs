using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemPickUp : MonoBehaviour, IInteractable
{
    [SerializeField]private ItemData itemData;

    public void Interact()
    {
        if(InventoryScript.Instance.inventoryItems.Count < InventoryScript.Instance.InventorySlots)
        {
            Debug.Log(itemData.itemName);
            InventoryScript.Instance.inventoryItems.Add(itemData);
            InventoryScript.Instance.AddItemIcon(itemData);
            Destroy(transform.parent.gameObject);
        }
    }
}
