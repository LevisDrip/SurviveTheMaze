using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BawxCode : MonoBehaviour, IDamageble, IInteractable
{
    [SerializeField] private float health = 100;

    [SerializeField]private List<GameObject> dropItems = new List<GameObject>();

    public void Interact()
    {
        if (dropItems.Count > 0)
        {
            Instantiate(dropItems[Random.Range(0, dropItems.Count)], transform.position, transform.rotation);
            Destroy(gameObject);
        }

        Destroy(gameObject);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        
        if (health <= 0)
        {
            if (dropItems.Count > 0)
            {
                Instantiate(dropItems[Random.Range(0, dropItems.Count)], transform.position, transform.rotation);
                Destroy(gameObject);
            }
       
            Destroy(gameObject);
        }
    }
}
