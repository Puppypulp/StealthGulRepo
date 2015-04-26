using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateVisionMesh : MonoBehaviour 
{
    public GameObject enemyObject;    
    public float cosValue = 0.5f;

    private int degrees = 90;    

    private void Awake()
    {
        transform.position = new Vector3(0f, 0.25f, 0f);

        gameObject.GetComponent<MeshRenderer>().enabled = true;

        float angle = Utilities.CosToAngleDegree(cosValue) * 2f;
        degrees = (int)angle;
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

        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
        }
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        mesh.RecalculateBounds();

        transform.rotation = Quaternion.AngleAxis((float)degrees * -0.5f, transform.up);
       
    }

    private void Update()
    {
        if (enemyObject != null)
        {
            transform.position = enemyObject.transform.position;

            transform.rotation = Quaternion.AngleAxis((float)degrees * -0.5f, transform.up);
            Vector3 lookVec = enemyObject.transform.rotation * transform.forward;
            transform.rotation = Quaternion.LookRotation(lookVec, transform.up);
        }

        if (enemyObject == null)
            Destroy(gameObject);
    }
}
