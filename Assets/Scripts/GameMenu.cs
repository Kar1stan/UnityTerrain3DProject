using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Управление игровым меню
public class GameMenu : MonoBehaviour
{                                // GameMenu - Canvas, остается всегда активный (чтобы скрипт работал)
    public GameObject Content;   // Content - Контейнер с элементами меню (скрываемый/отображаемый)
                                 // все элементы меню внутри контейнера Content
    // для того чтобы меню можно было включить из других скриптов мы
    // а) создаем статическое свойство, отслеживающее изменение
    // б) сохраняем статическую ссылку на объект (self)
    private static GameMenu self;
    private static bool _IsShown;
    private static bool IsShown
    {
        get => _IsShown;
        set
        {
            _IsShown = value;
            self.UpdateState();
        }
    }
    
    void Start()
    {
        self = this;
        IsShown = Content.activeInHierarchy;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsShown = !IsShown;
        }
    }

    public void UpdateState()
    {
        Content.SetActive(IsShown);  // Отобразить / скрыть контент меню
        Time.timeScale = IsShown ? 0.0f : 1.0f;  // Остановка/пуск времени
        
    }

    // Обработчики элементов меню
    public void OnDoneButton()
    {
        IsShown = false;
    }

    public void OnToggleSoundEffects(bool isOn)
    {
        Debug.Log(isOn);
    }
    
}
/* Д.З. Обеспечить переключение звуковых эффектов (через меню), проверить в игре
 * Добавить игровую музыку (фоновую, постоянную, повторяющ.), добавить в меню
 * элемент управления громкостью этой музыки. При активном меню ставить музыку
 * на паузу.
 */
