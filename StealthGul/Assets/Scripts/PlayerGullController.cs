using UnityEngine;
using System.Collections;

public class PlayerGullController : MonoBehaviour 
{
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float rotationMoveSpeed = 15.0f;
    public float snapToCoverDistance = 10.0f;
    public bool inCover = false;
    public LayerMask coverRayMask;
    public PlayerGullAnimController gullAnimController;
    public GameObject playerAttackBox;

    private Vector3 m_moveDirection = Vector3.zero;
    private Vector3 m_facingDirection = Vector3.zero;
    private CharacterController m_player1CharacterController;
    private Transform m_camTrans;
    private Vector3 xCam = Vector3.zero;
    private Vector3 flatZCam = Vector3.zero;
    private Vector3 m_camRelativeVec = Vector3.zero;
    private bool m_canBreakFromCover = true;
    private Vector3 m_coverObjectVec = Vector3.zero;
    private Vector3 m_currentCoverEdgeNormal = Vector3.zero;
    private Vector3 m_currentCoverPoint = Vector3.zero;

	// Use this for initialization
	private void Awake() 
    {
        m_player1CharacterController = gameObject.GetComponent<CharacterController>(); 
        m_camTrans = Camera.main.transform;
        m_facingDirection = new Vector3(0.0f, 0.0f, 0.0f);

        playerAttackBox.SetActive(false);

        // Subscribe to the animation done event
        gullAnimController.AnimationDoneEvent += ResetAttackBox;
	}
	
	// Update is called once per frame
	private void Update() 
    {
        // CAMERA RELATIVE VECTORS
        xCam = Vector3.Normalize(m_camTrans.right);
        flatZCam = Vector3.Normalize(m_camTrans.forward - (Vector3.Dot(Vector3.up, m_camTrans.forward)) * Vector3.up);
        m_camRelativeVec = (xCam * Input.GetAxis("Horizontal")) + (flatZCam * Input.GetAxis("Vertical"));

        if (Input.GetButtonDown("Cover") && !inCover)
        {
            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, transform.forward, out rayHit, snapToCoverDistance, coverRayMask))
            {
                Camera.main.gameObject.GetComponent<CameraController>().inCoverCam = true;
                StartCoroutine("MoveToCover", rayHit);
            }
        }

        if (Input.GetButtonDown("Cover") && m_canBreakFromCover && inCover)
        {
            Camera.main.gameObject.GetComponent<CameraController>().inCoverCam = false;
            inCover = false;
        }

        // MOVEMENT CODE IF PLAYER IS GROUNDED
        if (m_player1CharacterController.isGrounded)
        {
            if (inCover)
            {
                if (!CoverEdgeValid())
                {
                    Vector3 vecToPlayerFromCoverPoint = Vector3.Normalize( transform.position - m_currentCoverPoint );
                    float dot = Vector3.Dot(vecToPlayerFromCoverPoint, m_coverObjectVec);

                    if (dot < -0.05f)
                    {
                        m_moveDirection = m_coverObjectVec * Mathf.Clamp(Vector3.Dot(m_camRelativeVec, m_coverObjectVec), 0f, 1f);
                    }
                    else
                    {
                        m_moveDirection = m_coverObjectVec * Mathf.Clamp(Vector3.Dot(m_camRelativeVec, m_coverObjectVec), -1f, 0f);
                    }
                }
                else
                {
                    m_moveDirection = m_coverObjectVec * Vector3.Dot(m_camRelativeVec, m_coverObjectVec);
                }                
            }
            else
            {
                m_moveDirection = m_camRelativeVec;
            }
           
            m_moveDirection *= moveSpeed;
            
            // Jump Check
            if(Input.GetButtonDown("Jump"))
            {
            	m_moveDirection.y = jumpSpeed;
            }

            if (Input.GetButtonDown("Attack"))
            {
                gullAnimController.PeckAnim();
                playerAttackBox.SetActive(true);
            }
                    
        }

        // SETTING THE MOVEMENT BASED ON VALUES ABOVE
        m_moveDirection.y -= gravity * Time.deltaTime;
        m_player1CharacterController.Move(m_moveDirection * Time.deltaTime);

        LookingAndFacing();
	}

    private IEnumerator MoveToCover(RaycastHit rayHit)
    {        
        m_coverObjectVec = GetCoverEdgeVec(rayHit);
        m_currentCoverEdgeNormal = rayHit.normal;
        m_currentCoverPoint = rayHit.point;

        Vector3 playerVecToWall = Vector3.Normalize( transform.position - rayHit.point );
        Vector3 wallPos = rayHit.point + new Vector3( playerVecToWall.x, 0f, playerVecToWall.z );

        float startTime = Time.time;
        float lerpTime = 0.5f;

        while (Time.time - startTime < lerpTime)
        {
            float delta = Utilities.LerpScale( (Time.time - startTime), 0f, lerpTime, 0f, 1f);
            transform.position = Vector3.Lerp(transform.position, wallPos, delta);
            yield return null;
        }
        transform.position = wallPos;

        StartCoroutine("BreakOutCoverTimer");

        inCover = true;        
    }

    private Vector3 GetCoverEdgeVec(RaycastHit rayHit)
    {
        Vector3 coverVec = Vector3.zero;       
        coverVec = Vector3.Normalize( Vector3.Cross( Utilities.FlattenVector(rayHit.normal), Vector3.up ) );

        return coverVec;
    }   

    private bool CoverEdgeValid()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, m_currentCoverEdgeNormal * -1f, out rayHit, 10.0f, coverRayMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }    

    private IEnumerator BreakOutCoverTimer()
    {
        m_canBreakFromCover = false;

        yield return new WaitForSeconds(0.25f);

        m_canBreakFromCover = true;
    }

    private void LookingAndFacing()
    {          
        if (UsingLeftStick())
        {
            // If the player is not using the right stick we set the facing direction by the left stick vector
            m_facingDirection = new Vector3(m_moveDirection.x, 0.0f, m_moveDirection.z);
            Quaternion lookRotation = Quaternion.LookRotation(m_facingDirection, Vector3.up);
            // Rotation is slerped to the rotation value for smoother interaction
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationMoveSpeed);
        }
        
    }    

    private bool UsingLeftStick()
    {
        if (Input.GetAxis("Horizontal") > 0.05f || Input.GetAxis("Horizontal") < -0.05f || Input.GetAxis("Vertical") > 0.05f || Input.GetAxis("Vertical") < -0.05f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ResetAttackBox()
    {
        playerAttackBox.SetActive(false);
    }

    private void OnDestory()
    {
        gullAnimController.AnimationDoneEvent -= ResetAttackBox;
    }
}
