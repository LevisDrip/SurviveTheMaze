using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillScript : MonoBehaviour, IItemAble
{
    public float HealAmount;

    public void UseItem()
    {
        FindFirstObjectByType<PlayerController>().TakeDamage(-HealAmount);

        FindFirstObjectByType<PlayerController>().ItemDiscard();

        Destroy(transform.parent.gameObject);
    }
}
