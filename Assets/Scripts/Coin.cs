using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    private const float COIN_TIME = 10;

    private GameObject CoinPrefab;  // ����������� �� ��������
    private TMPro.TextMeshProUGUI coinDistanceValue;
    private Image coinDistanceBar;  // ��������� ���������� �� ������
    private Image countdown;        // ������ ��������� ������� - ������� ������� �������
    private GameObject compassArrow;
    private Animator animator;
    private GameObject stalker;
    private AudioSource coinSound;  // ���� ����� ������
    private float timeleft;         // ������� ������� �� ������������ ������
    

    void Start()
    {
        animator = GetComponent<Animator>();
        stalker = GameObject.Find("Stalker");
        coinDistanceValue = GameObject.Find("CoinDistanceValue").GetComponent<TMPro.TextMeshProUGUI>();
        compassArrow = GameObject.Find("CompassArrow");
        coinDistanceBar = GameObject.Find("CoinDistanceBar").GetComponent<Image>();
        coinSound = GetComponent<AudioSource>();
        countdown = GameObject.Find("CountDown").GetComponent<Image>();
        timeleft = COIN_TIME;
    }

    private void Update()
    {
        if (timeleft > 0)
        {
            float p = timeleft / COIN_TIME;
            countdown.fillAmount = p;
            countdown.color = new Color((1 - p), p, p);
            timeleft -= Time.deltaTime;
        }
        else
        {
            Stalker.coins--;
            OnDisappearEnd();
        }
    }

    void LateUpdate()
    {
        if(animator.GetInteger("ClipNumber") != 2)
        if((this.transform.position - stalker.transform.position).magnitude < 3 )
        {
            animator.SetInteger("ClipNumber", 1);
        }
        else
        {
            animator.SetInteger("ClipNumber", 0);
        }

        float dis = (this.transform.position - stalker.transform.position).magnitude;
        coinDistanceValue.text = // ���������� �� ������ �� ���������
            (Mathf.Round(dis * 10) / 10f).ToString("0.0");
        coinDistanceBar.fillAmount = dis / 20;

        /* Compas   / Camera          ������� ���������� ��
         *         / ����             ���� �� �������
         *      S  ------ Coin        ����������: Vector3.SignedAngle(from, to, axis)
         */
        Vector3 c_s = this.transform.position - stalker.transform.position;  // S  ------ Coin
        Vector3 c_f = Camera.main.transform.forward;  // --- Camera
        c_s.y = c_f.y = 0;  // �������� �������� �� ��������� �����
        float ang = Vector3.SignedAngle(c_s, c_f, Vector3.up);  // ���� ����� ���������
        // coinAngleValue.text = Mathf.Round(ang).ToString();
        compassArrow.transform.localEulerAngles = new Vector3(0, 0, ang);  // ������� ������� �������

    }

    // ����������� � � �������, � ���.
    // ����� ����� ������ �� ����� ����, ������� �������� �� ��������� ������
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Coin - Trigger");
        animator.SetInteger("ClipNumber", 2);  // ������ ����� ������������

        if(GameState.IsSoundEffects)  // ���� ������������ ������ ��������, ��
            coinSound.Play();         // ������������ �����

        Stalker.coins++;
    }

    // �� ����������� - ��� Rigidbody
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Coin - Collision");
    }

    // �������, ���������� ���������� �� ��������� ����� ������������
    public void OnDisappearEnd()
    {
        // ������� ������ � ������� (��������)
        CoinPrefab = Resources.Load<GameObject>("Prefabs/Coin");

        // ������� ����� �� �������
        var newCoin = GameObject.Instantiate(CoinPrefab);

        // ��������� ����� �������
        Vector3 pos;
        do                                                           // ���������� ����� �������
        {                                                            // 
            pos = this.transform.position                            // 
                + Vector3.right * Random.Range(-20f, 20f)            // 20 - ������������ ����������
                + Vector3.forward * Random.Range(-20f, 20f);         // 
                                                                     // 
        } while ((stalker.transform.position - pos).magnitude < 10   // 10 - ����������� ����������
                || pos.x < 20 || pos.z < 20 );                       // 20 - ������� ��� (�����)

        // ������ ����� - ��������� ������ ������ � ����� �������
        pos.y = Terrain.activeTerrain.SampleHeight(pos) + 1.5f;  // TODO: 1.5f -> ������ ��� ������ ���������� ������

        newCoin.transform.position = pos;

        // ������� ������ ������
        GameObject.Destroy(this.transform.parent.gameObject);
    }
}
/*   [---10.2---]
 *   [---6.0    ]
 *   [   0.1    ]
 *   �.�. ���������� ������� ��������� ����� � ��� �����������
 */

/* �������: ����������� ������ ������������ ������. ����� ���������
 * ������� ������ ���������� � ����� �����.
 * ����� ���� ��������� �����: �������� � 10, ���� ������ �� ������ �����, ��
 *  ���-�� �����������, ���� ������ - �������������.
 */

/* �������: ���������� ���� � �������� - ����� �������� � ����������
 * �������: �������� ������� ������, ���������� �� �������������� �����
 *   ���� ��� �������, �� ������������� �����, � ������� �������� ������
 *   �� ��������. �� ������ ���������� �������� �) �������� �) ���������
 *   ��� �����. * ���� �������� ������ ����������
 *   �������� ���� ������ �������.
 * �������: �����������/��������� ������ ���������� ������ ����
 */