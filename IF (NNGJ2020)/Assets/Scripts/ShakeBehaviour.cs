using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBehaviour : MonoBehaviour
{
    private Transform t;

    private float shakeDuration = 0f;

    public float shakeMagnitude = 0.7f;

    public float dampingSpeed = 1.0f;

    Vector3 initialPosition;

    void Awake()
    {
        if (t == null) 
            t = GetComponent<Transform>();
    }

    void OnEnable()
    {
        // initialPosition = transform.localPosition;
        initialPosition = new Vector3(
            gameObject.GetComponent<PlayerCameraMovement>().cameraTarget.position.x,
            gameObject.GetComponent<PlayerCameraMovement>().cameraTarget.position.y,
            transform.position.z);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeDuration > 0) {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        } else {
            shakeDuration = 0f;
            
            initialPosition = new Vector3(
            gameObject.GetComponent<PlayerCameraMovement>().cameraTarget.position.x,
            gameObject.GetComponent<PlayerCameraMovement>().cameraTarget.position.y,
            transform.position.z);
            transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake() {
        shakeDuration = 1.0f;
    }
}
