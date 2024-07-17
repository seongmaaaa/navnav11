using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    private bool hasKey = false; // ���� ���� ����

    void Awake()
    {
        instance = this;
    }

    public bool HasKey
    {
        get { return hasKey; }
        set { hasKey = value; }
    }
    public float moveSpeed = 5f; // �÷��̾� �̵� �ӵ�
    public float lookSpeed = 2f; // ī�޶� ȸ�� �ӵ�
    public float jumpForce = 5f; // ���� ��
    public Camera playerCamera; // �÷��̾� ī�޶�

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        RotateCamera();
    }

    void MovePlayer()
    {
        // �̵� �Է�
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // �̵� ���� ���
        Vector3 move = transform.right * moveHorizontal + transform.forward * moveVertical;
        Vector3 moveVelocity = move * moveSpeed;
        moveVelocity.y = rb.velocity.y; // ���� �ӵ� ����

        // Rigidbody�� ����Ͽ� �̵� ó��
        rb.velocity = moveVelocity;

        // ���� �Է�
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void RotateCamera()
    {
        // ���콺 �Է��� ����Ͽ� ī�޶� ȸ����ŵ�ϴ�.
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = -Input.GetAxis("Mouse Y") * lookSpeed;

        // �÷��̾ ȸ����ŵ�ϴ�.
        transform.Rotate(0, mouseX, 0);

        // ī�޶� ���Ϸ� ȸ����ŵ�ϴ�.
        playerCamera.transform.Rotate(mouseY, 0, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Guard"))
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        }
    }
}
