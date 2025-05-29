using System;
using System.Collections;
using UnityEngine;


public enum MusicType { Boss, Main };
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // <-- Esto mantiene el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public AudioSource audioSource;
    public AudioClip main;
    public AudioClip boss;
    public AudioClip victory;
    public static MusicType type;
    public static MusicType Type { get => type; set => type = value; }
    private AudioClip currentClip = null;
    void Start()
    {
    }
    public void Init()
    {
        restart();
        audioSource.Play();
    }
    public void Stop()
    {
        audioSource.Stop();
    }
    public void SetBossMusic()
    {
        CambiarMusica(boss);
    }
    public void SetMainMusic()
    {
        CambiarMusica(main);
    }
    public void VictoryMusic()
    {
        CambiarMusica(victory);
    }
    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (audioSource.clip == nuevaMusica) return;
        audioSource.Stop();
        audioSource.clip = nuevaMusica;
        audioSource.loop = true;
        audioSource.Play();
        currentClip = nuevaMusica;
        Debug.Log(currentClip);
    }
    public void restart()
    {
        SetMainMusic();
    }
}
