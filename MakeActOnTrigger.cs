using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeActOnTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public float jumpSpeedConstant = 2f;
    public GameObject particleOnTrigger;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 aboveTriggerer = new();
        float sizeTriggerer = other.gameObject.transform.lossyScale.y;  //lossyScale or localScale?
        aboveTriggerer = other.gameObject.transform.position;
        aboveTriggerer.y += 2*sizeTriggerer;
        other.gameObject.transform.RotateAround(aboveTriggerer, Vector3.forward, 180);
        
        Instantiate(particleOnTrigger, transform.position, Quaternion.identity);
        Destroy(gameObject);

    }
}
