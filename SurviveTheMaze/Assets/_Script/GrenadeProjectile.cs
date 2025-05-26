using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    float FuseTimer = 5;
    public float explosionRadius;

    public float damage;

    public float LaunchForce;

    public GameObject ExplosionFX;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * LaunchForce);
    }

    // Update is called once per frame
    void Update()
    {
        FuseTimer -= Time.deltaTime;

        if (FuseTimer <= 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider c in colliders)
            {
                IDamageble damageble = c.GetComponent<IDamageble>();
                if(damageble != null )
                {
                    damageble.TakeDamage(damage);
                }
            }

            Instantiate(ExplosionFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
