using UnityEngine;
using System.Collections;

public class PlayerGullController : MonoBehaviour 
{
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float rotationMoveSpeed = 15.0f;

    private Vector3 m_moveDirection = Vector3.zero;
    private Vector3 m_facingDirection = Vector3.zero;
    private CharacterController m_player1CharacterController;
    private Transform m_camTrans;
    private Vector3 xCam = Vector3.zero;
    private Vector3 flatZCam = Vector3.zero;

	// Use this for initialization
	private void Awake() 
    {
        m_player1CharacterController = gameObject.GetComponent<CharacterController>();       
        m_camTrans = Camera.main.transform;
        m_facingDirection = new Vector3(0.0f, 0.0f, 0.0f);       
	}
	
	// Update is called once per frame
	private void Update() 
    {
        // CAMERA RELATIVE VECTORS
        xCam = Vector3.Normalize(m_camTrans.right);
        flatZCam = Vector3.Normalize(m_camTrans.forward - (Vector3.Dot(Vector3.up, m_camTrans.forward)) * Vector3.up);

        // MOVEMENT CODE IF PLAYER IS GROUNDED
        if (m_player1CharacterController.isGrounded)
        {
            m_moveDirection = (xCam * Input.GetAxis("Horizontal")) + (flatZCam * Input.GetAxis("Vertical"));
           
            m_moveDirection *= moveSpeed;
            
            //REMOVED JUMP CODE FOR NOW	
            if(Input.GetButton("Jump"))
            {
            	m_moveDirection.y = jumpSpeed;
            }
                    
        }

        // SETTING THE MOVEMENT BASED ON VALUES ABOVE
        m_moveDirection.y -= gravity * Time.deltaTime;
        m_player1CharacterController.Move(m_moveDirection * Time.deltaTime);

        LookingAndFacing();
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
}
