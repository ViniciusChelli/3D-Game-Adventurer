using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead;
    public int life = 1000;

    public Image[] hearts;
    public Sprite heartFull;
    public Sprite heartEmpty;

    public GameObject gameOverPanel;
    public GameObject playerHitFx;

    public AudioClip hitSfx;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void RecoveryHP(int amount)
    {
        life += amount;

        if(life >= 100)
        {
            life = 100;
        }

        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        AudioManager.Instance.PlaySFX(hitSfx);
        Player.Instance.ShakeCamera();

        GameObject hitEffect = Instantiate(playerHitFx, transform.position + new Vector3(0, 1f, 0f), transform.rotation);
        Destroy(hitEffect, 2f);

        life -= damage;

        if(life <= 0)
        {
            Death();
        }
        else
        {
            //toma hit

            player.Animator.SetTrigger("Hit");
            StartCoroutine(DelayedHit());
        }

        UpdateHearts();
    }

    private void UpdateHearts()
    {
        int totalHearts = hearts.Length;
        int healthPerHeart = 100 / totalHearts;
        int fullHearts = Mathf.CeilToInt((float)life / healthPerHeart);

        for (int i = 0; i < totalHearts; i++)
        {
            if(i < fullHearts)
            {
                hearts[i].sprite = heartFull;
            }
            else
            {
                hearts[i].sprite = heartEmpty;
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("main_menu");
        Destroy(gameObject);
    }

    void Death()
    {
        player.Animator.SetTrigger("Die");
        player.isPaused = true;
        isDead = true;
        StartCoroutine(DelayedGameOver());
    }

    IEnumerator DelayedHit()
    {
        player.isPaused = true;
        yield return new WaitForSeconds(0.5f);
        player.isPaused = false;
    }

    IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(1f);
        gameOverPanel.SetActive(true);
    }
}
