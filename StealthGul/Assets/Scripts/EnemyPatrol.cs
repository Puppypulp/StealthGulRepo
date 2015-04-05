using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour 
{
    public Transform waypointsGroup;
    public float waitAtNavPointTime = 2.0f;
    public float visionDistance = 10.0f;
    public float distanceToPLayerStop = 2.5f;
    public float searchForPlayerTimeout = 3.0f;
    public GameObject playerObject;
    public LayerMask enemyRaycastMask;

    private Vector3 m_lastKnowPlayerPos;
    private int m_positionsCounter = 0;
    private float m_agentMoveSpeed;
    private FSM enemyFSM;
    private NavMeshAgent navAgent;
    private Transform[] m_destinationPoints;
    private float m_enemyVisionDot = 0f;

    private void Awake()
    {
        enemyFSM = new FSM(this);
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        m_agentMoveSpeed = navAgent.speed;

        m_destinationPoints = new Transform[waypointsGroup.childCount];
        // Loop the number of times the length of the destination points array then assign each child to the destination waypoints array.
        for (int i = 0; i < m_destinationPoints.Length; i++)
        {
            m_destinationPoints[i] = waypointsGroup.GetChild(i);
        }
    }

    private void Start()
    {
        // The initial state
        enemyFSM.GoToState("MovingToPositionState", MovingToPositionUpdate);        
    }

    private void Update()
    {
        // Calling the on update function everyframe for the FSM
        enemyFSM.OnUpdate();       
    }

    #region Moving To Position State
    private IEnumerator MovingToPositionState()
    {
        navAgent.speed = m_agentMoveSpeed * 0.5f;
        while (true)
        {
            if (m_positionsCounter >= m_destinationPoints.Length)
            {
                m_positionsCounter = 0;
            }
            navAgent.speed = m_agentMoveSpeed * 0.5f;
            Transform point = m_destinationPoints[m_positionsCounter];
            navAgent.ResetPath();
            navAgent.SetDestination(point.position);
            // Wait for the new path to be available
            while (navAgent.pathPending)
            {
                yield return null;
            }

            // Wait for the movement to reach destination
            while (navAgent.remainingDistance > distanceToPLayerStop)
            {
                yield return null;
            }
            navAgent.speed = m_agentMoveSpeed * 0.0f;
            m_positionsCounter++;
            yield return new WaitForSeconds(waitAtNavPointTime);
        }
    }

    private void MovingToPositionUpdate()
    {
        if (PlayerInSight())
        {
            Debug.Log("Player in enemy vision");
            enemyFSM.GoToState("ChasePlayerState", ChasePlayerUpdate);
        }
    }
    #endregion

    #region Chase Player State
    private IEnumerator ChasePlayerState()
    {
        navAgent.speed = m_agentMoveSpeed;
        navAgent.ResetPath();
       
        while (DistanceToPlayer() > distanceToPLayerStop)
        {
            if (PlayerInSight())
            {
                navAgent.ResetPath();
                navAgent.SetDestination(playerObject.transform.position);
            }
            else
            {
                enemyFSM.GoToState("SearchForPlayerState", SearchForPlayerUpdate);
                break;
            }
            yield return null;
        }

        // Attack player here or go to a state where we attack the player 
        enemyFSM.GoToState("AttackPlayerState", AttackPlayerUpdate);
    }

    private void ChasePlayerUpdate()
    {
        FacePlayer();
        m_lastKnowPlayerPos = playerObject.transform.position;
    }
    #endregion

    #region Attack Player State
    private IEnumerator AttackPlayerState()
    {
        Debug.Log("Should attack player now");

        yield return new WaitForSeconds(1.0f);

        if (PlayerInSight())
        {
            enemyFSM.GoToState("ChasePlayerState", ChasePlayerUpdate);
        }
        else
        {
            enemyFSM.GoToState("SearchForPlayerState", SearchForPlayerUpdate);
        }
    }

    private void AttackPlayerUpdate()
    {
        FacePlayer();
        m_lastKnowPlayerPos = playerObject.transform.position;
    }
    #endregion

    #region Search For Player State
    private IEnumerator SearchForPlayerState()
    {
        navAgent.speed = m_agentMoveSpeed * 0.0f;

        // Lost sight of player and holding for a moment
        yield return new WaitForSeconds(0.5f);

        navAgent.speed = m_agentMoveSpeed;

        // Move to the last known location of the player
        navAgent.ResetPath();
        navAgent.SetDestination(m_lastKnowPlayerPos);

        // Wait for the new path to be available
        while (navAgent.pathPending)
        {
            yield return null;
        }
        // Wait for the movement to reach destination
        while (navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            yield return null;
        }

        // Arrived at last know location now wait for a beat
        yield return new WaitForSeconds(searchForPlayerTimeout);

        // Go back to patrol route if they have one else go to guard post       
        enemyFSM.GoToState("MovingToPositionState", MovingToPositionUpdate);
    }

    private void SearchForPlayerUpdate()
    {
        if (PlayerInSight())
            enemyFSM.GoToState("ChasePlayerState", ChasePlayerUpdate);
    }
    #endregion

    public bool PlayerInSight()
    {
        // Calculate vision cone
        Vector3 enemyFacingDir = transform.forward;
        Vector3 vecToPlayer = Vector3.Normalize(playerObject.transform.position - transform.position);
        m_enemyVisionDot = Vector3.Dot(enemyFacingDir, vecToPlayer);

        // Check if the player is in the vision cone and the ray hit the player object
        if (m_enemyVisionDot >= 0.4f && DistanceToPlayer() <= visionDistance)
        {
            // Cast a ray forward from the enemy
            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, vecToPlayer, out rayHit, visionDistance, enemyRaycastMask))
            {
                if (rayHit.collider.tag == "Player")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public float DistanceToPlayer()
    {
        return Vector3.Magnitude(playerObject.transform.position - transform.position);
    }

    public void DebugDrawEnemyVision()
    {
        Vector3 vecToPlayer = Vector3.Normalize(playerObject.transform.position - transform.position) * visionDistance;
        if (PlayerInSight())
        {
            Debug.DrawRay(transform.position, vecToPlayer, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, vecToPlayer, Color.green);
        }
    }

    // Face the player
    public void FacePlayer()
    {
        Vector3 lookDirection = Vector3.Normalize(playerObject.transform.position - transform.position);
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        Quaternion lerpLookRotation = Quaternion.Slerp(transform.rotation, lookRotation, (Time.deltaTime * 30.0f));
        transform.rotation = lerpLookRotation;
    }  

	
}
