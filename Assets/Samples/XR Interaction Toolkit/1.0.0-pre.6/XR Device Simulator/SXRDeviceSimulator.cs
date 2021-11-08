using System;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;

/*
 * This is an XBox Controller-based simulator for the Oculus Quest 2 headset.  This code is
 * built from the base code provided with the 1.0.0-pre 6 XR Simulation Toolkit.
 * 
 * SpelmanXR, 2021
 * 
 * Notes
 * =====
 * 
 * XRSimulation Classes Documentation:
 * https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@0.10/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.html
 * 
 * InputSystem.XR Class Documentation:
 * https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.XR.html
 * 
 * Oculus Controls Input Mapping:
 * https://developer.oculus.com/documentation/unreal/unreal-controller-input-mapping-reference/
 * 
 * Oculus App Development in Unity
 * https://developer.oculus.com/documentation/unity/
 * 
 */


namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// A component which handles mouse and keyboard input from the user and uses it to
    /// drive simulated XR controllers and an XR head mounted display (HMD).
    /// </summary>
    /// <remarks>
    /// This class does not directly manipulate the camera or controllers which are part of
    /// the XR Rig, but rather drives them indirectly through simulated input devices.
    /// <br /><br />
    /// Use the Package Manager window to install the <i>XR Device Simulator</i> sample into
    /// your project to get sample mouse and keyboard bindings for Input System actions that
    /// this component expects. The sample also includes a prefab of a <see cref="GameObject"/>
    /// with this component attached that has references to those sample actions already set.
    /// To make use of this simulator, add the prefab to your scene (the prefab makes use
    /// of <see cref="InputActionManager"/> to ensure the Input System actions are enabled).
    /// <br /><br />
    /// Note that the XR Rig must read the position and rotation of the HMD and controllers
    /// by using Input System actions (such as by using <see cref="ActionBasedController"/>
    /// and <see cref="TrackedPoseDriver"/>) for this simulator to work as expected.
    /// Attempting to use XR input subsystem device methods (such as by using <see cref="XRController"/>
    /// and <see cref="SpatialTracking.TrackedPoseDriver"/>) will not work as expected
    /// since this simulator depends on the Input System to drive the simulated devices.
    /// </remarks>
    /// <seealso cref="XRSimulatedController"/>
    /// <seealso cref="XRSimulatedHMD"/>
    /// <seealso cref="SimulatedInputLayoutLoader"/>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_DeviceSimulator)]
    [HelpURL(XRHelpURLConstants.k_XRDeviceSimulator)]
    public partial class SXRDeviceSimulator : MonoBehaviour
    {
        //---------- Constants ----------
        const float DEFAULT_HEAD_X_SENSITIVITY = 1f;
        const float DEFAULT_HEAD_Y_SENSITIVITY = 1f;

        const float DEFAULT_HAND_X_SENSITIVITY = 0.5f;
        const float DEFAULT_HAND_Y_SENSITIVITY = 0.5f;

        const float DEFAULT_DISTANCE_BETWEEN_HANDS = 0.2f;

        //---------- Input Action Variables ----------
        bool m_Reset;               //keyboard ('V')
        Vector2 m_RightThumbstick;  //gampad/Xbox right joystick
        Vector2 m_LeftThumbstick;   //gampad/Xbox left joytick
        Vector2 m_HeadControl;      //gampad/Xbox D-pad
        float m_LeftGrip;           //gampad/Xbox left trigger
        float m_RightGrip;          //gampad/Xbox right trigger
        float m_LeftTrigger;        //gampad/Xbox left bumper/shoulder
        float m_RightTrigger;       //gampad/Xbox right bumper/shoulder
        bool m_ButtonA;             //gampad/Xbox Button A (south)
        bool m_ButtonB;             //gampad/Xbox Button B (east)
        bool m_ButtonX;             //gampad/Xbox Button X (west)
        bool m_ButtonY;             //gampad/Xbox Button Y (north)
        bool m_OculusButton;        //gampad/Xbox Select button
        bool m_MenuButton;          //gampad/Xbox Start/menu button


        //---------- Devices ----------
        XRSimulatedHMD m_HMDDevice;
        XRSimulatedController m_LeftControllerDevice;
        XRSimulatedController m_RightControllerDevice;

        //---------- Device States ----------
        XRSimulatedHMDState m_HMDState;
        XRSimulatedControllerState m_LeftControllerState;
        XRSimulatedControllerState m_RightControllerState;

        //---------- Device Euler Angles ----------
        Vector3 m_LeftControllerEuler;
        Vector3 m_RightControllerEuler;
        Vector3 m_CenterEyeEuler;

        //---------- Misc Members ----------
        Vector3 m_HeadStartingPosition;
        Vector3 m_LeftHandStartingPosition;
        Vector3 m_RightHandStartingPosition;


        //---------- Inspector Params ----------
        /*
        [SerializeField]
        [Tooltip("The Transform that contains the Camera. This is usually the \"Head\" of XR rigs. Automatically set to the first enabled camera tagged MainCamera if unset.")]
        Transform m_CameraTransform;
        /// <summary>
        /// The <see cref="Transform"/> that contains the <see cref="Camera"/>. This is usually the "Head" of XR rigs.
        /// Automatically set to <see cref="Camera.main"/> if unset.
        /// </summary>
        public Transform cameraTransform
        {
            get => m_CameraTransform;
            set => m_CameraTransform = value;
        }
        */

        [SerializeField]
        [Tooltip("Sensitivity of head rotation along the x-axis (pitch).")]
        float m_XHeadRotateSensitivity = DEFAULT_HEAD_X_SENSITIVITY;
        /// <summary>
        /// Sensitivity of rotation along the x-axis (pitch).
        /// </summary>
        /// <seealso cref="YHeadRotateSensitivity"/>
        public float XHeadRotateSensitivity
        {
            get => m_XHeadRotateSensitivity;
            set => m_XHeadRotateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("Sensitivity of head rotation along the y-axis (yaw).")]
        float m_YHeadRotateSensitivity = DEFAULT_HEAD_Y_SENSITIVITY;
        /// <summary>
        /// Sensitivity of rotation along the y-axis (yaw).
        /// </summary>
        /// <seealso cref="XHeadRotateSensitivity"/>
        public float YHeadRotateSensitivity
        {
            get => m_YHeadRotateSensitivity;
            set => m_YHeadRotateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("A boolean value of whether to invert the head y-axis." +
    "\nA false value (default) means typical FPS style where moving up/down pitches up/down." +
    "\nA true value means flight control style where moving up/down pitches down/up.")]
        bool m_InvertHeadY = false;
        /// <summary>
        /// A boolean value of whether to invert the y-axis of mouse input when rotating by mouse input.
        /// A <see langword="false"/> value (default) means typical FPS style where moving the mouse up/down pitches up/down.
        /// A <see langword="true"/> value means flight control style where moving the mouse up/down pitches down/up.
        /// </summary>
        public bool InvertHeadY
        {
            get => m_InvertHeadY;
            set => m_InvertHeadY = value;
        }


        [SerializeField]
        [Tooltip("Sensitivity of hand rotation along the x-axis (pitch).")]
        float m_XHandRotateSensitivity = DEFAULT_HAND_X_SENSITIVITY;
        /// <summary>
        /// Sensitivity of rotation along the x-axis (pitch).
        /// </summary>
        /// <seealso cref="YHandRotateSensitivity"/>
        public float XHandRotateSensitivity
        {
            get => m_XHandRotateSensitivity;
            set => m_XHandRotateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("Sensitivity of hand rotation along the y-axis (yaw).")]
        float m_YHandRotateSensitivity = DEFAULT_HAND_Y_SENSITIVITY;
        /// <summary>
        /// Sensitivity of rotation along the y-axis (yaw).
        /// </summary>
        /// <seealso cref="XHandRotateSensitivity"/>
        public float YHandRotateSensitivity
        {
            get => m_YHandRotateSensitivity;
            set => m_YHandRotateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("A boolean value of whether to invert the hadn y-axis." +
            "\nA false value (default) means typical FPS style where moving up/down pitches up/down." +
            "\nA true value means flight control style where moving up/down pitches down/up.")]
        bool m_InvertHandY = false;
        /// <summary>
        /// A boolean value of whether to invert the y-axis of mouse input when rotating by mouse input.
        /// A <see langword="false"/> value (default) means typical FPS style where moving the mouse up/down pitches up/down.
        /// A <see langword="true"/> value means flight control style where moving the mouse up/down pitches down/up.
        /// </summary>
        public bool InvertHandY
        {
            get => m_InvertHandY;
            set => m_InvertHandY = value;
        }


    }
}


//----------  ----------
