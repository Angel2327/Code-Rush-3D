using UnityEngine;

public class Rotator : MonoBehaviour
{

    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // Eje Y

    void Update(){
        transform.Rotate(Vector3.up * 100f * Time.deltaTime, Space.World);
    }
 
}