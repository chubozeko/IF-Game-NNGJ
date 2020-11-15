using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCar : MonoBehaviour
{
    public GameObject spawnLocation;

    public Rigidbody2D vehicle;

    public Vector2 spawnVelocity;

    public bool flipX;

    public bool flipY;

    public bool triggerOnce = true;

    private bool didSpawn = false;

    private List<Rigidbody2D> spawnedCars = new List<Rigidbody2D>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.CompareTag("Player")) {
            if ((triggerOnce && !didSpawn) || !triggerOnce) {
                if (triggerOnce) didSpawn = true;

                Rigidbody2D theCar = Instantiate(vehicle, spawnLocation.transform.position, spawnLocation.transform.rotation);

                if (flipX) theCar.gameObject.transform.localScale = new Vector2(-theCar.gameObject.transform.localScale.x, theCar.gameObject.transform.localScale.y);
                if (flipY) theCar.gameObject.transform.localScale = new Vector2(theCar.gameObject.transform.localScale.x, -theCar.gameObject.transform.localScale.y);

                spawnedCars.Add(theCar);
            }
        }
    }

    void FixedUpdate() {
        foreach (var car in spawnedCars) {
            car.MovePosition((Vector2)car.transform.position + new Vector2(spawnVelocity.x, spawnVelocity.y));
        }
    }
}
