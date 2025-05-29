using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private static int health = 10;
    private static int maxHealth = 10;
    private static float moveSpeed = 3f;
    private static float fireRate = 1f;
    private static float bulletSize = 2f;
    public static int Health { get => health; set => health = value; }
    public static int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public static float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public static float FireRate { get => fireRate; set => fireRate = value; }
    public static float BulletSize { get => bulletSize; set => bulletSize = value; }


    // private bool bootCollected = false;
    // private bool screwCollected = false;
    public List<string> collectedItems = new List<string>();
    private GameObject menu;
    private GameObject win;
    private GameObject winText;
    private bool isPaused = false;


    public HUD hud;
    public int Point { get { return point; } }
    // private bool redSyringeCollected = false;
    // private bool greenSyringeCollected = false;
    // private bool bigEyeSyringeCollected = false;
    public List<string> collectedNames = new List<string>();
    private int point;
    public int NumbBombs { get { return numbBombs; } }
    private int numbBombs = 2;

    void Start()
    {
        menu = GameObject.FindGameObjectWithTag("Menu");
        win = GameObject.FindGameObjectWithTag("Win");
        winText = GameObject.FindGameObjectWithTag("WinText");
        winText.SetActive(false);
        menu.SetActive(false);
        win.SetActive(false);

        hud.UpdateNumbBombs(numbBombs);
        IsaacController.instance.SetBombs(numbBombs);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && IsaacController.instance.isDeath == false)
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (menu == null) return;

        isPaused = !isPaused;

        menu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AddPoint(int value)
    {
        point += value;
        hud.UpdateScore(point);
    }

    public void UpdateLifeCount(int count)
    {
        health = count;
        hud.UpdateLife(health);
    }
    public void DecreaseLife(Vector2 position)
    {
        if (health > 0)
        {
            IsaacController.instance.Damage(position);
            health--;
            hud.UpdateLife(health);
            if (health == 0)
            {
                IsaacController.instance.Death();
                menu.SetActive(true);
            }
        }
    }

    public void DecreaseBombs()
    {
        if (numbBombs <= 0) return;
        numbBombs--;
        hud.UpdateNumbBombs(numbBombs);
        IsaacController.instance.SetBombs(numbBombs);
    }

    // public void UpdateCollectedItem(CollectionController item)
    // {
    //     collectedNames.Add(item.item.name);
    //     foreach (var i in item.item.effects)
    //     {
    //         switch (i)
    //         {
    //             case "red syringe":
    //                 redSyringeCollected = true;
    //                 break;
    //             case "green syringe":
    //                 greenSyringeCollected = true;
    //                 break;
    //             case "big eye":
    //                 bigEyeSyringeCollected = true;
    //                 break;
    //         }
    //     }
    //     if (redSyringeCollected && greenSyringeCollected && bigEyeSyringeCollected)
    //     {
    //         AddPoint(1000);
    //         redSyringeCollected = false;
    //         greenSyringeCollected = false;
    //         bigEyeSyringeCollected = false;
    //     }
    // }

    public void IncreaseBombs()
    {
        numbBombs++;
        hud.UpdateNumbBombs(numbBombs);
        IsaacController.instance.SetBombs(numbBombs);
    }

    public static void HealPlayer(int healthAmount)
    {
        health = Mathf.Min(maxHealth, health + healthAmount);
        HUD.instance.UpdateLife(health);
    }

    public static void MoveSpeedChange(float value)
    {
        moveSpeed += value;
        // hud.UpdateMoveSpeed(moveSpeed);
    }
    public static void FireRateChange(float value)
    {
        fireRate -= value;
        // hud.UpdateFireRate(fireRate);
    }
    public static void BulletSizeChange(float value)
    {
        bulletSize += value;
        // hud.UpdateBulletSize(bulletSize);
    }

    //Sinergias
    public void UpdateCollectedItems(CollectionController item)
    {
        collectedItems.Add(item.item.name);

        // foreach (var i in collectedItems)
        // {
        //     switch (i)
        //     {
        //         case "Boot":
        //             bootCollected = true;
        //             break;
        //         case "Screw":
        //             screwCollected = true;
        //             break;
        //     }
        // }

        // if (bootCollected && screwCollected)
        // {
        //     FireRateChange(0.25f);
        // }
        // hud.UpdateCollectedItems(collectedItems);
    }

    public void ResetState()
    {
        health = 10;
        maxHealth = 10;
        moveSpeed = 3f;
        fireRate = 1f;
        bulletSize = 2f;

        point = 0;
        numbBombs = 2;

        collectedItems.Clear();
        collectedNames.Clear();
        MusicManager.instance.Stop();
        isPaused = false;
        Time.timeScale = 1;
    }
    

    public void RestartGame()
    {
        ResetState();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void BackMenu()
    {
        MusicManager.instance.Stop();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void WinGame()
    {
        MusicManager.instance.VictoryMusic();
        win.SetActive(true);
        winText.SetActive(true);
    }
}
