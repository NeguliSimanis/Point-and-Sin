using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region CURRENT STATE
    bool isAlive = true;
    bool canPauseGame = true;
    #endregion

    #region MOVEMENT
    private Vector2 targetPosition;
    private Vector2 dirNormalized;
    #endregion

    #region COMPONENTS
    Rigidbody2D rigidBody2D;
    #endregion

    #region UI
    [SerializeField] Image healthBar;
    [SerializeField] GameObject defeatPanel;
    #endregion

    private void Awake()
    {
        LoadPlayerData();
    } 

    void LoadPlayerData()
    {
        if (PlayerData.current == null)
            PlayerData.current = new PlayerData();
        PlayerData.current.isGamePaused = false;
    }

    void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateHealthBar();
        ListenForGamePause();
        ListenForPlayerDefeat();
        if (!isAlive)
            Die();
        if (PlayerData.current.isGamePaused)
        {
            Debug.Log("gam is fuck paue");
            return;
        }
        #region MOVEMENT
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("gsdade");
            GetTargetPositionAndDirection();
        }
        if (Vector2.Distance(targetPosition, transform.position) <= 0.01f)
        {
            return; 
        }
        else
        {
            MovePlayer(); 
        }
        #endregion
    }

    void Die()
    {
        canPauseGame = false;
        PlayerData.current.isGamePaused = true;
        defeatPanel.SetActive(true);    
    }

    void ListenForPlayerDefeat()
    {
        if (PlayerData.current.currentLife == 0)
            isAlive = false;
    }

    void ListenForGamePause()
    {
        if (Input.GetKeyDown(KeyCode.P) && canPauseGame)
        {
            PlayerData.current.isGamePaused = !PlayerData.current.isGamePaused;
        }
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = (PlayerData.current.currentLife * 1f) / PlayerData.current.maxLife;
    }

    void GetTargetPositionAndDirection()
    {
        targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
        dirNormalized = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        dirNormalized = dirNormalized.normalized;
    }

    void MovePlayer()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y) + dirNormalized * PlayerData.current.moveSpeed * Time.deltaTime;
    }
}
