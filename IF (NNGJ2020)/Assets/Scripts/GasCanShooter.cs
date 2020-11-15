using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCanShooter : MonoBehaviour
{
    public float fireInterval = 2;

    public Rigidbody2D projectile;

    public float projectileSpeed;

    private float timeToEmit;

    private

    // Start is called before the first frame update
    void Awake()
    {
        timeToEmit = fireInterval;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeToEmit -= Time.fixedDeltaTime;

        if (timeToEmit <= 0) {
            Rigidbody2D inst = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);

            inst.velocity = gameObject.transform.up * projectileSpeed;

            Debug.Log("fired using " + inst.velocity);
            Debug.Log("emitter at " + gameObject.transform.position);
            Debug.Log("object at " + inst.transform.position);

            timeToEmit = fireInterval;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;        

        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y),
            new Vector2(transform.position.x, transform.position.y) + (Vector2)gameObject.transform.up.normalized * (projectileSpeed / 5));
    }
}
