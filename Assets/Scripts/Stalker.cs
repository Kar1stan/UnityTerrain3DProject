using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalker : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float moveVelocity;
    private AudioSource stalkerSound;
    private TMPro.TextMeshProUGUI coinCount;

    private static Stalker self ;
    private static int _coins;
    public static int coins { 
        get => _coins; 
        set
        {
            _coins = value;
            self.UpdateCoins();
        }
    }

    public Camera Cam;
    public GameObject CamPivot;

    private Vector3 cameraRod;
    private Vector3 cameraAngles;
    private Vector2 cameraSensitivity = new Vector2(4, 2);

    private Animator animator;
    const int STATE_IDLE = 0;
    const int STATE_WALK = 1;
    const int STATE_RUN  = 2;
    const int STATE_SIDE_WALK  = 4;

    const int BASE_VELOCITY = 400;

    public void UpdateCoins()
    {
        coinCount.text = coins.ToString();
    }
    void Start()
    {
        self = this;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        CamPivot = GameObject.Find("CamPivot");
        stalkerSound = GameObject.Find("Stalker").GetComponent<AudioSource>();
        moveVelocity = BASE_VELOCITY;
        cameraRod = CamPivot.transform.position - Cam.transform.position;
        cameraAngles = this.transform.eulerAngles;

        coinCount = GameObject.Find("CoinCount").GetComponent<TMPro.TextMeshProUGUI>();
        coins = 10;
    }


    void Update()
    {
        /*moveDirection.Set(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical"));
        */
        if (GameState.IsSoundEffects)
        {
            stalkerSound.Play();
        }
        Vector3 camFwd = Cam.transform.forward;
        camFwd.y = 0;
        camFwd = camFwd.normalized;

        float hor = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        moveDirection = camFwd * vert + Cam.transform.right * hor;

        if (Input.GetKeyDown(KeyCode.LeftShift)) moveVelocity *= 2;
        if(Input.GetKeyUp(KeyCode.LeftShift)) moveVelocity /= 2;
        
        // � ����������� ���� ����������: ������ �������� ��� timeScale=0
        //  ����������� - �������� ���� �� timeScale
        cameraAngles.x -= Input.GetAxis("Mouse Y") * cameraSensitivity.y * Time.timeScale;
        cameraAngles.y += Input.GetAxis("Mouse X") * cameraSensitivity.x * Time.timeScale;
        

        if (moveDirection != Vector3.zero) this.transform.forward = camFwd;
        characterController.SimpleMove(
            moveDirection * moveVelocity * Time.deltaTime);

        if (characterController.velocity.magnitude == 0)
        {
            animator.SetInteger("State", STATE_IDLE);
        }
        else
        {
            if (moveVelocity > BASE_VELOCITY)
            {
                animator.SetInteger("State", STATE_RUN);
            }
            else
            {
                if (Mathf.Abs(hor) > Mathf.Abs(vert))
                    animator.SetInteger("State", STATE_SIDE_WALK);
                else
                    animator.SetInteger("State", STATE_WALK);
            }
        }
        // if (Input.GetKeyDown(KeyCode.Space))                   //  < 3 - WALK
        //     Debug.Log(characterController.velocity.magnitude); // >= 3 - RUN
    }

    private void LateUpdate()
    {
        Vector2 mouseWheells = -Input.mouseScrollDelta;
        if (mouseWheells.y != 0)
        {
            if (mouseWheells.y < 0 && cameraRod.magnitude > 1 || mouseWheells.y > 0 && cameraRod.magnitude < 5)
            {
                cameraRod *= 1 + mouseWheells.y / 10;
            }
        }
        Cam.transform.position = CamPivot.transform.position - 
            Quaternion.Euler(0, cameraAngles.y, 0) * cameraRod;

        Cam.transform.eulerAngles = cameraAngles;
    }

    // ����������� ���� ���������� "isTrigger" � ������, ���������� � ����� �������� -
    // � ����� � � ������
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Stalker - Trigger");
    }

    // �� �����������, �.�. �� � ���� ��� ��.����
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Stalker - Collision");
    }
}
/* ��������
���������, ���� ��������� ��������:
 �������� - ��������� �������� �������, ���������� �� ��������
 �������� - ����.���� - ����� ������ � ��������� ����� ����
 ���������� �������� - "�������" ��������� (������������) ����� �������
 
�������� (������� �������� �������� ������):
 ������� ����� Animations (� Assets)
 � ����� ������� ����������, �������� Coin_Controller                |  Pulse
 � �������� Coin ��������� ��������� - Animator                      |  Coin(1)
 � ���� ��������� ������������ (�� �������) Coin_Controller          |
 ������� ������ �� Coin_Controller (� �������) ���������             |
   ���� ����������� (��������� ��������)                             |
 � ����� ������� Animation, �������� CoinRotate                      |  Pulse
 ������������ ���� ���� (CoinRotate) �� ��������� ��������           |
 ������� ������ �� CoinRotate ��������� �������� ����� (��������)    |
 �������� "������", ��������� �� �����, �������� Coin � ������� ��� �� ��� Y - 
   � ����� ���������� "����" � ��������� Coin:Rotation.
 �) �������� ���� ���� � ��������� �� ������ ������� �������, ������� ������������
     �������� �������� ��������� (���� ��������)
 �) ��������� ������� ������� �� ������ ������� � ����� �������� "������"
 � ����� Assets-Animations ���������� ���� (CoinRotate) � � ���������� �������
   ����������� ����������� ���������� (Loop Time)
-- ��������� - ������ ��������

������� �������� "���������" - ��������� ��������, �������� �� ������ �������

�������� �������� �������� ����:
 - ������� ����
 - �������� ��� � ��������� ��������
 - ����������� ��� ������ ��������� (� ������� �����������)
 - � ���� ������������� ������������ � ����� ���������
 */