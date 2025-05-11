using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button cargarPartidaButton;

    void Start()
    {
        if (PlayerPrefs.HasKey("NivelGuardado"))
        {
            cargarPartidaButton.interactable = true;
        }
        else
        {
            cargarPartidaButton.interactable = false;
        }
    }

    public void NuevaPartida()
    {
        SceneManager.LoadScene(1);
    }

    public void CargarPartida()
    {
        if (PlayerPrefs.HasKey("NivelGuardado"))
        {
            string nivelGuardado = PlayerPrefs.GetString("NivelGuardado");
            Debug.Log("Cargando nivel guardado: " + nivelGuardado);
            SceneManager.LoadScene(nivelGuardado);
        }
        else
        {
            Debug.LogWarning("No hay partida guardada.");
        }
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
