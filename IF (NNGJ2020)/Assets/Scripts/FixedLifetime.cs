using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FixedLifetime : MonoBehaviour
{
    public float LifeTime = 5;

    public ParticleSystem emit;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, LifeTime);
    }

    void OnCollisionEnter2D(Collision2D col) {
        DetachParticles();
        Destroy(gameObject);
    }

    void OnDestroy() {
        DetachParticles();
    }

    void DetachParticles ()    {
        if (emit != null) {
            emit.transform.parent = null;

            var e = emit.emission;
            
            e.enabled = false;

            Destroy(emit.gameObject, emit.main.startLifetime.constant);
        }
    }
}