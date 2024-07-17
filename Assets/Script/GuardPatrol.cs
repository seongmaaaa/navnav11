using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class GuardPatrol : MonoBehaviour
{
    public static GuardPatrol Instance { get; private set; }
    public Transform[] patrolPoints; // 순찰 지점 배열
    private int currentPatrolIndex; // 현재 순찰 지점 인덱스
    public float patrolSpeed = 4f; // 순찰 속도
    public float chaseSpeed = 5f; // 추격 속도
    public float rayDistance = 5f; // Ray의 길이
    public LayerMask wallLayer; // 벽 레이어
    public float detectionRadius = 10f; // 플레이어 감지 반경
    public Transform player; // 플레이어 Transform
    public float alarmDuration = 10f;

    private NavMeshAgent agent; // NavMeshAgent 컴포넌트
    private Vector3 initialPosition; // 초기 위치
    private bool returningToStart; // 초기 위치로 돌아가는 중인지 여부
    private bool isChasing; // 추격 중인지 여부
    private bool isAlarmed; // 알람 상태
    private Vector3 alarmPosition; // 알람 위치
    private float alarmEndTime; // 알람 상태 종료 시간
    private bool isPlayerInRoom; // 플레이어가 방 안에 있는지 여부
    private bool playerHasKey; // 플레이어가 열쇠를 가지고 있는지 여부
    private bool isRoomSafe = false;
    void Start()
    {
        // NavMeshAgent 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        // 초기 위치 저장
        initialPosition = transform.position;

        // 첫 순찰 지점 설정
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            returningToStart = false;
        }
    }

    private void Update()
    {
        if (isRoomSafe)
        {
            // 방이 안전한 상태에서는 플레이어 감지 및 추격 기능을 비활성화합니다.
            return;
        }
        if (isAlarmed)
        {
            if (Time.time > alarmEndTime)
            {
                // 알람 상태가 종료되면 다시 순찰 시작
                isAlarmed = false;
                returningToStart = true;
                agent.speed = patrolSpeed;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
            else
            {
                agent.SetDestination(alarmPosition);
                if (agent.remainingDistance < 0.5f)
                {
                    Debug.Log("경계 모드 ON ON");
                }
            }
        }
        else if (isChasing)
        {
            // 플레이어 추격
            agent.SetDestination(player.position);
        }
        else
        {
            // 목적지에 도달했는지 확인
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                if (returningToStart)
                {
                    // 초기 위치에 도착하면 다시 순찰 시작
                    returningToStart = false;
                    currentPatrolIndex = 0;
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
                else if (currentPatrolIndex >= patrolPoints.Length - 1)
                {
                    // 마지막 순찰 지점에 도달하면 초기 위치로 돌아가기
                    agent.SetDestination(initialPosition);
                    returningToStart = true;
                }
                else
                {
                    // 다음 순찰 지점 설정
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
            }

            // 벽을 감지하기 위해 Ray를 생성
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, wallLayer))
            {
                // 벽이 감지된 경우
                Debug.Log("벽 감지!");

                // 벽을 피하기 위한 새로운 방향 설정
                Vector3 avoidDirection = Vector3.Reflect(transform.forward, hit.normal);
                Vector3 newDestination = transform.position + avoidDirection * rayDistance;

                // 새로운 목적지를 설정
                agent.SetDestination(newDestination);
            }
        }

        // 플레이어 감지
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            StartChasing();
        }
        else
        {
            StopChasing();
        }
    }

    void StartChasing()
    {
        if (!isChasing && !isPlayerInRoom)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            Debug.Log("플레이어 추격 시작 추격 모드 ON");
        }
    }

    void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            Debug.Log("추격 종료 일반 모드 ON");
            // 현재 순찰 지점으로 돌아가기
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    public void OnAlarmTriggered(Vector3 position)
    {
        isAlarmed = true;
        alarmPosition = position;
        alarmEndTime = Time.time + alarmDuration;
        agent.speed = chaseSpeed;
        agent.SetDestination(alarmPosition);
        Debug.Log("알람반응 경비원 이동");
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public bool PlayerHasKey
    {
        get { return playerHasKey; }
        set { playerHasKey = value; }
    }
    
}


