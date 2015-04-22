using UnityEngine;
using System.Collections;

public class EnemyHealthController : MonoBehaviour 
{
    public int enemyHealth = 10;
    public int awareDamage = 1;
    public int cautiousDamage = 2;

    private AttackCollisionBehaviour m_attackScript;
    private EnemyPatrol m_enemyPatrolScript;

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

            if (enemyHealth <= 0)
                EnemyDead();
        }
    }

    private void EnemyDead()
    {
        Destroy(gameObject);
    }

    private void OnDestory()
    {
        m_attackScript.AttackHitEvent -= HitByPlayer;
    }
}
