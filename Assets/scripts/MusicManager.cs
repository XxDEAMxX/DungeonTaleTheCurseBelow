using System;
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
    public AudioSource audioSource;
    public AudioClip musica1;
    public AudioClip musica2;
 
    public static MusicType type;
    public static MusicType Type { get => type; set => type = value; }
    public bool isBossMusic = false;

    void Start()
    {

        if (!isBossMusic)
        {
            CambiarMusica(musica1);
        }
        else
        {
            CambiarMusica(musica2);
        }
    }

    void Update()
    {
        if (isBossMusic)
        {
            CambiarMusica(musica2);
        }
        else
        {
            CambiarMusica(musica1);
        }
    }

    public void CambiarMusica(AudioClip nuevaMusica)
    {
        if (audioSource.clip == nuevaMusica) return;

        audioSource.Stop();
        audioSource.clip = nuevaMusica;
        audioSource.loop = true;
        audioSource.Play();
    }
}
