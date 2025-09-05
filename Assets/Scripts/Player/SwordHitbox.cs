using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            other.GetComponent<Enemy>().OnHit(damage);
        }
    }

}
