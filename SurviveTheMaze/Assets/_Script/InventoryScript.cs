using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public static InventoryScript Instance { get; private set; }
    

    #region Inventory UI

        [Header("Inventory Visual UI")]
        public GameObject InventoryMenu;
        public GameObject InventoryVisual;

        [Header("Inventory Numerical UI")]
        public int EnemyKills;

        //Delete or implement later.
        public int Coins;

        [Header("Inventory Other UI")]
        public GameObject InventoryParent;

    #endregion


    #region Other Variables

        [Header("Item Data")]
        public ItemDataStorageComponentn SelectedInventoryItem;
        public ItemDataStorageComponentn PreviouslySelectedItem;
        public ItemData EquippedItem;

        [Header("Other")]

        public Image SelectedItemVisual;
        public bool IsOpen = false;


        [Header("Inventory Items")]
        public List<ItemData> inventoryItems = new List<ItemData>();

    #endregion

    

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        InventoryMenu.SetActive(false);
    }

    private void Start()
    {
        foreach (ItemData item in inventoryItems)
        {
            AddItemIcon(item);
        }

        SelectedItemVisual.gameObject.SetActive(false);
    }



    public void ToggleInventory()
    {
        if (IsOpen)
        {
            SelectedInventoryItem = null;

            InventoryMenu.SetActive(false);
            IsOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
        else
        {
            InventoryMenu.SetActive(true);
            IsOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            SelectedItemVisual.gameObject.SetActive(false);
        }
    }

    public void AddItemIcon(ItemData item)
    {
        GameObject spawned = Instantiate(InventoryVisual, InventoryParent.transform);
        spawned.GetComponent<Image>().sprite = item.itemVisual;
        spawned.GetComponent<ItemDataStorageComponentn>().StoredItem = item;
    }

    public void DiscardItem()
    {
        if(SelectedInventoryItem != null)
        {
            inventoryItems.Remove(SelectedInventoryItem.StoredItem);
            Destroy(SelectedInventoryItem.gameObject);
            FindFirstObjectByType<PlayerController>().ItemDiscard();

            SelectedItemVisual.gameObject.SetActive(false);

        }
    }

    public void EquipItem()
    {
        if(SelectedInventoryItem != null)
        {
            EquippedItem = SelectedInventoryItem.StoredItem;
            FindFirstObjectByType<PlayerController>().ItemEquip();
        }
    }

    public void MoveSelectedVisual(Vector3 ItemIconPos)
    {
        if (SelectedInventoryItem)
        {
            SelectedItemVisual.gameObject.SetActive(true);
            SelectedItemVisual.transform.position = ItemIconPos;
        }
    }
}
