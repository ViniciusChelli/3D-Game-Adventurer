using UnityEngine;

public class LootBox : MonoBehaviour
{
    public GameObject idleBox;
    public GameObject destroyBox;
    public Animator anim;
    public Collider coll;
    public Transform spawnPoint;

    public GameObject[] loots;

    public AudioClip destroyBoxSfx;
    public AudioClip spawnItem;

    public void DestroyBox()
    {
        AudioManager.Instance.PlaySFX(destroyBoxSfx);
        AudioManager.Instance.PlaySFX(spawnItem);

        coll.enabled = false;
        Instantiate(loots[Random.Range(0, loots.Length)], spawnPoint.position, Quaternion.identity);
        
        idleBox.SetActive(false);
        destroyBox.SetActive(true);
        anim.Play("Bang");
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<SwordHitbox>())
        {
            DestroyBox();
        }
    }
}
