using UnityEngine;

public class BottleHealth : MonoBehaviour
{
    public ItemData itemData;
    //public int recoveryAmount;
    public GameObject collectEffectPrefab;

    public AudioClip itemCollectSfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
            Inventory.Instance.AddItem(itemData);
            AudioManager.Instance.PlaySFX(itemCollectSfx);
           // Player.Instance.playerHealth.RecoveryHP(recoveryAmount);
            Destroy(gameObject);
        }
    }
}
