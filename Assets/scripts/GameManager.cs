using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public HUD hud;
    public int Point { get { return point; } }
    private int point;
    public int vidaCount = 8;

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
            // Movement.instance.GetDamage(position);
            vidaCount--;
            hud.UpdateLife(vidaCount);
            if (vidaCount == 0)
            {
                Movement.instance.Death();
            }
        }
    }
}
