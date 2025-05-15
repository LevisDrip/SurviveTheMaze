using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemData") ]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemVisual;
    public GameObject EquippedItem;
}
