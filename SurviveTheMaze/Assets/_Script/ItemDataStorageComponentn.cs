using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ItemDataStorageComponentn : MonoBehaviour, IPointerClickHandler
{
    public ItemData StoredItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        InventoryScript.Instance.SelectedInventoryItem = this;
        InventoryScript.Instance.PreviouslySelectedItem = this;
        InventoryScript.Instance.MoveSelectedVisual(this.transform.position);
    }
}
