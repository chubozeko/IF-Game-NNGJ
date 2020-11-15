using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    Player _player;
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            _player = other.gameObject.GetComponent<Player>();
            _player.lastCheckPoint = this.transform;
        }
    }   

}
