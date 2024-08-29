using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect2 : MonoBehaviour
{
    // private Transform pos; // Reference to player's Rigidbody.
    public float revolutionsPerSecond = 1;
    public float heightConstant = 0.5f;
    public float hoveringSpeedConstant = 200;
    public float dampening = 1;
    public bool local_rotation = false;
    private int phase;
    private float initial_y;

    // Start is called before the first frame update
    private void Start()
    {
        // pos = GetComponent<Transform>(); // Access the object's Transform Component.
        phase = Time.frameCount;
        initial_y = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Hover.
        Vector3 movement = transform.position;
        double hoverMovement = Math.Sin(hoveringSpeedConstant/10000/dampening* Time.frameCount + phase)*heightConstant;
        movement.y = (float)hoverMovement + initial_y;
        transform.position = movement;
        //Rotation.
        // This method works:
        // float turn = revolutionsPerSecond/dampening;
        // Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        // transform.rotation *= turnRotation;
        // Simpler method:
        if (local_rotation)
        {
            transform.Rotate(0, revolutionsPerSecond*7.2f/dampening, 0);    
        }
        else
        {
            transform.Rotate(Vector3.up, revolutionsPerSecond * 7.2f/dampening, Space.World);
        }
        
        

    }
}
