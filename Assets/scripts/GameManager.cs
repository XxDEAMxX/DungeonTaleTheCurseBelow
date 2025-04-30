using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public HUD hud;
    public int Point { get { return point; } }
    private int point;
    public int vidaCount = 3;
    public int NumbBombs { get { return numbBombs; } }
    private int numbBombs = 2;

    void Start()
    {
        hud.UpdateNumbBombs(numbBombs);
        IsaacController.instance.SetBombs(numbBombs);
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
        vidaCount = count;
        hud.UpdateLife(vidaCount);
    }
    public void DecreaseLife(Vector2 position)
    {
        if (vidaCount > 0)
        {   
            IsaacController.instance.Damage(position);
            vidaCount--;
            hud.UpdateLife(vidaCount);
            if (vidaCount == 0)
            {
                IsaacController.instance.Death();
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

    public void IncreaseBombs()
    {
        numbBombs++;
        hud.UpdateNumbBombs(numbBombs);
        IsaacController.instance.SetBombs(numbBombs);
    }
}
