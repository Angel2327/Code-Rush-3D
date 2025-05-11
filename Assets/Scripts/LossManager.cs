using UnityEngine;
using UnityEngine.SceneManagement;

public class LossManager : MonoBehaviour
{
    public void ReintentarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
