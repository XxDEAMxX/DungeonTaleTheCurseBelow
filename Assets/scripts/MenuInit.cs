using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuInit : MonoBehaviour
{
    private GameObject tran;
    private Image tranImage;

    private void Start()
    {
        // Encuentra el panel por tag y obtiene el componente Image
        tran = GameObject.FindGameObjectWithTag("Tran");
        tran.SetActive(false);
        if (tran != null)
        {
            tranImage = tran.GetComponent<Image>();
        }
    }

    public void Play()
    {
        // Inicia la transición
        tran.SetActive(true);
        StartCoroutine(FadeToBlackAndLoadScene());
        GameManager.instance.ResetState();
    }

    public void Exit()
    {
        Debug.Log("Exit...");
        Application.Quit();
    }

    private IEnumerator FadeToBlackAndLoadScene()
    {
        float duration = 1f; // duración del fade
        float elapsed = 0f;

        Color color = tranImage.color;
        color.a = 0f;
        tranImage.color = color;

        // Asegúrate de que el panel esté activo
        tranImage.gameObject.SetActive(true);

        // Hacer el fade a negro
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            tranImage.color = color;
            yield return null;
        }

        // Esperar un pequeño tiempo si quieres
        yield return new WaitForSeconds(0.1f);

        // Cargar la siguiente escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
