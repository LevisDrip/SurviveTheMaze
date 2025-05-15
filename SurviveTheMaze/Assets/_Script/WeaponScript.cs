using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponScript : MonoBehaviour
{
    public Transform camHolder;

    public float Damage;
    public float DamageRange;

    public float CritDamage;
    public float CritChance;

    private Animator GunClip;
    private AudioSource SFX;

    [SerializeField] private Transform LaserPoint;
    [SerializeField]private LineRenderer Laser;

    [SerializeField] private LayerMask PlayerLayer;

    private void Awake()
    {
        Debug.Log(GameManager.Instance == null);    
        camHolder = GameObject.Find("GameManager").GetComponent<GameManager>().CamHolder;

        GunClip = GetComponent<Animator>();
        SFX = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Vector3 LineDir = camHolder.transform.position + camHolder.forward * 100;   
        Laser.SetPosition(1, LineDir);

        Laser.SetPosition(0, LaserPoint.position);

        if (Physics.Raycast(camHolder.position, camHolder.forward, out RaycastHit hit, ~PlayerLayer))
        {
            Laser.SetPosition(1, hit.point);
        }
    }

    private void Fire()
    {
        if (Physics.Raycast(camHolder.position, camHolder.forward, out RaycastHit hit))
        {
            Debug.DrawLine(camHolder.position, hit.point, Color.red);

            print(hit.transform.name);

            float DamageThingie = Damage + Random.Range(-DamageRange, DamageRange);
            float R = Random.Range(0, 100);
            if (R <= CritChance)
            {
                DamageThingie += CritDamage;
            }

            Rigidbody Flingable = hit.transform.GetComponent<Rigidbody>();
            if (Flingable != null)
            {
                Flingable.AddForceAtPosition(camHolder.forward * DamageThingie * 3f, hit.point, ForceMode.Impulse);
            }

            IDamageble Damageable = hit.transform.GetComponent<IDamageble>();
            if (Damageable != null)
            {
                Damageable.TakeDamage(DamageThingie);
                Debug.Log(DamageThingie);

            }
        }


        


    }

    //public void OnFire(InputAction.CallbackContext ctx)
    //{
    //    if(ctx.performed)
    //    {

    //    }

    //}
    public void OnFire()
    {
        if (InventoryScript.Instance.IsOpen)
        {
            return;
        }

        Debug.Log("fireS");
        GunClip.Play("Shooting");
        SFX.Play();
        Fire();
    }

    
}
