using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{ 
    [SerializeField] LayerMask IgnoreLayer;
    public Transform origin;
    public int Damage;

    Vector3 OldPos;
    float AliveTime;

    // Update is called once per frame
    void Update()
    {
        //collision
        if (Physics.Linecast(transform.position, OldPos, out RaycastHit Hit, ~IgnoreLayer) && Hit.transform != origin)
        {
            IDamageble Target = Hit.transform.GetComponent<IDamageble>();
            if (Target != null)
            {
                Target.TakeDamage(Damage);
            }
        }

        OldPos = transform.position;

        //death timer
        AliveTime += Time.deltaTime;
        if (AliveTime >= 1)
        {
            Destroy(gameObject);
        }
    }
}
