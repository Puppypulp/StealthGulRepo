using UnityEngine;
using System.Collections;

public class PlayerHealthController : MonoBehaviour 
{
    public float playerHealth = 10.0f;
    public float attackerDamage = 1f;
    public float damageColorIntensity = 4.0f;
    public GameObject playerMesh;
    public bool alive = true;

    private AttackCollisionBehaviour m_attackCollisionScript;
    private bool m_beingHit = false;
    
    private void Awake()
    {
        m_attackCollisionScript = gameObject.GetComponent<AttackCollisionBehaviour>();
        m_attackCollisionScript.AttackHitEvent += PlayerHit;
    }   

    private void PlayerHit(GameObject attacker)
    {
        if (attacker.tag == "EnemyAttack" && !m_beingHit)
        {
            Debug.Log("Player Hit");
            if (playerHealth > 0f)
            {
                playerHealth -= attackerDamage;
                PlayerHitPush(attacker);
            }
            else
            {
                PlayerDead();
                return;
            }

            StopCoroutine("DamageHitFlash");
            StartCoroutine("DamageHitFlash");
        }
    }

    private void PlayerDead()
    {
        Debug.Log("Player is dead");
        alive = false;
    }

    private void PlayerHitPush(GameObject attacker)
    {
        Vector3 vecToAttacker = Vector3.Normalize(transform.position - attacker.transform.position);
        Vector3 currentPos = transform.position;
        Vector3 offsetPos = new Vector3(vecToAttacker.x, 0f, vecToAttacker.z);

        transform.position = currentPos + (offsetPos * 1.5f);
    }

    // This controls a visual flash on the enemy to know you've hit them
    private IEnumerator DamageHitFlash()
    {
        m_beingHit = true;

        float startTime = Time.time;
        float waitTime = 0.25f;
        while ((Time.time - startTime) < waitTime)
        {
            float offset = (Time.time - startTime) * Random.Range(1f, 10f);
            float noiseVal = Mathf.PerlinNoise(offset, offset);

            playerMesh.GetComponent<Renderer>().materials[0].SetFloat("_Intensity", noiseVal * damageColorIntensity);
            playerMesh.GetComponent<Renderer>().materials[1].SetFloat("_Intensity", noiseVal * damageColorIntensity);
            playerMesh.GetComponent<Renderer>().materials[2].SetFloat("_Intensity", noiseVal * damageColorIntensity);
            yield return null;
        }

        playerMesh.GetComponent<Renderer>().materials[0].SetFloat("_Intensity", 0f);
        playerMesh.GetComponent<Renderer>().materials[1].SetFloat("_Intensity", 0f);
        playerMesh.GetComponent<Renderer>().materials[2].SetFloat("_Intensity", 0f);

        m_beingHit = false;
    }

    private void OnDestory()
    {
        m_attackCollisionScript.AttackHitEvent -= PlayerHit;
    }

}
