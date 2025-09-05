using UnityEngine;

public class EnemyDamageHitbox : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
       //     Player.Instance.ShakeCamera();
        }
    }
}
