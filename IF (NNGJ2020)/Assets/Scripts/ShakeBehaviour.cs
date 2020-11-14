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
        initialPosition = transform.localPosition;
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
            transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake() {
        shakeDuration = 1.0f;
    }
}
