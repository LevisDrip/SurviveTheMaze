using System.Collections;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiRework : MonoBehaviour, IDamageble
{
    public NavMeshAgent agent;

    private Animator EnemyMove;

    public Transform player;

    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    private float PatrolWait = 10;

    //#region DashValues


    //[Header("Dash Values")]
    //[SerializeField] private float DashDuration;
    //[SerializeField] private float DashSpeed;
    //[SerializeField] private float TimeToDash;
    //[SerializeField] private float WaitAfterDash;
    //Vector3 DashStartPos;
    //float CurrentDashTime;


    //#endregion

    [SerializeField] private float health;

    #region Patrol


    [Header("Patrol")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;


    #endregion

    #region Attacking


    [Header("Attacking")]
    //[SerializeField] private bool IsDashing;
    [SerializeField] private float damage;

    [SerializeField] private GameObject EnemyProjectile;
    [SerializeField] private GameObject FirePoint;

    [SerializeField] private float ProjectileForce;

    [SerializeField] private float ShootTimer;


    #endregion

    #region States


    [Header("States")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;


    #endregion

    #region Collsions


    [Header("Collisions")]
    [SerializeField] private BoxCollider PlayerCollision;


    #endregion

    private void Awake()
    {

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.enabled = true;

        PlayerCollision = GetComponent<BoxCollider>();

        EnemyMove = GetComponent<Animator>();
        PatrolWait = 5f;
    }

    private void Update()
    {
        ShootTimer -= Time.deltaTime;


        if (!player)
        {
            player = FindFirstObjectByType<PlayerController>().transform;
            return;
        }

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) EnemyShoots();

        //Debug.Log(IsDashing);

        if (EnemyMove && agent.enabled)
        {
            EnemyMove.SetBool("Moving", agent.velocity.magnitude != 0 ? true : false);
        }
    }

    #region States


    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && agent.enabled)
        {
            agent.SetDestination(walkPoint);

            // Check if the enemy has reached the walkPoint
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.SetDestination(transform.position);
                PatrolWait -= Time.deltaTime;
                if (PatrolWait <= 0)
                {
                    walkPointSet = false; // Reset to find a new walk point
                    PatrolWait = 5;
                }
            }
        }
    }


    #region PatrolLogic


    private void SearchWalkPoint()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-walkPointRange, walkPointRange),
            0,
            Random.Range(-walkPointRange, walkPointRange)
        );

        Vector3 potentialWalkPoint = transform.position + randomDirection;

        // Validate the walk point using NavMesh
        if (NavMesh.SamplePosition(potentialWalkPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }


    #endregion


    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }


    #endregion


    #region Damage Functions

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            GameManager.Instance.EnemiesKilled++;
            InventoryScript.Instance.EnemyKills++;
            Destroy(gameObject);
        }
    }

    void EnemyShoots()
    {
        agent.SetDestination(transform.position);

        if(ShootTimer <= 0)
        {

            Vector3 direction = (player.position + new Vector3(0, 1, 0) - FirePoint.transform.position).normalized;

            EnemyProjectileScript projectile = Instantiate(EnemyProjectile, FirePoint.transform.position, Quaternion.LookRotation(direction)).GetComponent<EnemyProjectileScript>();
            projectile.origin = transform;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direction * ProjectileForce, ForceMode.Impulse);

                ShootTimer = 2;
            }
        }
    }

    //void DashAttack()
    //{
    //    CurrentDashTime += Time.deltaTime;
    //    //prepare dash
    //    if(CurrentDashTime <= TimeToDash)
    //    {
    //        IsDashing = true;
    //        agent.enabled = false;
    //        PlayerCollision.isTrigger = true;
    //        DashStartPos = transform.position;

    //        //aim at player
    //        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    //    }
    //    //dash
    //    else if(CurrentDashTime <= DashDuration + TimeToDash)
    //    {
    //        transform.position += transform.forward * DashSpeed * Time.deltaTime;
    //        if(Physics.Linecast(DashStartPos, transform.position, out RaycastHit Hit))
    //        {
    //            IDamageble Target = Hit.transform.GetComponent<IDamageble>();
    //            if(Target != null && Hit.transform == player)
    //            {
    //                Target.TakeDamage(damage);
    //                DashStartPos = transform.position;
    //            }
    //            else if(Hit.transform != player && Hit.transform != transform)
    //            {
    //                transform.position = Hit.point;
    //                CurrentDashTime = 999;
    //            }

    //            print(Hit.transform.name);
    //        }
    //    }
    //    //fuck off
    //    else if(CurrentDashTime > DashDuration + TimeToDash)
    //    {
    //        IsDashing = false;
    //        agent.enabled = true;
    //        PlayerCollision.isTrigger = false;
    //        CurrentDashTime = 0;
    //    }
    //}

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}