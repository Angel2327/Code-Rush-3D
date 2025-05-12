using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    private int totalPickups;

    private float movementX;
    private float movementY;
    public float speed = 5f;
    public float rotationSpeed = 1000f;

    private float floatHeight;
    public float floatForce = 1f;
    public float damping = 2f;

    public float floatAmplitude = 0.2f; // Qué tanto sube y baja
    public float floatFrequency = 1f;   // Qué tan rápido sube y baja
    private float baseHeight;
    private float floatTimer;

    public TextMeshProUGUI countText;
    public TextMeshProUGUI levelText;
    public GameObject winTextObject;
    public GameObject LossPanel;

    public AudioClip pickupSound;
    public AudioClip deathSound;
    public AudioClip victorySound;
    private AudioSource audioSource;
    private AudioSource backgroundMusic;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        baseHeight = transform.position.y;
        floatHeight = baseHeight;

        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;
        count = 0;
        SetCountText();
        SetLevelText();
        winTextObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();

        GameObject musicObj = GameObject.Find("BackgroundMusic");
        if (musicObj != null)
        {
            backgroundMusic = musicObj.GetComponent<AudioSource>();
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        // Actualizamos el floatHeight con una onda sinusoidal
        floatTimer += Time.fixedDeltaTime;
        floatHeight = baseHeight + Mathf.Sin(floatTimer * floatFrequency * Mathf.PI * 2f) * floatAmplitude;

        // Movimiento hacia adelante/atrás
        Vector3 moveDir = transform.forward * -movementY;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        // Aplicar fuerza para mantener la altura flotante oscilante
        float currentHeight = transform.position.y;
        float difference = floatHeight - currentHeight;
        float verticalVelocity = rb.linearVelocity.y;
        float springForce = difference * floatForce - verticalVelocity * damping;
        rb.AddForce(Vector3.up * springForce);

        // Rotación sobre el eje Y
        float rotationAmount = movementX * rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, rotationAmount * 2, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;

            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Points: " + count.ToString() + " / " + totalPickups.ToString();

        if (count >= totalPickups)
        {
            if (backgroundMusic != null)
                backgroundMusic.Stop();

            if (audioSource != null && victorySound != null)
                audioSource.PlayOneShot(victorySound);

            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));

            Invoke("NextLevel", 3f);
        }
    }

    void SetLevelText()
    {
        int baseLevelIndex = 1;
        int relativeIndex = SceneManager.GetActiveScene().buildIndex - baseLevelIndex;
        int world = (relativeIndex / 3) + 1;
        int stage = (relativeIndex % 3) + 1;

        levelText.text = "Nivel " + world + "-" + stage;
    }

    void NextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Has completado todos los niveles.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));

            if (backgroundMusic != null)
                backgroundMusic.Stop();

            audioSource.PlayOneShot(deathSound);
            LossPanel.SetActive(true);
            Time.timeScale = 0f;
            Invoke("Lose", 2f);
        }
    }

    void Lose()
    {
        Camera.main.transform.SetParent(null);
        Destroy(gameObject);
    }
}
