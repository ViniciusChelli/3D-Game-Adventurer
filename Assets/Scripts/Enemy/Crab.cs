using Unity.VisualScripting;
using UnityEngine;

public class Crab : Enemy
{
    bool isPlayerVisible;

    public float moveSpeed;
    public float attackRange;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    public float detectionRadius;
    public float rotationSpeed;

    public GameObject canvaAlert;
//    public float alertYOffset;

    public EnemyDamageHitbox clawHitbox;

    float distance;

    private void Start()
    {
        clawHitbox.damage = damage;
    }

    private void Update()
    {
        distance = Vector3.Distance(transform.position, Player.Instance.transform.position);

        if (isPlayerVisible == false)
        {           
            if(distance <= detectionRadius)
            {
                isPlayerVisible = true;
                GameObject alert = Instantiate(canvaAlert);
                alert.GetComponent<EnemyAlert>().target = transform;
                Destroy(alert, 2f);
                //GameObject alert = Instantiate(canvaAlert, transform.position + new Vector3(0,alertYOffset,0), transform.rotation);
                //alert.transform.forward = Camera.main.transform.forward;
                //Destroy(alert, 2f);
            }
        }
        else
        {
            if(distance > detectionRadius)
            {
                isPlayerVisible = false;
                animator.SetFloat("Speed", 0);
                return;
            }

            RotateTowardsPlayer();

            if(distance > attackRange)
            {
                animator.SetFloat("Speed", 1);
                Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;
                direction.y = 0;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }

            if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown && !Player.Instance.playerHealth.isDead)
            {
                //executa o ataque
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    public override void Attack()
    {
        animator.SetTrigger("Attack");
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;
        direction.y = 0f;

        if(direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public override void Death()
    {
        animator.SetTrigger("Die");
        enabled = false;
        rb.useGravity = false;
        coll.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
