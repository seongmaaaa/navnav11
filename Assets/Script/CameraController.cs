using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    void Update()
    {
        // ī�޶� ��ġ ����
        transform.position = player.position + offset;

        transform.LookAt(player);
    }
}

