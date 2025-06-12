using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryTabs : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject SelectedInventoryTab;
    [SerializeField] private GameObject OtherInventoryTab;

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectedInventoryTab.SetActive(true);
        OtherInventoryTab.SetActive(false);
    }
}
