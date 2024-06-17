using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarSpecs", menuName = "CarSpecs", order = 1)]
public class CarSpecs : ScriptableObject
{
    public float dampingForce = 50;
    public float strength = 500;
    public float suspensionRestDist = 0.6f;
    public float hitDist = 0.4f;
    public float gripFactor = 0.4f;
    public float tireMass = 2f;
    
    public float totalSpeed = 20f;
    public float rotationSpeed = 10f;

    public float tireRotationAngle = 60f;
    public float speedValue = 300f;
}
