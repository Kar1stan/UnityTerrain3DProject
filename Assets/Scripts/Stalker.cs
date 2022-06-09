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
        
        // с добавлением меню обнаружено: камера крутится при timeScale=0
        //  модификация - умножаем вход на timeScale
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

    // срабатывает если установить "isTrigger" у монеты, вызывается в обоих скриптах -
    // и здесь и у монеты
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Stalker - Trigger");
    }

    // не срабатывает, т.к. ни у кого нет тв.тела
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Stalker - Collision");
    }
}
/* Анимации
Аккуратно, есть несколько терминов:
 Аниматор - компонент игрового объекта, отвечающий за анимацию
 Анимация - аним.клип - набор кадров и переходов между ними
 Контроллер анимации - "автомат" переходов (переключений) между клипами
 
Действия (создаем анимацию вращения монеты):
 Создаем папку Animations (в Assets)
 В папке создаем контроллер, называем Coin_Controller                |  Pulse
 У элемента Coin добавляем компонент - Animator                      |  Coin(1)
 В этот компонент перетягиваем (из ассетов) Coin_Controller          |
 Двойной щелчок по Coin_Controller (в ассетах) открывает             |
   окно контроллера (диаграмму автомата)                             |
 В папке создаем Animation, называем CoinRotate                      |  Pulse
 Перетягиваем этот клип (CoinRotate) на диаграмму автомата           |
 Двойной щелчок по CoinRotate открывает редактор клипа (анимацию)    |
 Нажимаем "запись", переходим на сцену, выбираем Coin и вращаем его по оси Y - 
   в клипе появляется "кадр" с атрибутом Coin:Rotation.
 а) Копируем этот кадр и вставляем на другие позиции времени, вручную корректируем
     числовые значения атрибутов (угла вращения)
 б) Переносим бегунок времени на другую позицию и снова нажимаем "запись"
 В папке Assets-Animations фокусируем клип (CoinRotate) и в инспекторе свойств
   устанавлием циклическое повторение (Loop Time)
-- Проверяем - должно работать

Сделать анимацию "пульсации" - изменение масштаба, испытать на другом объекте

Добавить анимацию бокового бега:
 - создать клип
 - добавить его в конроллер анимаций
 - реализовать все группы переходов (с другими состояниями)
 - в коде предусмотреть переключение в новое состояние
 */