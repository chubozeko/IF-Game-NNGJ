using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour

{
[SerializeField] PlayerCameraMovement _playerCameraMovement;
[SerializeField] float launchForceValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2 (0, launchForceValue));
        }
    }
}
