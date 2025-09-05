using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject collectEffectPrefab;

    public AudioClip itemCollectSfx;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
            AudioManager.Instance.PlaySFX(itemCollectSfx);
            GameManager.Instance.AddCoin();
            Destroy(gameObject);
        }
    }
}
