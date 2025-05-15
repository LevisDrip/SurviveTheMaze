using UnityEngine;

public class ButtonScript : MonoBehaviour, IInteractable
{
    public Transform SpawnPoint;
    public GameObject Bawx;

    public void Interact()
    {
        Instantiate(Bawx, SpawnPoint.position, Bawx.transform.rotation);
    }
}
