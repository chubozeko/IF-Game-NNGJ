using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLifetime : MonoBehaviour
{
    public float LifeTime = 5;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, LifeTime);
    }
}
