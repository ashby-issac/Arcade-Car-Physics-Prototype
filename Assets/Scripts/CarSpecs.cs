using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarSpecs", menuName = "CarSpecs", order = 1)]
public class CarSpecs : ScriptableObject
{
    public float dampingForce;
    public float strength;
    public float suspensionRestDist = 0.5f;
    public float hitDist = 0.3f;
    public float gripFactor = 1f;
    public float tireMass = 1f;
    
    public float totalSpeed = 10f;
    public float AccelSpeed = 50f;
    public float rotationSpeed = 10f;

    public float tireRotationAngle = 60f;
}
