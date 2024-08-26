using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb; // Reference to player's Rigidbody.
    private ConstantForce torque;
    public float torqueConstant = 100;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Access player's Rigidbody.
        torque = GetComponent<ConstantForce>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Handle physics-based movement and rotation.
    private void FixedUpdate()
    {
        // // Move player based on vertical input.
        // float moveVertical = Input.GetAxis("Vertical");
        // Vector3 movement = transform.forward * moveVertical * speed * Time.fixedDeltaTime;
        // rb.MovePosition(rb.position + movement);

        // // Rotate player based on horizontal input.
        // float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
        // Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        // rb.MoveRotation(rb.rotation * turnRotation);
        float verticalKey = Input.GetAxis("Vertical");
        float horizontalKey = Input.GetAxis("Horizontal");
        // float unknownKey = Input.GetAxis();
        Vector3 inputVector = new();
        inputVector.x = horizontalKey * torqueConstant;
        inputVector.z = verticalKey * torqueConstant;
        inputVector.y = 0;
        torque.torque = inputVector;

    }
}
