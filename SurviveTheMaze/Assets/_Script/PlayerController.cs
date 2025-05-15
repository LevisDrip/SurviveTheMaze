using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageble
{
    #region MovementVariables

    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float speedAddition = 5;
    [SerializeField] float gravity = -30f;
    [SerializeField] float jumpHeight = 3.5f;
    [SerializeField] float groundCheckSize = 0.1f;
    [Tooltip("It is recommended to create a player layer and excluding it from the ground layer.")]
    [SerializeField] LayerMask groundMask;

    #endregion

    #region CameraRotVariables

    [Header("Camera rotation")]
    [SerializeField] Vector2 sensitivity = new Vector2(750f, 750f);
    [SerializeField] Transform camHolder;
    [Range(0f, 90f)]
    [SerializeField] float xClamp = 85f;

    #endregion

    #region WeaponBobVariables

    [Header("Weapon Bobbing")]
    [SerializeField] float bobFrequency = 5f;
    [SerializeField] float currentBobFrequency = 3f;
    [SerializeField] float bobAmplitude = 0.1f;
    [SerializeField] float swayAmplitude = 0.075f;
    private float bobTimer = 0f;
    private Vector3 initialHandPosition;

    #endregion

    #region Health

    [Header("Health")]
    public float health;
    public float maxHealth;
    public Slider HealthBar;

    #endregion

    #region UI stuff

    [Header("UI stuff")]
    public GameObject DeathScreen;

    public GameObject Inventory;

    public GameObject InteractText;

    public GameObject LoadingScreen;

    #endregion

    #region Other Variables

    [Header("Other")]
    [SerializeField] private Transform HAND;

    [SerializeField] private float rayDistance;

    public Transform CamHolder;

    bool DoInteract;
  
    GameObject CurrentItem;

    private CharacterController characterController;
    bool isGrounded;
    float xRot = 0f;

    #endregion

    Vector3 verticalVelocity = Vector3.zero;
    Vector2 moveInput = Vector2.zero;
    Vector2 lookInput = Vector2.zero;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        InteractText.SetActive(false);
        initialHandPosition = HAND.localPosition;

        DeathScreen.SetActive(false);
        LoadingScreen.SetActive(false);

        DontDestroyOnLoad(gameObject);

        health = maxHealth;

        if (HealthBar)
        {
            HealthBar.maxValue = maxHealth;
        }
    }

    private void Update()
    {
        Move();

        ApplyGravity();

        UpdateCameraRot();

        ButtonCheck();

        WeaponBob();

        if (HealthBar)
        {
            HealthBar.value = health;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckSize, groundMask);

        //Reset downwards velocity when grounded
        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = 0;
        }
    }


    #region CodeLogic
    //-------------------------------------------------------------

    private void Move()
    {
        Vector3 horizontalVelocity = (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;
        characterController.Move(horizontalVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private void OnJumpPressed()
    {
        verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
    }

    private void UpdateCameraRot()
    {
        float mouseX = lookInput.x * sensitivity.x * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity.y * Time.deltaTime;

        //Rotates body horizontally
        transform.Rotate(Vector3.up, mouseX);

        //Rotates camera vertically
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -xClamp, xClamp);
        camHolder.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    private void ButtonCheck()
    {
        if (Physics.Raycast(camHolder.position, camHolder.forward, out RaycastHit hit, rayDistance))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                InteractText.SetActive(true);

                if (DoInteract)
                {
                    interactable.Interact();
                    DoInteract = false;
                }
                
            }
            else
            {
                InteractText.SetActive(false);
                DoInteract = false;
            }
        }
        else
        {
            InteractText.SetActive(false);
            DoInteract = false;
        }
    }

    public void ItemEquip()
    {
        if (CurrentItem)
        {
            Destroy(CurrentItem);
            CurrentItem = null;
        }
        ItemData toEquip = InventoryScript.Instance.EquippedItem;
        GameObject NewItem = Instantiate(toEquip.EquippedItem, HAND.position, HAND.rotation, HAND);
        CurrentItem = NewItem;
    }

    public void ItemDiscard()
    {
        Destroy(CurrentItem);
        CurrentItem = null;
    }

    private void WeaponBob()
    {
        if (!CurrentItem) return;

        if (moveInput.magnitude > 0 && isGrounded)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(bobTimer * 2) * bobAmplitude;
            float swayOffset = Mathf.Sin(bobTimer) * swayAmplitude;
            HAND.localPosition = initialHandPosition + new Vector3(swayOffset, bobOffset, 0);
        }
        else
        {
            bobTimer = 0;
            HAND.localPosition = Vector3.Lerp(HAND.localPosition, initialHandPosition, Time.deltaTime * 5f);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (HealthBar)
        {
            HealthBar.value = health;
        }

        if (health <= 0)
        {
            TriggerPlayerDeath();
        }

        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void TriggerPlayerDeath()
    {
        
        Cursor.lockState = CursorLockMode.None;
        DeathScreen.SetActive(true);
        HAND.gameObject.SetActive(false);
        GetComponent<PlayerController>().enabled = false;

        
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickUpAble pickUp = other.GetComponent<IPickUpAble>();
        if (pickUp != null)
        {
            pickUp.GetCoin();
        }

        if (other.gameObject.CompareTag("TutorialText"))
        {
            other.GetComponent<MeshRenderer>().enabled = true;
        }

        if (other.CompareTag("Exit"))
        {
            LoadingScreen.SetActive(true);
            FindFirstObjectByType<NoiseTexture>().RegenerateMap();

        }

        if (other.CompareTag("TutorialExit"))
        {
            LoadingScreen.SetActive(true);
            SceneManager.LoadScene(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TutorialText"))
        {
            other.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    //-------------------------------------------------------------
    #endregion


    #region InputActions
    //-------------------------------------------------------------

    public void OnMove(InputAction.CallbackContext ctx)
    {
        //Debug.Log(ctx.ReadValue<Vector2>());
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        //Debug.Log(ctx.ReadValue<Vector2>());
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            

            if (isGrounded)
            {
                Debug.Log("I get up!");
                OnJumpPressed();
            }
        }
    }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            moveSpeed += speedAddition;
            bobFrequency += currentBobFrequency;
        }
        if (ctx.canceled)
        {
            moveSpeed -= speedAddition;
            bobFrequency -= currentBobFrequency;
        }
        
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DoInteract = true;
            Debug.Log("touch");
        }
        
    }

    public void OnAccesInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Inventory Accesed");
            InventoryScript.Instance.ToggleInventory();
        }
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!CurrentItem)
            {
                return;
            }

            WeaponScript WS = CurrentItem.GetComponent<WeaponScript>();
            IItemAble item = CurrentItem.GetComponent<IItemAble>();
            if (WS != null)
            {
                WS.OnFire();
            }

            if(item != null)
            {
                item.UseItem();
            }
        }

    }

    //-------------------------------------------------------------
    #endregion



    public void ReloadItem()
    {
        if (CurrentItem == null)
        {
            return ;
        }
        
        CurrentItem.SetActive(false);
        CurrentItem.SetActive(true);
    }

    public void DisableLoadScreen()
    {
        LoadingScreen.SetActive(false);
    }

    public void AfterDeathContinueBTN()
    {
        Cursor.lockState = CursorLockMode.Locked;
        DeathScreen.SetActive(false);
        HAND.gameObject.SetActive(true);
        GetComponent<PlayerController>().enabled = true;

        Destroy(gameObject);

        FindFirstObjectByType<NoiseTexture>().RegenerateMap();
    }
}


