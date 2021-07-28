# AforgeTest

This was a 'play' project of mine to try out object detection and tracking. The aforge framework and phidgets .Net 2.1 installer are included in the project if needed for your local. This code assumes a few things like a USB webcam and an 8/8/8 phidgets controller with 2 servos in a pan & tilt configuration. If you're not sure what these things are, here are a few links:

### Links

* [AForge](http://aforgenet.com/)
* [Phidgets](https://www.phidgets.com/)
* [Micorsoft Lifecam](https://www.amazon.com/Microsoft-64L-00003-LifeCam-VX-1000/dp/B000GE9XQ2)
* [Trossen Robotics - pan & tilt kits](https://www.trossenrobotics.com/c/robot-turrets.aspx)

### Operation

Assuming all the hardware is setup and the code correctly configured, you should be able to run this. In the form, drag to select an area which contains the object to track. The code is simplistic, assuming a prevailing color exists and it uses that for object tracking. Rudimentary actually. But the structure is there to add complexity. 

![FormWindow](/aforgetestform.PNG)

### Finally

It was a fun project I played with around mid-2010. I'm uploading more for posterity and for others to use as a basis for thier builds, more learning that anthing else. I tried to keep things simple with discrete function calls for ease of reusability in other projects.

If you're interested in robotics, I recommend getting started at the [Trossen Robotics Community](http://forums.trossenrobotics.com/content.php)