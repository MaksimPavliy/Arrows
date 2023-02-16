using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isPlayerTurn = true;
    public static UnityAction OnRestart;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void Restart()
    {
        OnRestart?.Invoke();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(Time.timeScale == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
        }
        else if(Time.timeScale == 1 && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 0;
        }
    }
}
