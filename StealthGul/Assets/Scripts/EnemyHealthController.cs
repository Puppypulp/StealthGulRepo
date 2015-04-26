using UnityEngine;
using System.Collections;

public class EnemyHealthController : MonoBehaviour 
{
    public int enemyHealth = 10;
    public int awareDamage = 1;
    public int cautiousDamage = 2;
    public GameObject enemyOverlayMesh;
    public float damageColorIntensity = 4.0f;
    public ParticleSystem unawareHitEffect;

    private AttackCollisionBehaviour m_attackScript;
    private EnemyPatrol m_enemyPatrolScript;

    private bool m_beingHit = false;

    private void Awake()
    {
        m_attackScript = gameObject.GetComponent<AttackCollisionBehaviour>();
        m_enemyPatrolScript = gameObject.GetComponent<EnemyPatrol>();

        m_attackScript.AttackHitEvent += HitByPlayer;
    }

    private void HitByPlayer(GameObject attacker)
    {
        if (attacker.tag == "PlayerAttack")
        {
            switch (m_enemyPatrolScript.currentEnemyAwareness)
            {
                case EnemyPatrol.EnemyAwareState.unaware:
                    enemyHealth = 0;
                    break;
                case EnemyPatrol.EnemyAwareState.aware:
                    enemyHealth -= awareDamage;
                    break;
                case EnemyPatrol.EnemyAwareState.cautious:
                    enemyHealth -= cautiousDamage;
                    break;
            }

            StopCoroutine("DamageHitFlash");
            StartCoroutine("DamageHitFlash");

            if (enemyHealth <= 0)
            {
                StartCoroutine("EnemyDead");
            }
        }
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
           
            enemyOverlayMesh.GetComponent<Renderer>().materials[2].SetFloat("_Intensity", noiseVal * damageColorIntensity);
            yield return null;
        }
        
        enemyOverlayMesh.GetComponent<Renderer>().materials[2].SetFloat("_Intensity", 0f);

        m_beingHit = false;
    }

    private IEnumerator EnemyDead()
    {
        float totalWaitTime = unawareHitEffect.startLifetime;

        yield return null;

        unawareHitEffect.Play();
        Time.timeScale = 0.1f;

        yield return new WaitForSeconds(totalWaitTime * 0.75f);

        Time.timeScale = 1f;

        yield return new WaitForSeconds(totalWaitTime * 0.25f);

        Destroy(gameObject);
    }

    private void OnDestory()
    {
        m_attackScript.AttackHitEvent -= HitByPlayer;
    }
}
