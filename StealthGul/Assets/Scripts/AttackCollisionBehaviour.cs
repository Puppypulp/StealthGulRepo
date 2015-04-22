using UnityEngine;
using System.Collections;

public class AttackCollisionBehaviour : MonoBehaviour 
{
    public delegate void AttackEvent(GameObject attackerObject);
    public AttackEvent AttackHitEvent;

    private void OnTriggerEnter(Collider col)
    {
        if (AttackHitEvent != null)
        {
            AttackHitEvent(col.gameObject);
        }
    }
}
