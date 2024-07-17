using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    public NavMeshAgent myAgent;
    public Transform targetTr;
    public float rayDistance = 5f; // Ray�� ����
    public LayerMask wallLayer; // �� ���̾�

    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, wallLayer))//���̾� ����
        {
            // ���� ������ ���
            Debug.Log("�� ����!");

            // ���� ���ϱ� ���� ���ο� ���� ����
            Vector3 avoidDirection = Vector3.Reflect(transform.forward, hit.normal);
            Vector3 newDestination = transform.position + avoidDirection * rayDistance;

            // ���ο� �������� ����
            myAgent.SetDestination(newDestination);
        }
        else
        {
            // ���� �������� ���� ��� ���� �������� �̵�
            myAgent.SetDestination(targetTr.position);
        }
    }
}

