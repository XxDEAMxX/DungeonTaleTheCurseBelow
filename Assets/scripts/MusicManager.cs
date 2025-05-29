using System;
using System.Collections;
using UnityEngine;


public enum MusicType { Boss, Main };
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Del()
    {
        yield return new WaitForSeconds(3f);
        restart();
        audioSource.Play();
    }

    public AudioSource audioSource;
    public AudioClip main;
    public AudioClip boss;
    public AudioClip victory;

    public static MusicType type;
    public static MusicType Type { get => type; set => type = value; }
    public bool isBossMusic = false;

    void Start()
    {
        if (!isBossMusic)
        {
            CambiarMusica(main);
        }
        else
        {
            CambiarMusica(boss);
        }
    }

    public void Init()
    {
        StartCoroutine(Del());
    }


    public void Stop()
    {
        audioSource.Stop();
    }

    void Update()
    {
        if (isBossMusic)
        {
            CambiarMusica(boss);
        }
        else
        {
            CambiarMusica(main);
        }
    }

    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (audioSource.clip == nuevaMusica) return;

        audioSource.clip = nuevaMusica;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void VictoryMusic()
    {
        CambiarMusica(victory);
    }

    public void restart()
    {
        if (!isBossMusic)
        {
            CambiarMusica(main);
        }
        else
        {
            CambiarMusica(boss);
        }
    }
}
