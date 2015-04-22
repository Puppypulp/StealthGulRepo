using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateVisionMesh : MonoBehaviour 
{
    public GameObject enemyObject;
    public int degrees = 90;

    private void Awake()
    {
        //gameObject.AddComponent<MeshFilter>();
        //gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();

        Vector3 origin = transform.position;
        Quaternion startRefRotation = transform.rotation;
        // Add first point
        vertices.Add(transform.position);

        // Create Verts
        for (int i = 0; i < degrees; i++)
        {
            transform.rotation = Quaternion.AngleAxis((float)i, transform.up);
            Vector3 offsetVert = (transform.position + (transform.forward * 10.0f));
            vertices.Add(offsetVert);
        }
        mesh.vertices = vertices.ToArray();

        List<int> triangles = new List<int>();

        for (int i = 0; i < degrees - 1; i++)
        {
            if (i == 0)
            {
                triangles.Add(i); 
            }
            else
            {
                triangles.Add(0);
            }           
            triangles.Add(i + 1); 
            triangles.Add(i + 2); 
        }

        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();

        transform.rotation = Quaternion.AngleAxis((float)degrees * -0.5f, transform.up);
    }

    private void Update()
    {
        transform.position = enemyObject.transform.position;

        transform.rotation = Quaternion.AngleAxis((float)degrees * -0.5f, transform.up);
        Vector3 lookVec = enemyObject.transform.rotation * transform.forward;
        transform.rotation = Quaternion.LookRotation(lookVec, transform.up);
    }
}
