// using UnityEngine;
// using System.Collections.Generic;
// using UnityEngine.UI;

// public enum EnemyState
// {
//     None,
//     Idle,
//     StandFire,
//     Moving,
//     MoveStopped,
//     Sitting,
//     StandUp,
//     Dead
// }

// public class EnemyAI : MonoBehaviour
// {
//     [Header("AI Settings")] public EnemyState currentState = EnemyState.None;
//     public float moveSpeed = 3.5f;
//     public float rotationSpeed = 120f;
//     public float stoppingDistance = 0.5f;
//     [SerializeField] float maxHP = 100f;
//     [SerializeField] private float currentHP = 0;

//     [SerializeField] private List<GameObject> objectsHideOnDead;
//     [SerializeField] private Slider sdHP;

//     [Header("Patrol Settings")] public List<PatrolRoute> patrolRoutes = new List<PatrolRoute>();
//     private int currentRouteIndex = 0;

//     private Animator animator;
//     private Vector3 currentVelocity;
//     private float currentSpeed;
//     private Quaternion targetRotation;
//     private float stateTimer;

//     private PatrolRoute currentRoute = null;
//     private Transform waypoint;
//     private Transform playerTransform;

//     private EnemiesManager _enemiesManager;
//     [HideInInspector] public bool isDead = false;
//     public float GetCurrentHP => currentHP;
//     [SerializeField] private MachineGun gun;

//     [System.Serializable]
//     public class PatrolRoute
//     {
//         public Transform waypoint;
//         public EnemyState state;
//         public float waitTime;
//     }

//     void Start()
//     {
//         animator = GetComponentInChildren<Animator>();
//         InitializeAI();
//     }

//     public void Init(EnemiesManager parent, Transform player, Tracker tracker)
//     {
//         _enemiesManager = parent;
//         this.playerTransform = player;
//         currentHP = maxHP;
//         sdHP.value = 1;
//         ShowHideObjects(true);

//         isDead = false;

//         Tracker newTracker = Instantiate(tracker, tracker.gameObject.transform.parent);
//         newTracker.gameObject.SetActive(true);
//         newTracker.SetTarget(this);
//     }

//     void Update()
//     {
//         if (currentState == EnemyState.None || currentState == EnemyState.Dead) return;

//         switch (currentState)
//         {
//             case EnemyState.Idle:
//                 UpdateIdle();
//                 break;

//             case EnemyState.Moving:
//                 UpdateMoving();
//                 ApplyMovement();
//                 break;

//             case EnemyState.MoveStopped:
//                 UpdateMoveStopped();
//                 break;

//             case EnemyState.Sitting:
//                 UpdateSitting();
//                 break;

//             case EnemyState.StandUp:
//                 UpdateStandUp();
//                 break;
//             case EnemyState.StandFire:
//                 UpdateStandFire();
//                 break;
//         }

//         UpdateLookTarget();
//         UpdateAnimations();
//     }

//     void InitializeAI()
//     {
//         if (patrolRoutes.Count > 0)
//         {
//             currentRouteIndex = 0;

//             currentRoute = patrolRoutes[currentRouteIndex];

//             if (currentRoute.waypoint == null) waypoint = playerTransform;
//             else waypoint = currentRoute.waypoint;

//             ChangeState(currentRoute.state);
//         }
//     }

//     void ChangeState(EnemyState newState, bool force = false)
//     {
//         if (currentState == newState && !force) return;

//         //Debug.Log("Change State: " + newState);
//         // Exit current state
//         switch (currentState)
//         {
//             case EnemyState.Sitting:
//                 // Clean up sitting if needed
//                 break;
//         }

//         currentState = newState;
//         stateTimer = currentRoute.waitTime;

//         // Enter new state
//         switch (newState)
//         {
//             case EnemyState.Idle:
//                 //stateTimer = currentRoute.waitTime;
//                 break;

//             case EnemyState.StandUp:
//                 // Start stand up animation
//                 break;
//         }
//     }

//     void UpdateIdle()
//     {
//         stateTimer -= Time.deltaTime;
//         if (stateTimer <= 0)
//         {
//             //move to next action
//             AdvanceToNextWaypoint();
//         }
//     }

//     void UpdateLookTarget()
//     {
//         Vector3 direction = (waypoint.position - transform.position).normalized;
//         direction.y = 0; // Ignore vertical difference

//         // Calculate rotation
//         targetRotation = Quaternion.LookRotation(direction);
//         transform.rotation = Quaternion.RotateTowards(
//             transform.rotation,
//             targetRotation,
//             rotationSpeed * Time.deltaTime
//         );
//     }

//     void UpdateMoving()
//     {
//         if (patrolRoutes.Count == 0) return;

//         // Calculate movement
//         float distance = Vector3.Distance(
//             new Vector3(transform.position.x, 0, transform.position.z),
//             new Vector3(waypoint.position.x, 0, waypoint.position.z)
//         );


//         if (distance <= stoppingDistance)
//         {
//             UpdateMoveStopped();
//             AdvanceToNextWaypoint();
//         }
//         else
//         {
//             currentSpeed = moveSpeed;
//             currentVelocity = transform.forward * currentSpeed;
//         }
//     }

//     void UpdateMoveStopped()
//     {
//         currentSpeed = 0;
//         currentVelocity = Vector3.zero;
//     }

//     void UpdateSitting()
//     {
//         stateTimer -= Time.deltaTime;
//         if (stateTimer <= 0)
//         {
//             ChangeState(EnemyState.StandUp);
//         }
//     }

//     void UpdateStandUp()
//     {
//         stateTimer -= Time.deltaTime;
//         if (stateTimer <= 0)
//         {
//             AdvanceToNextWaypoint();
//         }
//     }

//     void UpdateStandFire()
//     {
//         stateTimer -= Time.deltaTime;
//         if (stateTimer <= 0)
//         {
//             AdvanceToNextWaypoint();
//         }
//     }

//     void AdvanceToNextWaypoint()
//     {
//         currentRouteIndex++;
//         if (patrolRoutes.Count > currentRouteIndex)
//         {
//             currentRoute = patrolRoutes[currentRouteIndex];
//             if (currentRoute.waypoint == null) waypoint = playerTransform;
//             else waypoint = currentRoute.waypoint;

//             ChangeState(currentRoute.state, true);
//         }
//         else
//         {
//             //do nothing
//         }
//     }

//     void ApplyMovement()
//     {
//         transform.position += currentVelocity * Time.deltaTime;
//     }

//     void UpdateAnimations()
//     {
//         float animSpeed = Mathf.Clamp01(currentSpeed / moveSpeed);
//         animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
//         animator.SetBool("IsSitting", currentState == EnemyState.Sitting);
//         animator.SetBool("IsStandingUp", currentState == EnemyState.StandUp);
//         animator.SetBool("Idle", currentState == EnemyState.Idle);
//         animator.SetBool("IsStandFire", currentState == EnemyState.StandFire);
//     }

//     public void TakeDamage(float damage)
//     {
//         if (isDead) return;

//         currentHP -= damage;
//         sdHP.value = currentHP / maxHP;
//         if (currentHP <= 0)
//         {
//             ShowHideObjects(false);
//             Die();
//         }
//     }

//     public void ShowHideObjects(bool show)
//     {
//         foreach (var obj in objectsHideOnDead)
//         {
//             if (obj != null) obj.SetActive(show);
//         }
//     }

//     public void Die()
//     {
//         isDead = true;
//         Destroy(GetComponent<BoxCollider>());
//         ChangeState(EnemyState.Dead);
//         currentVelocity = Vector3.zero;
//         animator.SetTrigger("Die");
//         // Additional death logic
//         _enemiesManager.EnemyDead(this);
//         if (gun != null) gun.Dead();

//         EffectManager.Instance.PlayEffect<GameObject>(EEffectType.EnemyDestroy,
//             new Vector3(transform.position.x, transform.position.y - 5, transform.position.z), transform.rotation);
//     }
// }