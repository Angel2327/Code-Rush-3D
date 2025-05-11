using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Guardar()
    {
        string nivelActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("NivelGuardado", nivelActual);
        PlayerPrefs.Save();

        Debug.Log("Guardado nivel: " + nivelActual);

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

}
