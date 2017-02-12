The goal of this project is to show the complications VR has with a game like ping-pong. 
Collisions between the ball and racket are not detected timely when the racket moves faster than the required 90fps so we have to predict collisions and parent the ball to the racket in order to ensure the ball reacts with the rackets hit

PLAYING:
Load up the scene in: Assets/Scenes/temp.unity

To instantiate another ball after hitting a ball, press the right trigger (ball must be near your feet or lower)

SCRIPT:
Script for the collisions between the ball and racket can be found in: Assets/Scripts/BallManager.cs
