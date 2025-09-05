using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int life;
    public int damage;
    public float hitYOffset;
    public Animator animator;
    public Renderer enemyRenderer;
    public Collider coll;
    public Rigidbody rb;

    public GameObject hitFx;
    public AudioClip hitSfx;

    public virtual void Move()
    {

    }

    public virtual void Attack()
    {

    }

    public void OnHit(int damage)
    {
        AudioManager.Instance.PlaySFX(hitSfx);

        life -= damage;

        if(life <= 0)
        {
            Death();
        }
        else
        {
            animator.SetTrigger("Hit");

            Player.Instance.ShakeCamera();

            StartCoroutine(HitFlash());

            GameObject hitEffect = Instantiate(hitFx, transform.position + new Vector3(0, hitYOffset, 0f), transform.rotation);
            Destroy(hitEffect, 2f);
        }    
    }

    IEnumerator HitFlash()
    {
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = Color.white;
    }

    public virtual void Death()
    {
        
    }
}
