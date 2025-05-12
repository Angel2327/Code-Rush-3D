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

        floatHeight = transform.position.y;

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
        Vector3 moveDir = transform.forward * -movementY;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        float currentHeight = transform.position.y;
        float difference = floatHeight - currentHeight;
        float verticalVelocity = rb.linearVelocity.y;
        float springForce = difference * floatForce - verticalVelocity * damping;
        rb.AddForce(Vector3.up * springForce);

        float rotationAmount = movementX * rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, rotationAmount * 2, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;

            // ← Sonido al recolectar
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

            Invoke("NextLevel", 1f);
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

            Invoke("Lose", 1.5f);  
        }
    }

    void Lose()
    {
        Camera.main.transform.SetParent(null);
        Destroy(gameObject);

        Time.timeScale = 0f;
    }
}
