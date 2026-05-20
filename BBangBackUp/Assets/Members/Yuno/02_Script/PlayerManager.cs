using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerManager : MonoBehaviour
{   
    public static PlayerManager Instance;
    public Animator animator;
    public bool isAttack;
    public bool isShovel;
    bool hasDeadAnimPlayed; 
    SpriteRenderer spriteRenderer;

    // 4방향을 명확하게 체크하기 위해 변수 정비
    [HideInInspector] public bool isRight;
    [HideInInspector] public bool isLeft;
    [HideInInspector] public bool isUp;
    [HideInInspector] public bool isDown;
    
    [Header("플레이어 이동 관련")]
    public float moveSpeed = 5f;
    public float originalMoveSpeed; 
    private Vector2 moveDir;
    public float minX, maxX, minY, maxY;

    [Header("애니메이션 관련")] 
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDead = Animator.StringToHash("Dead");
    private readonly int hashShout = Animator.StringToHash("Shout");
    private readonly int hashShovel = Animator.StringToHash("Shovel");
    public readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashGameRestart = Animator.StringToHash("GameRestart");

    private PlayerAttack playerAttack;

    private void Awake() 
    {
        Instance = this;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Start()
    {
        hasDeadAnimPlayed = false;
        isAttack = false;
        isShovel = false;

        // 기본 정면(아래) 바라보게 초기화
        isDown = true; 

        originalMoveSpeed = moveSpeed; 

        animator.ResetTrigger(hashGameRestart);
        animator.ResetTrigger(hashDead);

        animator.Rebind();
        animator.Update(0f);
    }

    void OnMove(InputValue value)
    {
        if (GameManager.instance.isDead)
        {
            moveDir = Vector2.zero;
            return;
        }
        moveDir = value.Get<Vector2>();
    }

    void Update()
    {   
        if (GameManager.instance.isDead)
        {
            if (!hasDeadAnimPlayed)
            {
                animator.SetTrigger(hashDead);
                hasDeadAnimPlayed = true;
            }
            return; 
        }

        float currentSpeed = moveDir.magnitude * moveSpeed;
        animator.SetFloat(hashSpeed, currentSpeed);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(hashShout);
        }

        if (isShovel || (playerAttack != null && playerAttack.IsAttacking))
        {
            return; 
        }

        // 방향 설정 로직 (움직임이 있을 때만 4방향 중 '가장 강한 입력 방향' 하나만 true로 세팅)
        if (moveDir != Vector2.zero)
        {
            // 수평(X) 입력이 더 클 때
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
            {
                isUp = false;
                isDown = false;
                
                if (moveDir.x > 0)
                {
                    spriteRenderer.flipX = false;
                    isRight = true;
                    isLeft = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                    isRight = false;
                    isLeft = true;
                }
            }
            // 수직(Y) 입력이 더 클 때 (또는 같을 때 위쪽 우선 원하면 여기에 포함)
            else
            {
                isRight = false;
                isLeft = false;

                if (moveDir.y > 0)
                {
                    isUp = true;
                    isDown = false;
                }
                else
                {
                    isUp = false;
                    isDown = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance.isDead) return;
        
        Vector3 nextPos = transform.position + (Vector3)(moveDir * moveSpeed * Time.fixedDeltaTime);

        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);

        transform.position = nextPos;
    }

    public void PlayerShovel()
    {
        if(GameManager.instance.isDead || isAttack || isShovel) return;

        isShovel = true;
        animator.SetTrigger(hashShovel);
        originalMoveSpeed = moveSpeed; 
        moveSpeed = 0f;
        
        StartCoroutine(MoveStopInShovel(0.75f));
    }

    IEnumerator MoveStopInShovel(float time)
    {
        yield return new WaitForSeconds(time);
        moveSpeed = originalMoveSpeed; 
        isShovel = false;
    }
}