using UnityEngine;
using System.Collections;

public class Monster1 : MonoBehaviour, IDamageable
{
    [Header("Data Reference")]
    public MonsterData data; // 몬스터 데이터 ScriptableObject와 연결

    [Header("Safe Zone")]
    public LayerMask safeZoneLayer;//세이프존 레이어
    public float safeZoneCheckRadius = 0.2f;//세이프존 체크하는 반경

    [Header("Required Objects")]
    public GameObject attackHitbox;//공격 범위 체크하는 오브젝트(애니메이션 이벤트로 활성화/비활성화)

    // 개별 인스턴스의 상태 데이터
    private int _currentHp; //개별 인스턴스의 현재 체력
    
    protected Rigidbody2D _rb;//몬스터의 리지드바디
    protected Transform _playerTrm;//플레이어 트랜스폼
    protected Animator _anim;//몬스터 애니메이터

    private bool _inChaseRange = false;//추적 범위 안에 있는지 여부
    private bool _inAttackRange = false;//공격 범위 안에 있는지 여부
    private bool _isAttacking = false;//공격 중인지 여부
    private bool _canAttack = true;//공격 가능한 상태인지 여부

    private Coroutine _logicCoroutine;//몬스터의 메인 로직 코루틴 참조
    private const string PLAYER_TAG = "Player";//플레이어 태그 상수

    private readonly int hash_isWalking = Animator.StringToHash("isWalking");//애니메이터 파라미터 해시값
    private readonly int hash_Attack = Animator.StringToHash("Attack");//애니메이터 파라미터 해시값

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();//리지드바디 가져오기
        _anim = GetComponent<Animator>();//애니메이터 가져오기

        //몬스터 초기 상태 설정
        if (attackHitbox != null) //공격 범위 있으면
        {
            attackHitbox.SetActive(false);//공격 범위 오브젝트 비활성화
        }
        if (data != null)//데이터 있으면
        {
            _currentHp = data.maxHp;//최대 체력으로 초기화
        }
    }

    protected virtual void Start()
    {
        GameObject player = GameObject.FindWithTag(PLAYER_TAG);//플레이어 태그로 플레이어 오브젝트 찾기
        if (player != null) _playerTrm = player.transform;//플레이어 트랜스폼 저장
    }

    public void SetChasing(bool value)//추적 범위 안에 들어왔는지 여부 설정하는 함수(트리거에서 호출)
    {
        _inChaseRange = value;//추적 범위 안에 들어왔는지 여부 설정
        if (_inChaseRange && _logicCoroutine == null)//추적 범위 안에 들어왔고 아직 로직 코루틴이 실행 중이 아니면
            _logicCoroutine = StartCoroutine(MainLogicRoutine());//메인 로직 코루틴 시작
            SoundManager.instance.PlaySFX("ZombieChase");
    }

    public void SetAttackRange(bool value) => _inAttackRange = value;

    private IEnumerator MainLogicRoutine()//몬스터의 메인 로직 코루틴
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();//물리 업데이트마다 실행

        while (_inChaseRange && _playerTrm != null)//추적 범위 안에 있고 플레이어가 존재하는 동안
        {
            if (IsPlayerInSafeZone())//플레이어가 세이프존 안에 있으면
            {
                StopMonster();//몬스터 멈추기
                yield return wait;//다음 물리 업데이트까지
                continue;//세이프존 안에서는 추적하지 않고 계속 체크만 함
            }

            if (_inAttackRange && _canAttack && !_isAttacking)//공격 범위 안에 있고 공격 가능하며 현재 공격 중이 아니면
            {
                yield return StartCoroutine(AttackRoutine());//공격 루틴 실행
            }

            if (!_isAttacking)//공격 중이 아니면
            {
                if (!_inAttackRange)//공격 범위 안에 있지 않으면
                {
                    Vector2 dir = ((Vector2)_playerTrm.position - (Vector2)transform.position).normalized;//플레이어 방향 계산
                    _rb.linearVelocity = dir * data.speed; //몬스터 이동
                    _anim.SetBool(hash_isWalking, true);//걷는 애니메이션 재생

                    if (Mathf.Abs(dir.x) > 0.1f)//플레이어 방향에 따라 몬스터 좌우 반전
                        transform.localScale = new Vector3(dir.x > 0 ? -1 : 1, 1, 1);//플레이어가 오른쪽에 있으면 왼쪽으로, 왼쪽에 있으면 오른쪽으로
                }
                else
                {
                    StopMonster();//공격 범위 안에 있으면 몬스터 멈추기
                }
            }
            yield return wait;//다음 물리 업데이트까지 기다림
        }

        StopMonster();//추적 범위를 벗어나면 몬스터 멈추기
        _logicCoroutine = null;
    }

    private bool IsPlayerInSafeZone()//플레이어가 세이프존 안에 있는지 체크하는 함수
    {
        if (_playerTrm == null) return false;//플레이어 트랜스폼이 없으면 세이프존 안에 있지 않다고 간주
        return Physics2D.OverlapCircle(_playerTrm.position, safeZoneCheckRadius, safeZoneLayer) != null;
        //플레이어 위치를 중심으로 반경 내에 세이프존 레이어가 있는지 체크하여 세이프존 안에 있는지 여부 반환
    }

    private IEnumerator AttackRoutine()//공격 루틴 코루틴
    {
        _isAttacking = true;//공격 중 상태로 설정
        _canAttack = false;//공격 불가능 상태로 설정

        _rb.linearVelocity = Vector2.zero;//공격 시작 시 몬스터 멈추기
        _anim.SetBool(hash_isWalking, false);//걷는 애니메이션 끄기
        _anim.SetTrigger(hash_Attack);//공격 애니메이션 트리거

        yield return new WaitForSeconds(0.3f);//공격 애니메이션에서 타이밍 맞춰서 공격 범위 활성화 (예: 0.3초 후)
        if (attackHitbox != null) attackHitbox.SetActive(true);//공격 범위 활성화
        
        
        yield return new WaitForSeconds(0.2f);//공격 범위 활성화 유지 시간 (예: 0.2초)
        if (attackHitbox != null) attackHitbox.SetActive(false);//공격 범위 비활성화

        _isAttacking = false;//공격 중 상태 해제
        StartCoroutine(CooldownRoutine());//공격 쿨타임 시작
    }

    private IEnumerator CooldownRoutine()//공격 쿨타임 코루틴
    {
        yield return new WaitForSeconds(data.attackCooldown);//데이터에서 설정한 공격 쿨타임만큼 대기
        _canAttack = true;//공격 가능 상태로 설정
    }

    private void StopMonster()//몬스터 멈추는 함수
    {
        if (_rb != null) _rb.linearVelocity = Vector2.zero;//리지드바디 속도 0으로 설정하여 멈추기
        if (_anim != null) _anim.SetBool(hash_isWalking, false);//걷는 애니메이션 끄기
    }
    
    public void TakeDamage(int amount)//피해 입는 함수
    {
        _currentHp -= amount;//현재 체력에서 피해량만큼 감소
        Debug.Log("데미지:"+amount);
        if(_currentHp <= 0) //체력이 0 이하가 되면 사망 처리
        {
            Debug.Log($"{data.monsterName} 사망!");//몬스터 이름과 사망 메시지 출력
            Destroy(gameObject);//몬스터 오브젝트 파괴시키기
        }
    }
}