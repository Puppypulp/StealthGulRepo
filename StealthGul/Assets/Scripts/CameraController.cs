using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
    public GameObject followObject;
    public float catchupSpeed = 5.0f;
    public float xOffset = 0.0f;
    public float zOffset = 0.0f;
    public float yOffset = 0.0f;
    private Vector3 smoothedCamPos = Vector3.zero;

    private void Awake()
    {
        smoothedCamPos = new Vector3((followObject.transform.position.x + xOffset), (followObject.transform.position.y + yOffset), (followObject.transform.position.z + zOffset));
    }

    private void Update()
    {
        // Set the adjusted position to be the positon of the follow object iased by the offset assigned by the x and z offset
        Vector3 adjustedCamPos = new Vector3((followObject.transform.position.x + xOffset), (followObject.transform.position.y + yOffset), (followObject.transform.position.z + zOffset));

        // The smoothed value of the cam position lerp from the camera current pos to the adjusted cam pos. This is constantly updated every frame
        smoothedCamPos = Vector3.Lerp(transform.position, adjustedCamPos, (Time.deltaTime * catchupSpeed));

        // Set the came pos to the adjusted position every frame
        transform.position = smoothedCamPos;
    }
	
}
