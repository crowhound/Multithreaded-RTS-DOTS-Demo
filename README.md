# Most Recent Project Video Link
 The video at the following link shows the performance of about 500 moving units taking orders and doing navigation events with stable FPS and zero runtime memory allocation on the main thread.
 
[YouTube video link](https://www.youtube.com/watch?v=ng-i-Cb7iP0)

# Self-Learning Project
This is one of my projects I am doing to either learn something new, improve on something I already know how to do, or to try a different way to do something compared to how I currently am doing it.
The current progress report is at the bottom of the read me.

# Unity Version
Unity 6 LTS patch 28

# Game Play
Important Notice: This is a work in progress that I work on a little bit at a time.
Currently Units can be selected and told to move. If zombies and the soldiers get within a certain range the units will start to shoot.
I am currently working on the visuals for shooting, so no visuals for firing is implemented.
Enemy units do take damage and die though.


## Units
Soliders with red hats are selectable units. 
Zombies are the the enemy targetable units.

Selected Units have a green circle under their feet to provide visual feedback on which ones are currently selected.

## Input Controls
Multi select units - Left mouse button drag to create a visible box to select multiple units.
Single select units - Left mouse button click on a unit or if you left mouse button drag and the area of selection is small it will select only a certain unit.
Right click to move selected units.


# What am I trying to learn?
Currently I am trying to learn how to use Unity's most recently added features in the DOTS, Burst, and Mathematics package updates.
These features allow for in some cases over 100x increase in performance. Not a 100% increase, but a full 100 times increase.

This project is aimed at learning how to use the Unit Job system to properly handle multithreading jobs with the burst compiler to 
bring out the most performance possible with the smallest memory footprint.

## Learning Resources
Someone highly regarded in the Unity community made a seven hour learning course that goes over some of the stuff I wanted to learn more of. 
Going through the training video than adding my own features on top of it for more hands on expierence with the DOTS technology's new features.

## Current Progress
- Multithreading move commands for selected units are working
- Selected units have a visual indicator to show they are selected.
- Have Entity baker components to bake authoring data from normal gameobjects into a entity IComponentData to allow for the best performance possible.
- All units friendly or enemies have health and can die now.
- Soldiers shoot when zombies get within a certain range.


## Currently Working On Finishing
- Visuals for soldiers firing their weapons.
