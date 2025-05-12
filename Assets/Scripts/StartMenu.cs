using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button cargarPartidaButton;
    public AudioClip sonidoBotonGeneral;
    public AudioClip sonidoBotonEspecial;
    public AudioSource musicaDeFondo;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

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
        DetenerMusicaDeFondo();
        audioSource.PlayOneShot(sonidoBotonEspecial);
        float delay = sonidoBotonEspecial.length;
        Invoke("CargarEscenaNuevaPartida", delay);
    }

    public void CargarPartida()
    {
        if (PlayerPrefs.HasKey("NivelGuardado"))
        {
            DetenerMusicaDeFondo();
            audioSource.PlayOneShot(sonidoBotonEspecial);
            float delay = sonidoBotonEspecial.length;
            Invoke("CargarEscenaCargarPartida", delay);
        }
        else
        {
            Debug.LogWarning("No hay partida guardada.");
        }
    }

    public void Salir()
    {
        DetenerMusicaDeFondo();
        audioSource.PlayOneShot(sonidoBotonGeneral);
        float delay = sonidoBotonGeneral.length;
        Invoke("SalirDelJuego", delay);
    }

    private void CargarEscenaNuevaPartida()
    {
        SceneManager.LoadScene(1);
    }

    private void CargarEscenaCargarPartida()
    {
        string nivelGuardado = PlayerPrefs.GetString("NivelGuardado");
        SceneManager.LoadScene(nivelGuardado);
    }

    private void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void DetenerMusicaDeFondo()
    {
        if (musicaDeFondo != null)
        {
            musicaDeFondo.Stop();
        }
    }
}
