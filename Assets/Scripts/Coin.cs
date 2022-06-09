using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    private const float COIN_TIME = 10;

    private GameObject CoinPrefab;  // загружается из ресурсов
    private TMPro.TextMeshProUGUI coinDistanceValue;
    private Image coinDistanceBar;  // индикатор расстояния до монеты
    private Image countdown;        // таймер обратного отсчета - рисунок остатка времени
    private GameObject compassArrow;
    private Animator animator;
    private GameObject stalker;
    private AudioSource coinSound;  // звук сбора монеты
    private float timeleft;         // остаток времени до исчезновения монеты
    

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
        coinDistanceValue.text = // расстояние от монеты до персонажа
            (Mathf.Round(dis * 10) / 10f).ToString("0.0");
        coinDistanceBar.fillAmount = dis / 20;

        /* Compas   / Camera          Вывести информацию об
         *         / угол             угле на Дисплей
         *      S  ------ Coin        Инструмент: Vector3.SignedAngle(from, to, axis)
         */
        Vector3 c_s = this.transform.position - stalker.transform.position;  // S  ------ Coin
        Vector3 c_f = Camera.main.transform.forward;  // --- Camera
        c_s.y = c_f.y = 0;  // проекции векторов на плоскость Земли
        float ang = Vector3.SignedAngle(c_s, c_f, Vector3.up);  // угол между векторами
        // coinAngleValue.text = Mathf.Round(ang).ToString();
        compassArrow.transform.localEulerAngles = new Vector3(0, 0, ang);  // рисунок стрелки компаса

    }

    // срабатывает и в Сталкер, и тут.
    // Здесь пишем только ту часть кода, которая отвечает за поведение монеты
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Coin - Trigger");
        animator.SetInteger("ClipNumber", 2);  // запуск клипа исчезновения

        if(GameState.IsSoundEffects)  // если проигрывание звуков включено, то
            coinSound.Play();         // проигрывание звука

        Stalker.coins++;
    }

    // не срабатывает - нет Rigidbody
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Coin - Collision");
    }

    // событие, вызываемое Аниматором по окончанию клипа исчезновения
    public void OnDisappearEnd()
    {
        // Находим префаб в ассетах (ресурсах)
        CoinPrefab = Resources.Load<GameObject>("Prefabs/Coin");

        // создаем новую из префаба
        var newCoin = GameObject.Instantiate(CoinPrefab);

        // подбираем новую позицию
        Vector3 pos;
        do                                                           // Генерируем новую позицию
        {                                                            // 
            pos = this.transform.position                            // 
                + Vector3.right * Random.Range(-20f, 20f)            // 20 - максимальное расстояние
                + Vector3.forward * Random.Range(-20f, 20f);         // 
                                                                     // 
        } while ((stalker.transform.position - pos).magnitude < 10   // 10 - минимальное расстояние
                || pos.x < 20 || pos.z < 20 );                       // 20 - граница гор (карты)

        // Высота Земли - коррекция высоты монеты в новой позиции
        pos.y = Terrain.activeTerrain.SampleHeight(pos) + 1.5f;  // TODO: 1.5f -> высота над землей предыдущей монеты

        newCoin.transform.position = pos;

        // удаляем данную монету
        GameObject.Destroy(this.transform.parent.gameObject);
    }
}
/*   [---10.2---]
 *   [---6.0    ]
 *   [   0.1    ]
 *   Д.З. Обеспечить подсчет собранных монет и его отображение
 */

/* Задание: реализовать таймер исчезновения монеты. После истечения
 * времени монета появляется в новом месте.
 * Ведем счет собранных монет: начинаем с 10, если монету не успели взять, то
 *  кол-во уменьшается, если успели - увеличивается.
 */

/* Задание: установить небо с облаками - найти картинку и подключить
 * Задание: добавить игровой объект, отвечающий за дополнительное время
 *   если его собрать, то увеличивается время, в течение которого монета
 *   не исчезает. На объект установить анимации а) ожидание б) исчезание
 *   при сборе. * Срок действия бонуса ограничить
 *   Добавить звук взятия объекта.
 * Задание: приближение/отдаление камеры прокруткой колеса мыши
 */