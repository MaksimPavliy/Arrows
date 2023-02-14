using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isPlayerTurn = true;

    private void Awake()
    {
        if(!instance && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
