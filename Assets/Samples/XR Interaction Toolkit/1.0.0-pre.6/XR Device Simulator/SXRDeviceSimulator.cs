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
        /// <summary>
        /// The coordinate space in which to operate.
        /// </summary>
        /// <seealso cref="keyboardTranslateSpace"/>
        /// <seealso cref="mouseTranslateSpace"/>
        public enum Space
        {
            /// <summary>
            /// Applies translations of a controller or HMD relative to its own coordinate space, considering its own rotations.
            /// Will translate a controller relative to itself, independent of the camera.
            /// </summary>
            Local,

            /// <summary>
            /// Applies translations of a controller or HMD relative to its parent. If the object does not have a parent, meaning
            /// it is a root object, the parent coordinate space is the same as the world coordinate space. This is the same
            /// as <see cref="Local"/> but without considering its own rotations.
            /// </summary>
            Parent,

            /// <summary>
            /// Applies translations of a controller or HMD relative to the screen.
            /// Will translate a controller relative to the camera, independent of the controller's orientation.
            /// </summary>
            Screen,
        }

        /// <summary>
        /// The transformation mode in which to operate.
        /// </summary>
        /// <seealso cref="mouseTransformationMode"/>
        public enum TransformationMode
        {
            /// <summary>
            /// Applies translations from input.
            /// </summary>
            Translate,

            /// <summary>
            /// Applies rotations from input.
            /// </summary>
            Rotate,
        }



        bool m_Reset;
        Vector2 m_RightThumbstick;
        Vector2 m_LeftThumbstick;
        Vector2 m_HeadControl;
        float m_LeftGrip;
        float m_RightGrip;
        float m_LeftTrigger;
        float m_RightTrigger;
        bool m_ButtonA;
        bool m_ButtonB;
        bool m_ButtonX;
        bool m_ButtonY;
        bool m_OculusButton;
        bool m_MenuButton;


        Vector3 m_LeftControllerEuler;
        Vector3 m_RightControllerEuler;
        Vector3 m_CenterEyeEuler;

        XRSimulatedHMDState m_HMDState;
        XRSimulatedControllerState m_LeftControllerState;
        XRSimulatedControllerState m_RightControllerState;

        XRSimulatedHMD m_HMDDevice;
        XRSimulatedController m_LeftControllerDevice;
        XRSimulatedController m_RightControllerDevice;




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


        [SerializeField]
        [Tooltip("The coordinate space in which mouse translation should operate.")]
        Space m_MouseTranslateSpace = Space.Screen;
        /// <summary>
        /// The coordinate space in which mouse translation should operate.
        /// </summary>
        /// <seealso cref="Space"/>
        /// <seealso cref="keyboardTranslateSpace"/>
        public Space mouseTranslateSpace
        {
            get => m_MouseTranslateSpace;
            set => m_MouseTranslateSpace = value;
        }


        [SerializeField]
        [Tooltip("Sensitivity of translation in the x-axis (left/right) when triggered by mouse input.")]
        float m_MouseXTranslateSensitivity = 0.0004f;
        /// <summary>
        /// Sensitivity of translation in the x-axis (left/right) when triggered by mouse input.
        /// </summary>
        /// <seealso cref="mouseDeltaAction"/>
        /// <seealso cref="mouseYTranslateSensitivity"/>
        /// <seealso cref="mouseScrollTranslateSensitivity"/>
        public float mouseXTranslateSensitivity
        {
            get => m_MouseXTranslateSensitivity;
            set => m_MouseXTranslateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("Sensitivity of translation in the y-axis (up/down) when triggered by mouse input.")]
        float m_MouseYTranslateSensitivity = 0.0004f;
        /// <summary>
        /// Sensitivity of translation in the y-axis (up/down) when triggered by mouse input.
        /// </summary>
        /// <seealso cref="mouseDeltaAction"/>
        /// <seealso cref="mouseXTranslateSensitivity"/>
        /// <seealso cref="mouseScrollTranslateSensitivity"/>
        public float mouseYTranslateSensitivity
        {
            get => m_MouseYTranslateSensitivity;
            set => m_MouseYTranslateSensitivity = value;
        }


        [SerializeField]
        [Tooltip("Sensitivity of rotation along the x-axis (pitch) when triggered by mouse input.")]
        float m_MouseXRotateSensitivity = 0.1f;
        /// <summary>
        /// Sensitivity of rotation along the x-axis (pitch) when triggered by mouse input.
        /// </summary>
        /// <seealso cref="mouseDeltaAction"/>
        /// <seealso cref="mouseYRotateSensitivity"/>
        /// <seealso cref="mouseScrollRotateSensitivity"/>
        public float mouseXRotateSensitivity
        {
            get => m_MouseXRotateSensitivity;
            set => m_MouseXRotateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("Sensitivity of rotation along the y-axis (yaw) when triggered by mouse input.")]
        float m_MouseYRotateSensitivity = 0.1f;
        /// <summary>
        /// Sensitivity of rotation along the y-axis (yaw) when triggered by mouse input.
        /// </summary>
        /// <seealso cref="mouseDeltaAction"/>
        /// <seealso cref="mouseXRotateSensitivity"/>
        /// <seealso cref="mouseScrollRotateSensitivity"/>
        public float mouseYRotateSensitivity
        {
            get => m_MouseYRotateSensitivity;
            set => m_MouseYRotateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("Sensitivity of rotation along the z-axis (roll) when triggered by mouse scroll input.")]
        float m_MouseScrollRotateSensitivity = 0.05f;
        /// <summary>
        /// Sensitivity of rotation along the z-axis (roll) when triggered by mouse scroll input.
        /// </summary>
        /// <seealso cref="mouseScrollAction"/>
        /// <seealso cref="mouseXRotateSensitivity"/>
        /// <seealso cref="mouseYRotateSensitivity"/>
        public float mouseScrollRotateSensitivity
        {
            get => m_MouseScrollRotateSensitivity;
            set => m_MouseScrollRotateSensitivity = value;
        }

        [SerializeField]
        [Tooltip("A boolean value of whether to invert the y-axis of mouse input when rotating by mouse input." +
            "\nA false value (default) means typical FPS style where moving the mouse up/down pitches up/down." +
            "\nA true value means flight control style where moving the mouse up/down pitches down/up.")]
        bool m_MouseYRotateInvert;
        /// <summary>
        /// A boolean value of whether to invert the y-axis of mouse input when rotating by mouse input.
        /// A <see langword="false"/> value (default) means typical FPS style where moving the mouse up/down pitches up/down.
        /// A <see langword="true"/> value means flight control style where moving the mouse up/down pitches down/up.
        /// </summary>
        public bool mouseYRotateInvert
        {
            get => m_MouseYRotateInvert;
            set => m_MouseYRotateInvert = value;
        }





    }
}
