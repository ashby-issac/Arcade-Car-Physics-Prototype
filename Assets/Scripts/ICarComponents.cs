using UnityEngine;

public interface ICarComponents
{
    void SuspensionForce(float distance = 0, Transform tireTransform = null);
    void SteeringForce(Transform tireTransform = null);
    void AccelerationForce(float accelInput = 0, Transform tireTransform = null);
}