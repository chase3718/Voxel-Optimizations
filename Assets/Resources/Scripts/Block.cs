using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public float density;
    public float health;
    public float volume;
    public float armor;
    public float cost;
    public float horsePower;
    public float mass {
        get => (density * volume) * 1000;
    }
}