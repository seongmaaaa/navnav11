using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public float pickupRadius = 3f; // ���踦 ȹ���� �� �ִ� �ݰ�

    void Update()
    {
        // �÷��̾���� �Ÿ� ���
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= pickupRadius)
        {
            // �÷��̾�� ���踦 ȹ��
            PlayerController.Instance.HasKey = true;
            Destroy(gameObject); // ���� ������Ʈ ����
        }
    }
}
