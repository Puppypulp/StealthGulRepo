using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    public GameObject followObject;
    public float catchupSpeed = 5.0f;
    public float xOffset = 0.0f;
    public float zOffset = 0.0f;
    public float yOffset = 0.0f;

    public float coverXOffset = 0.0f;
    public float coverYOffset = 0.0f;
    public float coverZOfsset = 0.0f;
    public bool inCoverCam = false;

    public float rightStickCamInfluence = 1.0f;

    private Vector3 smoothedCamPos = Vector3.zero;
    private PlayerGullController m_playerController;

    private void Awake()
    {
        m_playerController = followObject.GetComponent<PlayerGullController>();
        smoothedCamPos = new Vector3((followObject.transform.position.x + xOffset), (followObject.transform.position.y + yOffset), (followObject.transform.position.z + zOffset));
    }

    private void Update()
    {
        Vector3 adjustedCamPos = Vector3.zero;

        if (inCoverCam)
        {
            adjustedCamPos = new Vector3((followObject.transform.position.x + coverXOffset), (followObject.transform.position.y + coverYOffset), (followObject.transform.position.z + coverZOfsset));
        }
        else
        {
            adjustedCamPos = new Vector3((followObject.transform.position.x + xOffset), (followObject.transform.position.y + yOffset), (followObject.transform.position.z + zOffset));
        }

        // The smoothed value of the cam position lerp from the camera current pos to the adjusted cam pos. This is constantly updated every frame
        Vector3 rightStickCamOffset = RightStickCamVector() * rightStickCamInfluence;
        smoothedCamPos = Vector3.Lerp(transform.position, adjustedCamPos + rightStickCamOffset, (Time.deltaTime * catchupSpeed));

        // Set the came pos to the adjusted position every frame
        transform.position = smoothedCamPos;
    }

    private Vector3 RightStickCamVector()
    {
        // CAMERA RELATIVE VECTORS
        Vector3 xCam = Vector3.Normalize(transform.right);
        Vector3 flatZCam = Vector3.Normalize(transform.forward - (Vector3.Dot(Vector3.up, transform.forward)) * Vector3.up);

        return (xCam * Input.GetAxis("HorizontalRight")) + (flatZCam * Input.GetAxis("VerticalRight"));
    }
	
}
