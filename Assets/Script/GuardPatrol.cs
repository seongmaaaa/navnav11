using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class GuardPatrol : MonoBehaviour
{
    public static GuardPatrol Instance { get; private set; }
    public Transform[] patrolPoints; // ���� ���� �迭
    private int currentPatrolIndex; // ���� ���� ���� �ε���
    public float patrolSpeed = 4f; // ���� �ӵ�
    public float chaseSpeed = 5f; // �߰� �ӵ�
    public float rayDistance = 5f; // Ray�� ����
    public LayerMask wallLayer; // �� ���̾�
    public float detectionRadius = 10f; // �÷��̾� ���� �ݰ�
    public Transform player; // �÷��̾� Transform
    public float alarmDuration = 10f;

    private NavMeshAgent agent; // NavMeshAgent ������Ʈ
    private Vector3 initialPosition; // �ʱ� ��ġ
    private bool returningToStart; // �ʱ� ��ġ�� ���ư��� ������ ����
    private bool isChasing; // �߰� ������ ����
    private bool isAlarmed; // �˶� ����
    private Vector3 alarmPosition; // �˶� ��ġ
    private float alarmEndTime; // �˶� ���� ���� �ð�
    private bool isPlayerInRoom; // �÷��̾ �� �ȿ� �ִ��� ����
    private bool playerHasKey; // �÷��̾ ���踦 ������ �ִ��� ����
    private bool isRoomSafe = false;
    void Start()
    {
        // NavMeshAgent ������Ʈ ��������
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;

        // �ʱ� ��ġ ����
        initialPosition = transform.position;

        // ù ���� ���� ����
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
            // ���� ������ ���¿����� �÷��̾� ���� �� �߰� ����� ��Ȱ��ȭ�մϴ�.
            return;
        }
        if (isAlarmed)
        {
            if (Time.time > alarmEndTime)
            {
                // �˶� ���°� ����Ǹ� �ٽ� ���� ����
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
                    Debug.Log("��� ��� ON ON");
                }
            }
        }
        else if (isChasing)
        {
            // �÷��̾� �߰�
            agent.SetDestination(player.position);
        }
        else
        {
            // �������� �����ߴ��� Ȯ��
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                if (returningToStart)
                {
                    // �ʱ� ��ġ�� �����ϸ� �ٽ� ���� ����
                    returningToStart = false;
                    currentPatrolIndex = 0;
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
                else if (currentPatrolIndex >= patrolPoints.Length - 1)
                {
                    // ������ ���� ������ �����ϸ� �ʱ� ��ġ�� ���ư���
                    agent.SetDestination(initialPosition);
                    returningToStart = true;
                }
                else
                {
                    // ���� ���� ���� ����
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
            }

            // ���� �����ϱ� ���� Ray�� ����
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, wallLayer))
            {
                // ���� ������ ���
                Debug.Log("�� ����!");

                // ���� ���ϱ� ���� ���ο� ���� ����
                Vector3 avoidDirection = Vector3.Reflect(transform.forward, hit.normal);
                Vector3 newDestination = transform.position + avoidDirection * rayDistance;

                // ���ο� �������� ����
                agent.SetDestination(newDestination);
            }
        }

        // �÷��̾� ����
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
            Debug.Log("�÷��̾� �߰� ���� �߰� ��� ON");
        }
    }

    void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            Debug.Log("�߰� ���� �Ϲ� ��� ON");
            // ���� ���� �������� ���ư���
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
        Debug.Log("�˶����� ���� �̵�");
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


