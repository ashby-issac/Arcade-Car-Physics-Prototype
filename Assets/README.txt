Custom Car Physics Prototype

------------------------------
Implemented a arcade car control prototype using a raycast based approach for the physics simulation. 
In this case there is a transform that represents each tire and appropriate forces including suspension force (y), acceleration force (z) and steering force (x) are added to the rigidbody at the point of each tire.

GameplayController (Monobehaviour) is the main script that controls most of the gameplay related mechanics such as input for the car, raycast processing for each tire, Scene reloading, UI updation.

CarSystem implements the SuspensionForce, SteeringForce, and the AccelerationForce implementations for controlling the Car and based on the inputs from the GameplayController the different forces would be applied.

Timer System class holds the timer updating behaviour when reaching at a specific checkpoint.
The core game loop is controlled by the timer and the car has to reach the destination within the specified time. Each checkpoint adds a small duration to the timer. 

Used a scriptable object for defining the Car's specs that is required for defining the car's physics.

Scriptable Object Data Representation
-------------------------------------
The suspension can be thought of as a spring which is at rest and compresses. The strenth parameter is a constant that can be used to mofify the strength of the spring compression (strength = 500). The dampingForce can be used to counteract the force applied on the suspension by multiplying with the velocity. (Code inside CarSystem's SuspensionForce)

The hitDist is used to check if the ray has hit the floor and if it has we apply force towards the suspension, steering.
GripFactor is used for determining the grip for the tires. It basically acts as a traction. Set the gripFactor to 1 gives more grip.
Steering force is to make sure the velocity is counteracted in a way that the slippery behaviour would be avoided and using the gripFactor we can reduce the grip and so we can decide on how much of the velocity needs to be counteracted. Could enhance this feature through an animation curve.
The mass is specified so that in order to counteract the slippery force we can use the formula (f = ma) to counteract the force which actually applied force in opposite direction using the acceleration and mass. (Code inside CarSystem's SteeringForce)

For the acceleration force, implemented a simple logic and used a constant accelSpeed which determines the speed for the rigidbody. Can be enhanced further. (Code inside CarSystem's AccelerationForce)






