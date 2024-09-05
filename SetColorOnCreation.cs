using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorOnCreation : MonoBehaviour
{
    public float r = 0;
    public float g = 0;
    public float b = 0;
    public float a = 0;
    private Renderer found_renderer;

    // Start is called before the first frame update
    void Start()
    {
        found_renderer = GetComponent<Renderer>();
        found_renderer.material.color = new Color(r/255,g/255,b/255,a/255);
    }
}