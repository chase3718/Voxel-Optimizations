using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    DockManager manager;
    Color original;

    private void Awake() {
        manager = GameObject.FindObjectOfType<DockManager>();
        GetComponent<Collider>().enabled = false;
        GetComponent<Collider>().enabled = true;
        original = transform.GetComponent<Renderer>().material.GetColor("_Color");
    }

    private void Update() {
        if (transform.tag != "ShipPart") {
            if (manager.CheckIntersects()) {
                this.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            } else {
                this.transform.GetComponent<Renderer>().material.SetColor("_Color", original);
            }
        }
    }

    private void OnMouseEnter()
    {
        if (manager.mode == DockManager.Mode.delete)
        {
            print("Hello World");
            original = transform.GetComponent<Renderer>().material.GetColor("_Color");
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Renderer>() != null)
                {
                    child.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                }
            }
        }
    }
    private void OnMouseExit()
    {
        if (manager.mode == DockManager.Mode.delete)
        {
            print("Goodbye World");
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Renderer>() != null)
                {
                    child.GetComponent<Renderer>().material.SetColor("_Color", original);
                }
            }
        }
    }
}