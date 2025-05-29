using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuInit : MonoBehaviour
{
    [SerializeField] private Image tranImage; // Asigna esto en el inspector

    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        // Si no está asignado por Inspector, intenta buscarla por tag
        if (tranImage == null)
        {
            GameObject tran = GameObject.FindGameObjectWithTag("Tran");
            if (tran != null)
            {
                tranImage = tran.GetComponent<Image>();
            }
        }

        if (tranImage != null)
        {
            // Asegura que arranca transparente
            Color color = tranImage.color;
            color.a = 0f;
            tranImage.color = color;
            tranImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("tranImage no asignado. Se cargará la escena sin transición.");
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Debug.Log("Exit...");
        Application.Quit();
    }

    private IEnumerator FadeToBlackAndLoadScene()
    {
        tranImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = tranImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            tranImage.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene(1);
    }
}
