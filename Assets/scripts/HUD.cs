using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bombText;
    public GameObject[] vida;

    void Start()
    {
    }

    void Update()
    {
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateLife(int life)
    {
        for (int i = 0; i < vida.Length; i++)
        {
            vida[i].SetActive(i < life);
        }
    }

    public void UpdateNumbBombs(int numBombs)
    {
        bombText.text = numBombs.ToString();
    }
}
