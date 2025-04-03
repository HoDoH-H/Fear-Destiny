using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementPattern
{
    public List<MP> patterns;
}

[System.Serializable]
public class MP
{
    public bool directionOnly;
    public Vector2 movement;
    public float timeBeforePattern;
}
