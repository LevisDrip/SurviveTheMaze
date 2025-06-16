using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTabs : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject SelectedInventoryTab;
    [SerializeField] private GameObject OtherInventoryTab;

    [SerializeField] private GameObject OtherTab;
    [SerializeField] private GameObject CurrentTab;

    [SerializeField] private Texture SelectedTabSprite;
    [SerializeField] private Texture UnselectedTabSprite;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SelectedInventoryTab != null)
        {
            SelectedInventoryTab.SetActive(true);
            CurrentTab.GetComponent<RawImage>().texture = SelectedTabSprite;

            OtherInventoryTab.SetActive(false);
            OtherTab.GetComponent<RawImage>().texture = UnselectedTabSprite;
        }
        else
        {
            return;
        }
    }
}
