using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isPaused;
    public float moveSpeed;
    public float rotationSpeed;

    public ParticleSystem slashFx;
    public CinemachineImpulseSource impulseSource;

    public PlayerCustomizeBody customizeBody;
    public PlayerHealth playerHealth;
    public PlayerHUD playerHUD;

    public AudioClip swordSfx;

    CharacterController controller;
    Animator animator;

    Vector3 inputDirection;

    bool isAttacking;
    float attackCooldown = 0.53f;

    public static Player Instance;

    public Animator Animator { get => animator; set => animator = value; }

    //é chamado 1x antes do Start()
    private void Awake()
    {
       // Instance = this;

        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPaused)
        {
            Move();
            Attack();
            UpdateAttackState();
        }   
    }

    //mover o personagem
    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector3(horizontal, 0f, vertical);

        if(inputDirection != Vector3.zero && isAttacking == false)
        {
            //calculo para onde o personagem deve olhar
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);

            //executa a acao de rotacionar de maneira suave
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            //movendo o personagem
            controller.Move(inputDirection * moveSpeed * Time.deltaTime);
        }

        Animator.SetFloat("Speed", inputDirection.magnitude);
    }

    //executar animação de ataque
    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && isAttacking == false)
        {
            AudioManager.Instance.PlaySFX(swordSfx);
            Animator.SetTrigger("Attack");
            isAttacking = true;

            attackCooldown = 0.53f;

            slashFx.Play();
        }

    }

    //parar animação de ataque
    void UpdateAttackState()
    {
        if (isAttacking == true)
        {
            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0f)
            {
                isAttacking = false;
            }
        }
    }

    public void ShakeCamera()
    {
        impulseSource.GenerateImpulse();
    }
}
