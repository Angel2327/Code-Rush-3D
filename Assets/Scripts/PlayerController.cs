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
    public TextMeshProUGUI levelText;  // Nueva variable para mostrar el nivel
    public GameObject winTextObject;
    public GameObject LossPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        // Establecer altura flotante con la posición inicial Y
        floatHeight = transform.position.y;

        // Contar la cantidad total de pickups en la escena
        totalPickups = GameObject.FindGameObjectsWithTag("PickUp").Length;

        count = 0;
        SetCountText();
        SetLevelText();  // Llamamos a la función para mostrar el nivel
        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        // Movimiento solo hacia adelante o atrás (W/S)
        Vector3 moveDir = transform.forward * -movementY;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        // Flotar a altura constante
        float currentHeight = transform.position.y;
        float difference = floatHeight - currentHeight;
        float verticalVelocity = rb.linearVelocity.y;
        float springForce = difference * floatForce - verticalVelocity * damping;
        rb.AddForce(Vector3.up * springForce);

        // Rotar en eje Y con A/D (izquierda/derecha)
        float rotationAmount = movementX * rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, rotationAmount * 2, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Points: " + count.ToString() + " / " + totalPickups.ToString();

        if (count >= totalPickups)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));

            // Cargar el siguiente nivel después de 2 segundos
            Invoke("NextLevel", 2f);
        }
    }

    // NUEVA FUNCIÓN: mostrar el nivel en formato "Nivel X-Y"
    void SetLevelText()
    {
        int baseLevelIndex = 1; // Ajusta esto si tus niveles empiezan en otra posición
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
            Camera.main.transform.SetParent(null);
            Destroy(gameObject);

            // Mostrar panel de derrota
            LossPanel.SetActive(true);

            // Opcional: Pausar el tiempo si quieres congelar el juego
            Time.timeScale = 0f;
        }
    }
}
