using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;

    private float movementX;
    private float movementY;
    public float speed = 5f;
    public float rotationSpeed = 1000f;
    private float floatHeight;
    public float floatForce = 1f;
    public float damping = 2f;

    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        // Establecer altura flotante con la posición inicial Y
        floatHeight = transform.position.y;

        count = 0;
        SetCountText();
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
        countText.text = "Points: " + count.ToString();

        if (count >= 36)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Camera.main.transform.SetParent(null);
            Destroy(gameObject);
            winTextObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "Has Perdido!";
        }
    }
}
