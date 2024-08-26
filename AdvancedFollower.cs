using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedFollower : MonoBehaviour
{
    // Start is called before the first frame update
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;
    public float flatSpeed = 0;
    public float minRadius = 0;
    public GameObject target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 move = transform.position;
        Vector3 targetPosition = target.transform.position;
        Vector3 displacement = targetPosition - move;
        Vector3 normalizedDisplacement = displacement.normalized;
        displacement -= normalizedDisplacement*minRadius;
        move += (displacement + normalizedDisplacement*flatSpeed)*Time.fixedDeltaTime;
        if (!followX)
        {
            move.x = transform.position.x;
        }
        if (!followY)
        {
            move.y = transform.position.y;
        }
        if (!followZ)
        {
            move.z = transform.position.z;
        }
        transform.position = move;


    }
}
