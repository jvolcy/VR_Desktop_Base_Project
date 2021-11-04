This is a starter project for developing VR apps while testing with desktop controls.  The Hierarchy contains the tow top-level objects "XR" and "FP Desktop".  For our purposes, the desktop "equivalent" the the VR app will be a first person controller.

To use this project, set the mode (VR or FP Desktop) you want to use.
For FP Desktop, disable the XR top level object and enable the FP Desktop object.  For XR, disable the FP Desktop object and enable the XR object.  You should never enable both.
All VR-specific objects should be a child of the "XR" top-level object.  All Desktop-specific objects should be a child of the "FP Desktop" object.  Objects that are common to both the desktop and the VR application should not be children of these top-level objects.
Both modes have a player.  If you write a script to control the player, you may want to attach that script to the player in both modes.  For XR, that would be the "XR Rig" child object of "XR".  For "FP Desktop", that would be the "PlayerCapsule" child object of "FP Desktop".

The two modes also have different Input Action assets.  This may be changed in the future into a single Input Action asset with two schemes.

Note that the project uses Unity's New Input System in both modes.

The project scene consists of 5 simple geometries to get you stated.  Delete these after you confirm that you see the scene the same way in both modes.