using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour

{
[SerializeField] PlayerCameraMovement _playerCameraMovement;
[SerializeField] float launchForceValue = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag=="Player"){
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            rb.AddForce(other.transform.up * launchForceValue);
        }
    }
}
