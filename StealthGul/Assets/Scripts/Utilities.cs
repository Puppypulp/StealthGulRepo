using UnityEngine;
using System.Collections;

public class Utilities
{
    // Distance betwen two objects 
    public static float DistanceBetween(GameObject obj1, GameObject obj2)
    {
        Vector3 vecToEnemy = obj1.transform.position - obj2.transform.position;
        float distanceBetween = Vector3.Magnitude(vecToEnemy);
        return distanceBetween;
    }

    // Function flattens a vector with the world y
	public static Vector3 FlattenVector(Vector3 inVec)
	{
		return inVec - (Vector3.Dot(inVec, Vector3.up)) * Vector3.up;
	}

    public static float LerpScale( float value, float from1, float to1, float from2, float to2) 
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float CosToAngleDegree(float cosValue)
    {
        float angle = Mathf.Acos(cosValue);
        float degree = angle * (180.0f / Mathf.PI);

        return degree;
    }
}
