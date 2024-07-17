using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVController : MonoBehaviour
{
    public float rotationSpeed = 30f; // ȸ�� �ӵ�
    public float detectionAngle = 30f; // �ν� ����
    public float detectionDistance = 6f; // �ν� �Ÿ�
    public LayerMask playerLayer; // �÷��̾� ���̾�
    public LayerMask obstacleLayer; // ��ֹ� ���̾�
    public GameObject alarmLight; // �˶� ǥ�� (���� �Ǵ� ��� ��)

    private bool alarmTriggered = false; // �˶� ����

    void Update()
    {
        // CCTV �ڵ� ȸ��
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // �÷��̾� Ž��
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionDistance, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToPlayer = (hitCollider.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < detectionAngle / 2)
            {
                // ��ֹ� üũ
                if (!Physics.Raycast(transform.position, directionToPlayer, detectionDistance, obstacleLayer))
                {
                    // �÷��̾� �߰�
                    TriggerAlarm();
                    break;
                }
            }
        }
    }

    void TriggerAlarm()
    {
        if (!alarmTriggered)
        {
            alarmTriggered = true;
            alarmLight.SetActive(true); // �˶� ǥ�� Ȱ��ȭ
            Debug.Log("�÷��̾� �߰�! �˶��� �︳�ϴ�.");

            
            GuardManager.Instance.AlertGuards(transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
        Gizmos.color = Color.blue;
        Vector3 frontRayPoint = transform.position + (transform.forward * detectionDistance);
        Gizmos.DrawLine(transform.position, frontRayPoint);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionDistance);
    }
}
