using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Werewolf : Enemy
{
    // Visão/Detecção
    [Header("Detecção")]
    public float detectionRadius = 12f;
    public float attackRange = 2.2f;         // ataque curto (garra/bocada)
    public float leapRangeMin = 3.0f;        // mínimo para considerar salto
    public float leapRangeMax = 6.5f;        // máximo para considerar salto
    public float rotationSpeed = 8f;

    // Movimento
    [Header("Movimento")]
    public float walkSpeed = 2.4f;
    public float runSpeed = 4.2f;            // usado quando enfurecido
    public float fixedMoveTick = 0.02f;      // para usar MovePosition com passo estável

    // Ataques
    [Header("Ataques")]
    public float basicAttackCooldown = 1.6f;
    public float leapAttackCooldown = 4.5f;
    public EnemyDamageHitbox clawOrBiteHitbox;   // hitbox do ataque básico
    public EnemyDamageHitbox leapHitbox;         // hitbox que ativa no pico do salto (opcional)

    // Feedback/Uivo
    [Header("Feedback")]
    public GameObject canvaAlert;             // UI de alerta (ex: "!")
    public float alertLifetime = 2f;

    // Enfurecido
    [Header("Enfurecido")]
    [Range(0f, 1f)] public float enrageHealthThreshold = 0.35f; // % da vida
    public float enrageRunSpeedBonus = 1.25f;                   // multiplica runSpeed
    private bool isEnraged;

    // Internos
    private bool isPlayerVisible;
    private float lastBasicTime;
    private float lastLeapTime;
    private float moveAccumulator;

    // Referências herdadas de Enemy:
    // protected Animator animator;
    // protected Rigidbody rb;
    // protected Collider coll;
    // protected int damage; (usando para clawOrBiteHitbox)
    // public override void Death()

    private void Start()
    {
        if (clawOrBiteHitbox != null)
            clawOrBiteHitbox.damage = damage;

        if (leapHitbox != null)
            leapHitbox.damage = Mathf.RoundToInt(damage * 1.25f); // salto bate um pouco mais forte
    }

    private void Update()
    {
        if (Player.Instance == null || Player.Instance.playerHealth == null) return;

        float dist = Vector3.Distance(transform.position, Player.Instance.transform.position);

        // Checa e ativa Enfurecido
        MaybeEnterEnrage();

        if (!isPlayerVisible)
        {
            if (dist <= detectionRadius)
            {
                isPlayerVisible = true;

                // Uivo/alerta visual
                if (canvaAlert != null)
                {
                    GameObject alert = Instantiate(canvaAlert);
                    var ea = alert.GetComponent<EnemyAlert>();
                    if (ea != null) ea.target = transform;
                    Destroy(alert, alertLifetime);
                }

                if (animator) animator.SetTrigger("Howl");
            }
            return;
        }
        else
        {
            // Perdeu o alvo
            if (dist > detectionRadius)
            {
                isPlayerVisible = false;
                if (animator) animator.SetFloat("Speed", 0f);
                return;
            }
        }

        // Rotaciona para o jogador
        RotateTowardsPlayer();

        // Decide ação: mover, ataque básico ou salto
        if (!Player.Instance.playerHealth.isDead)
        {
            // Salto tem prioridade quando na janela de distância
            if (dist >= leapRangeMin && dist <= leapRangeMax && Time.time - lastLeapTime >= leapAttackCooldown)
            {
                // não se mexe durante a preparação do salto
                if (animator) animator.SetFloat("Speed", 0f);
                DoLeap();
                lastLeapTime = Time.time;
            }
            else if (dist > attackRange)
            {
                // mover
                float speed = isEnraged ? runSpeed * enrageRunSpeedBonus : walkSpeed;
                if (animator) animator.SetFloat("Speed", 1f);
                MoveTowards(Player.Instance.transform.position, speed);
            }
            else
            {
                // em alcance de ataque básico
                if (animator) animator.SetFloat("Speed", 0f);

                if (Time.time - lastBasicTime >= basicAttackCooldown)
                {
                    Attack();
                    lastBasicTime = Time.time;
                }
            }
        }
        else
        {
            if (animator) animator.SetFloat("Speed", 0f);
        }
    }

    public override void Attack()
    {
        if (animator) animator.SetTrigger("Attack");
        // A ativação da hitbox básica deve ser anim-event (Enable/Disable collider)
        // Ex: Animation Event chama: EnableBasicHitbox() e DisableBasicHitbox()
    }

    private void DoLeap()
    {
        if (animator) animator.SetTrigger("Leap");
        // Sugestão: usar root motion ou um impulso aplicado via Animation Event:
        // - No pico do salto, chamar EnableLeapHitbox(), depois DisableLeapHitbox()
        // - Se preferir “impulso” via script, você pode aplicar rb.AddForce() no evento.
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0f;

        // acumulador para simular um passo fixo (MovePosition é recomendado no FixedUpdate,
        // mas aqui garantimos passo constante mesmo no Update)
        moveAccumulator += Time.deltaTime;
        while (moveAccumulator >= fixedMoveTick)
        {
            rb.MovePosition(rb.position + dir * speed * fixedMoveTick);
            moveAccumulator -= fixedMoveTick;
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void MaybeEnterEnrage()
    {
        if (isEnraged) return;

        // Enemy deve ter um jeito de expor a vida atual / máxima.
        // Se você tiver currentHealth/maxHealth no Enemy, ajuste aqui.
        // Exemplo genérico (substitua conforme seu Enemy):
        float hpPct = GetHealthPercent();
        if (hpPct >= 0f && hpPct <= enrageHealthThreshold)
        {
            isEnraged = true;
            // Você pode tocar um rugido específico aqui se quiser:
            // animator?.SetTrigger("Howl");
        }
    }

    // Placeholder: ajuste para ler do seu Enemy real.
    private float GetHealthPercent()
    {
        // Se o seu Enemy tiver algo como: currentHealth e maxHealth,
        // substitua esse método por:
        // return (float)currentHealth / Mathf.Max(1, maxHealth);
        return 1f; // remova isto e implemente com seus campos reais.
    }

    public override void Death()
    {
        if (animator) animator.SetTrigger("Die");
        enabled = false;
        rb.useGravity = false;
        coll.enabled = false;
    }

    // Chamados por Animation Events
   public void EnableBasicHitbox()
{
    if (clawOrBiteHitbox)
    {
        var col = clawOrBiteHitbox.GetComponent<Collider>();
        if (col) col.enabled = true;
        else clawOrBiteHitbox.gameObject.SetActive(true); // fallback
    }
}

public void DisableBasicHitbox()
{
    if (clawOrBiteHitbox)
    {
        var col = clawOrBiteHitbox.GetComponent<Collider>();
        if (col) col.enabled = false;
        else clawOrBiteHitbox.gameObject.SetActive(false); // fallback
    }
}

public void EnableLeapHitbox()
{
    if (leapHitbox)
    {
        var col = leapHitbox.GetComponent<Collider>();
        if (col) col.enabled = true;
        else leapHitbox.gameObject.SetActive(true);
    }
}

public void DisableLeapHitbox()
{
    if (leapHitbox)
    {
        var col = leapHitbox.GetComponent<Collider>();
        if (col) col.enabled = false;
        else leapHitbox.gameObject.SetActive(false);
    }
}


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;    Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;   Gizmos.DrawWireSphere(transform.position, leapRangeMin);
        Gizmos.color = Color.blue;   Gizmos.DrawWireSphere(transform.position, leapRangeMax);
    }
}
