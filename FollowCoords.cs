using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCoords : MonoBehaviour
{
    // Start is called before the first frame update
    public bool followX = true;
    public bool followY = false;
    public bool followZ = true;
    public float flatSpeed = 0;
    public float minRadius = 0;
    public float speed_multiplier = 1;
    void Start()
    {
        minRadius = GameManagement.Instance.map_scale * 4;
    }

    // Update is called once per frame
    public void Follow()
    {
        Point2D target_position2D = GameManagement.Instance.GetPlayerCoords(GameManagement.Instance.user_player);
        Vector3 target_position = new(target_position2D.x, 0, target_position2D.y);
        target_position *= GameManagement.Instance.map_scale;
        target_position.y = GameManagement.Instance.base_y;

        Vector3 move = transform.position;
        Vector3 displacement = target_position - move;
        Vector3 normalized_displacement = displacement.normalized;
        displacement -= normalized_displacement*minRadius;
        move += speed_multiplier * Time.fixedDeltaTime * (displacement + normalized_displacement*flatSpeed);
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

        // Calculate the rotation needed to point at the target
        Quaternion rotation = Quaternion.LookRotation(displacement);

        // Apply the rotation to the GameObject
        transform.rotation = rotation;

    }
}
