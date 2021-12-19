using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SimpleCarBehaviour : MonoBehaviour
{
    public float xLocation;
    public float yLocation;
    public float velocity;
    public float acceleration;
    public CapsuleCollider2D bodyCollider;
    public BoxCollider2D frontSensor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.Accelerate();
        this.AvoidCollision();
        this.RandomizeAcc();
        this.Move();
    }

    // If velocity below limit, increase acceleration
    void Accelerate()
    {

    }

    // If car detected within collision distance, decrease acceleration adequatly
    void AvoidCollision()
    {
        
    }

    // With low probability, randomly decrease acceleration by small value
    void RandomizeAcc(int probability=10)
    {
        System.Random r = new System.Random();
        int rInt = r.Next(0, 100);
        if (rInt < probability)
        {
            // TODO: adjust this value
            this.acceleration -= 5;
        }
    }

    // Move the car
    void Move()
    {
        this.velocity += this.acceleration;
    }
}
