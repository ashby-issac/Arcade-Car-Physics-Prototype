Custom Car Physics Prototype

Implemented a raycast based approach rather than using built-in wheel colliders. In this case there is a transform that represents each tire and appropriate forces are added to each tire's axis. The suspension is added to Y-Axis, Acceleration is added to Z-Axis, and for Steering/Slipping we take into account the X-Axis.

GameplayController is the main script that controls most of the gameplay related mechanics like the Inputs, Raycast processing for each tire, Scene reloading, UI updation.

CarSystem implements the SuspensionForce, SteeringForce, and the AccelerationForce implementations.

All the Car specification data such as the data required for Car's physics system is included inside the CarSpecs Scriptable Object.

Timer System implements the timer functionality for updating the timer when reaching at a specific checkpoint. Timer Data is implemented within the TimerSystem.


