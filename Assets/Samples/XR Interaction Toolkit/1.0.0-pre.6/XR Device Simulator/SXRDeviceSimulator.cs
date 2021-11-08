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


        /*
        [SerializeField]
        [Tooltip("tool_tip_description")]
        InputActionReference m_XXX;
        public InputActionReference XXX
        {
            get => m_XXX;
            set
            {
                UnsubscribeXXX();
                m_XXX = value;
                SubscribeXXX();
            }
        }
        */


        
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
        

        /// <summary>
        /// The transformation mode in which the mouse should operate.
        /// </summary>
        public TransformationMode mouseTransformationMode { get; set; } = TransformationMode.Rotate;


        //test
        public float stuff;

        /*
        Vector2 m_ManipulateLeftHandInput;
        Vector2 m_ManipulateRightHandInput;
        Vector2 m_ManipulateHeadInput;

        Vector2 m_MouseDeltaInput;

        bool m_RotateModeOverrideInput;
        bool m_NegateModeInput;
        */
        bool m_XConstraintInput;
        bool m_YConstraintInput;
        bool m_ZConstraintInput;
        /*
        bool m_ResetInput;


        bool m_GripInput;
        bool m_TriggerInput;
        bool m_PrimaryButtonInput;
        bool m_MenuInput;
        bool m_Primary2DAxisClickInput;
        */

        Vector3 m_LeftControllerEuler;
        Vector3 m_RightControllerEuler;
        Vector3 m_CenterEyeEuler;

        XRSimulatedHMDState m_HMDState;
        XRSimulatedControllerState m_LeftControllerState;
        XRSimulatedControllerState m_RightControllerState;

        XRSimulatedHMD m_HMDDevice;
        XRSimulatedController m_LeftControllerDevice;
        XRSimulatedController m_RightControllerDevice;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            m_HMDState.Reset();
            m_LeftControllerState.Reset();
            m_RightControllerState.Reset();

            m_LeftControllerState.devicePosition += new Vector3(-0.1f, 0f, 0f);
            m_RightControllerState.devicePosition += new Vector3(0.1f, 0f, 0f);

        }


        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            // Find the Camera if necessary
            if (m_CameraTransform == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                    m_CameraTransform = mainCamera.transform;
            }

            AddDevices();


            SubscribeResetAction();
            SubscribeRightThumbstickAction();
            SubscribeLeftThumbstickAction();
            SubscribeHeadControlAction();
            SubscribeLeftGripAction();
            SubscribeRightGripAction();
            SubscribeLeftTriggerAction();
            SubscribeRightTriggerAction();
            SubscribeButtonAAction();
            SubscribeButtonBAction();
            SubscribeButtonXAction();
            SubscribeButtonYAction();
            SubscribeOculusButtonAction();
            SubscribeMenuButtonAction();

        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            RemoveDevices();

            UnsubscribeResetAction();
            UnsubscribeRightThumbstickAction();
            UnsubscribeLeftThumbstickAction();
            UnsubscribeHeadControlAction();
            UnsubscribeLeftGripAction();
            UnsubscribeRightGripAction();
            UnsubscribeLeftTriggerAction();
            UnsubscribeRightTriggerAction();
            UnsubscribeButtonAAction();
            UnsubscribeButtonBAction();
            UnsubscribeButtonXAction();
            UnsubscribeButtonYAction();
            UnsubscribeOculusButtonAction();
            UnsubscribeMenuButtonAction();
 
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
            ProcessPoseInput();
            ProcessControlInput();

            if (m_HMDDevice != null)
            {
                InputState.Change(m_HMDDevice, m_HMDState);
            }

            if (m_LeftControllerDevice != null)
            {
                InputState.Change(m_LeftControllerDevice, m_LeftControllerState);
            }

            if (m_RightControllerDevice != null)
            {
                InputState.Change(m_RightControllerDevice, m_RightControllerState);
            }
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated device(s)
        /// related to position and rotation.
        /// </summary>
        protected virtual void ProcessPoseInput()
        {
            // Set tracked states
            m_LeftControllerState.isTracked = true;
            m_RightControllerState.isTracked = true;
            m_HMDState.isTracked = true;
            m_LeftControllerState.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);
            m_RightControllerState.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);
            m_HMDState.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);

            /* jv
            if (!m_ManipulateLeftInput && !m_ManipulateRightInput && !m_ManipulateHeadInput)
                return;
            */
            if (m_CameraTransform == null)
            {
                Debug.Log("m_CameraTransform is null.");
                return;
            }

            var cameraParent = m_CameraTransform.parent;
            var cameraParentRotation = cameraParent != null ? cameraParent.rotation : Quaternion.identity;
            var inverseCameraParentRotation = Quaternion.Inverse(cameraParentRotation);

 
        /* for now, ignore translation.  Only the head needs to translate.  the left and right controls will only rotate
        if ((mouseTransformationMode == TransformationMode.Translate && !m_RotateModeOverrideInput && !m_NegateModeInput) ||
            (mouseTransformationMode == TransformationMode.Rotate || m_RotateModeOverrideInput) && m_NegateModeInput)*/

        if (mouseTransformationMode == TransformationMode.Translate)
        {
            // Determine frame of reference
            GetAxes(m_MouseTranslateSpace, m_CameraTransform, out var right, out var up, out var forward);

            // Mouse translation
            /*
            var scaledMouseDeltaInput =
                new Vector3(m_MouseDeltaInput.x * m_MouseXTranslateSensitivity,
                    m_MouseDeltaInput.y * m_MouseYTranslateSensitivity,
                    m_MouseScrollInput.y * m_MouseScrollTranslateSensitivity);
            */
                var scaledManipulateLeftHandInput =
                    new Vector3(m_LeftThumbstick.x * m_MouseXTranslateSensitivity,
                        m_LeftThumbstick.y * m_MouseYTranslateSensitivity,
                        0);

                var scaledManipulateRightHandInput =
                    new Vector3(m_RightThumbstick.x * m_MouseXTranslateSensitivity,
                        m_RightThumbstick.y * m_MouseYTranslateSensitivity,
                        0);

                var scaledManipulateHeadInput =
                    new Vector3(m_HeadControl.x * m_MouseXTranslateSensitivity,
                        0,
                        m_HeadControl.y * m_MouseYTranslateSensitivity
                        );


                Vector3 deltaLeftHand, deltaRightHand, deltaHead;
            if (m_XConstraintInput && !m_YConstraintInput && m_ZConstraintInput) // XZ
            {
                    /*       deltaPosition =
                              right * scaledMouseDeltaInput.x +
                              forward * scaledMouseDeltaInput.y; */

                    deltaLeftHand =
                        right * scaledManipulateLeftHandInput.x +
                        forward * scaledManipulateLeftHandInput.y;

                    deltaRightHand =
                        right * scaledManipulateRightHandInput.x +
                        forward * scaledManipulateRightHandInput.y;

                    deltaHead =
                        right * scaledManipulateHeadInput.x +
                        forward * scaledManipulateHeadInput.y;
                }
            else if (!m_XConstraintInput && m_YConstraintInput && m_ZConstraintInput) // YZ
            {
                    deltaLeftHand = up * scaledManipulateLeftHandInput.y + forward * scaledManipulateLeftHandInput.x;
                    deltaRightHand = up * scaledManipulateRightHandInput.y + forward * scaledManipulateRightHandInput.x;
                    deltaHead = up * scaledManipulateHeadInput.y + forward * scaledManipulateHeadInput.x;

                }
                else if (m_XConstraintInput && !m_YConstraintInput && !m_ZConstraintInput) // X
            {
                    deltaLeftHand = right * (scaledManipulateLeftHandInput.x + scaledManipulateLeftHandInput.y);
                    deltaRightHand = right * (scaledManipulateRightHandInput.x + scaledManipulateRightHandInput.y);
                    deltaHead = right * (scaledManipulateHeadInput.x + scaledManipulateHeadInput.y);
                }
                else if (!m_XConstraintInput && m_YConstraintInput && !m_ZConstraintInput) // Y
            {
                    deltaLeftHand = up * (scaledManipulateLeftHandInput.x + scaledManipulateLeftHandInput.y);
                    deltaRightHand = up * (scaledManipulateRightHandInput.x + scaledManipulateRightHandInput.y);
                    deltaHead = up * (scaledManipulateHeadInput.x + scaledManipulateHeadInput.y);
                }
                else if (!m_XConstraintInput && !m_YConstraintInput && m_ZConstraintInput) // Z
            {
                    deltaLeftHand = forward * (scaledManipulateLeftHandInput.x + scaledManipulateLeftHandInput.y);
                    deltaRightHand = forward * (scaledManipulateRightHandInput.x + scaledManipulateRightHandInput.y);
                    deltaHead = forward * (scaledManipulateHeadInput.x + scaledManipulateHeadInput.y);
                }
                else
            {
                    deltaLeftHand = right * scaledManipulateLeftHandInput.x + up * scaledManipulateLeftHandInput.y;
                    deltaRightHand = right * scaledManipulateRightHandInput.x + up * scaledManipulateRightHandInput.y;
                    deltaHead = right * scaledManipulateHeadInput.x + up * scaledManipulateHeadInput.y;
                }

                // Scroll contribution
                //jv deltaPosition += forward * scaledMouseDeltaInput.z;

            //if (m_ManipulateLeftInput)
            //{
                var deltaRotationL = GetDeltaRotation(m_MouseTranslateSpace, m_LeftControllerState, inverseCameraParentRotation);
                m_LeftControllerState.devicePosition += deltaRotationL * deltaLeftHand;
            //}

            //if (m_ManipulateRightInput)
            //{
                var deltaRotationR = GetDeltaRotation(m_MouseTranslateSpace, m_RightControllerState, inverseCameraParentRotation);
                m_RightControllerState.devicePosition += deltaRotationR * deltaRightHand;
            //}

            //if (m_ManipulateHeadInput)
            //{
                var deltaRotationH = GetDeltaRotation(m_MouseTranslateSpace, m_HMDState, inverseCameraParentRotation);
                m_HMDState.centerEyePosition += deltaRotationH * deltaHead;
                m_HMDState.devicePosition = m_HMDState.centerEyePosition;
            //}

            // Reset
            if (m_Reset)
            {
                var resetScale = GetResetScale();

                //if (m_ManipulateLeftInput)
                //{
                    var devicePositionL = Vector3.Scale(m_LeftControllerState.devicePosition, resetScale);
                    // The active control for the InputAction will be null while the Action is in waiting at (0, 0, 0)
                    // so use a small value to reset the position to near origin.
                    if (devicePositionL.magnitude <= 0f)
                        devicePositionL = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);

                    m_LeftControllerState.devicePosition = devicePositionL;
                //}

                //if (m_ManipulateRightInput)
                //{
                    var devicePositionR = Vector3.Scale(m_RightControllerState.devicePosition, resetScale);
                    // The active control for the InputAction will be null while the Action is in waiting at (0, 0, 0)
                    // so use a small value to reset the position to near origin.
                    if (devicePositionR.magnitude <= 0f)
                        devicePositionR = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);

                    m_RightControllerState.devicePosition = devicePositionR;
                //}

                //if (m_ManipulateHeadInput)
                //{
                    // TODO: Tracked Pose Driver (New Input System) has a bug where it only subscribes to
                    // performed and not canceled, so the Transform will not be updated until the magnitude
                    // is considered actuated to trigger a performed event. As a workaround, set to
                    // a small value (enough to be considered actuated) instead of Vector3.zero.
                    var centerEyePosition = Vector3.Scale(m_HMDState.centerEyePosition, resetScale);
                    if (centerEyePosition.magnitude <= 0f)
                        centerEyePosition = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);

                    m_HMDState.centerEyePosition = centerEyePosition;
                    m_HMDState.devicePosition = m_HMDState.centerEyePosition;
                //}
            }
        }
            else
            {
            // Mouse rotation
            var scaledManipulateLeftHandInput =
                    new Vector3(m_LeftThumbstick.x * m_MouseXRotateSensitivity,
                        m_LeftThumbstick.y * m_MouseYRotateSensitivity * (m_MouseYRotateInvert ? 1f : -1f),
                        0);

                var scaledManipulateRightHandInput =
                    new Vector3(m_RightThumbstick.x * m_MouseXRotateSensitivity,
                        m_RightThumbstick.y * m_MouseYRotateSensitivity * (m_MouseYRotateInvert ? 1f : -1f),
                        0);

                var scaledManipulateHeadInput =
                    new Vector3(m_HeadControl.x * m_MouseXRotateSensitivity,
                        m_HeadControl.y * m_MouseYRotateSensitivity * (m_MouseYRotateInvert ? 1f : -1f),
                        0);

            //Debug.Log("right = " + m_ManipulateRightHandInput);

            /*
            var scaledMouseDeltaInput =
                new Vector3(m_MouseDeltaInput.x * m_MouseXRotateSensitivity,
                    m_MouseDeltaInput.y * m_MouseYRotateSensitivity * (m_MouseYRotateInvert ? 1f : -1f),
                    m_MouseScrollInput.y * m_MouseScrollRotateSensitivity);
            */
            Vector3 anglesLeft, anglesRight, anglesHead;
                if (m_XConstraintInput && !m_YConstraintInput && m_ZConstraintInput) // XZ
                {
                    anglesLeft = new Vector3(scaledManipulateLeftHandInput.y, 0f, -scaledManipulateLeftHandInput.x);
                    anglesRight = new Vector3(scaledManipulateRightHandInput.y, 0f, -scaledManipulateRightHandInput.x);
                    anglesHead = new Vector3(scaledManipulateHeadInput.y, 0f, -scaledManipulateHeadInput.x);
                }
                else if (!m_XConstraintInput && m_YConstraintInput && m_ZConstraintInput) // YZ
                {
                    anglesLeft = new Vector3(0f, scaledManipulateLeftHandInput.x, -scaledManipulateLeftHandInput.y);
                    anglesRight = new Vector3(0f, scaledManipulateRightHandInput.x, -scaledManipulateRightHandInput.y);
                    anglesHead = new Vector3(0f, scaledManipulateHeadInput.x, -scaledManipulateHeadInput.y);
                }
                else if (m_XConstraintInput && !m_YConstraintInput && !m_ZConstraintInput) // X
                {
                    anglesLeft = new Vector3(-scaledManipulateLeftHandInput.x + scaledManipulateLeftHandInput.y, 0f, 0f);
                    anglesRight = new Vector3(-scaledManipulateRightHandInput.x + scaledManipulateRightHandInput.y, 0f, 0f);
                    anglesHead = new Vector3(-scaledManipulateHeadInput.x + scaledManipulateHeadInput.y, 0f, 0f);
                }
                else if (!m_XConstraintInput && m_YConstraintInput && !m_ZConstraintInput) // Y
                {
                    anglesLeft = new Vector3(0f, scaledManipulateLeftHandInput.x + -scaledManipulateLeftHandInput.y, 0f);
                    anglesRight = new Vector3(0f, scaledManipulateRightHandInput.x + -scaledManipulateRightHandInput.y, 0f);
                    anglesHead = new Vector3(0f, scaledManipulateHeadInput.x + -scaledManipulateHeadInput.y, 0f);
                }
                else if (!m_XConstraintInput && !m_YConstraintInput && m_ZConstraintInput) // Z
                {
                    anglesLeft = new Vector3(0f, 0f, -scaledManipulateLeftHandInput.x + -scaledManipulateLeftHandInput.y);
                    anglesRight = new Vector3(0f, 0f, -scaledManipulateRightHandInput.x + -scaledManipulateRightHandInput.y);
                    anglesHead = new Vector3(0f, 0f, -scaledManipulateHeadInput.x + -scaledManipulateHeadInput.y);
                }
                else
                {
                    anglesLeft = new Vector3(scaledManipulateLeftHandInput.y, scaledManipulateLeftHandInput.x, 0f);
                    anglesRight = new Vector3(scaledManipulateRightHandInput.y, scaledManipulateRightHandInput.x, 0f);
                    anglesHead = new Vector3(scaledManipulateHeadInput.y, scaledManipulateHeadInput.x, 0f);
                }
                
                // Scroll contribution
                //jv anglesDelta += new Vector3(0f, 0f, scaledMouseDeltaInput.z);

                //jv if (m_ManipulateLeftInput)
                //jv {
                    m_LeftControllerEuler += anglesLeft;
                    m_LeftControllerState.deviceRotation = Quaternion.Euler(m_LeftControllerEuler);
                //jv }

                //jv if (m_ManipulateRightInput)
                //jv {
                    m_RightControllerEuler += anglesRight;
                    m_RightControllerState.deviceRotation = Quaternion.Euler(m_RightControllerEuler);
                //jv }

                //jv if (m_ManipulateHeadInput)
                //jv {
                    m_CenterEyeEuler += anglesHead;
                    m_HMDState.centerEyeRotation = Quaternion.Euler(m_CenterEyeEuler);
                //jv }

            // Reset
    
            if (m_Reset)
                {
                Debug.Log("Reset!");
                    var resetScale = GetResetScale();

                    //jv if (m_ManipulateLeftInput)
                    //jv {
                        m_LeftControllerEuler = Vector3.Scale(m_LeftControllerEuler, resetScale);
                        m_LeftControllerState.deviceRotation = Quaternion.Euler(m_LeftControllerEuler);
                    //jv }

                    //jv if (m_ManipulateRightInput)
                    //jv {
                        m_RightControllerEuler = Vector3.Scale(m_RightControllerEuler, resetScale);
                        m_RightControllerState.deviceRotation = Quaternion.Euler(m_RightControllerEuler);
                    //jv }

                    //jv if (m_ManipulateHeadInput)
                    //jv {
                        m_CenterEyeEuler = Vector3.Scale(m_CenterEyeEuler, resetScale);
                        m_HMDState.centerEyeRotation = Quaternion.Euler(m_CenterEyeEuler);
                    //jv }
                }

             }
            //Debug.Log("left = " + m_LeftControllerEuler);
                
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to input controls.
        /// </summary>
        protected virtual void ProcessControlInput()
        {
            ProcessAxis2DControlInput();

            //jv if (m_ManipulateLeftInput)
            //jv {
            ProcessLeftButtonControlInput(ref m_LeftControllerState);
            //jv }

            //jv if (m_ManipulateRightInput)
            //jv {
            ProcessRightButtonControlInput(ref m_RightControllerState);
            //jv }
        }



        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to 2D Axis input controls.
        /// </summary>
        protected virtual void ProcessAxis2DControlInput()
        {
            m_LeftControllerState.primary2DAxis = m_LeftThumbstick; // m_Axis2DInput;
            m_RightControllerState.primary2DAxis = m_RightThumbstick; // m_Axis2DInput;
        }
       


        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to button input controls.
        /// </summary>
        /// <param name="controllerState">The controller state that will be processed.</param>
        protected virtual void ProcessLeftButtonControlInput(ref XRSimulatedControllerState controllerState)
        {
            //controllerState.grip = m_GripInput ? 1f : 0f;
            //controllerState.WithButton(ControllerButton.GripButton, m_LeftGrip);
            //controllerState.trigger = m_TriggerInput ? 1f : 0f;
            //controllerState.WithButton(ControllerButton.TriggerButton, m_LeftTrigger);
            //controllerState.WithButton(ControllerButton.PrimaryButton, ???);
            controllerState.WithButton(ControllerButton.MenuButton, m_MenuButton);
            //controllerState.WithButton(ControllerButton.Primary2DAxisClick, ???);
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to button input controls.
        /// </summary>
        /// <param name="controllerState">The controller state that will be processed.</param>
        protected virtual void ProcessRightButtonControlInput(ref XRSimulatedControllerState controllerState)
        {
            //controllerState.grip = m_GripInput ? 1f : 0f;
            //controllerState.WithButton(ControllerButton.GripButton, m_RightGrip);
            //controllerState.trigger = m_TriggerInput ? 1f : 0f;
            //controllerState.WithButton(ControllerButton.TriggerButton, m_RightTrigger);
            ///controllerState.WithButton(ControllerButton.PrimaryButton, ???);
            controllerState.WithButton(ControllerButton.MenuButton, m_OculusButton);
            //controllerState.WithButton(ControllerButton.Primary2DAxisClick, ???);

        }


        /// <summary>
        /// Add simulated XR devices to the Input System.
        /// </summary>
        /// <seealso cref="InputSystem.AddDevice{TDevice}"/>
        protected virtual void AddDevices()
        {
            m_HMDDevice = InputSystem.InputSystem.AddDevice<XRSimulatedHMD>();
            if (m_HMDDevice == null)
            {
                Debug.LogError($"Failed to create {nameof(XRSimulatedHMD)}.");
            }

            m_LeftControllerDevice = InputSystem.InputSystem.AddDevice<XRSimulatedController>($"{nameof(XRSimulatedController)} - {InputSystem.CommonUsages.LeftHand}");
            if (m_LeftControllerDevice != null)
            {
                InputSystem.InputSystem.SetDeviceUsage(m_LeftControllerDevice, InputSystem.CommonUsages.LeftHand);
            }
            else
            {
                Debug.LogError($"Failed to create {nameof(XRSimulatedController)} for {InputSystem.CommonUsages.LeftHand}.", this);
            }

            m_RightControllerDevice = InputSystem.InputSystem.AddDevice<XRSimulatedController>($"{nameof(XRSimulatedController)} - {InputSystem.CommonUsages.RightHand}");
            if (m_RightControllerDevice != null)
            {
                InputSystem.InputSystem.SetDeviceUsage(m_RightControllerDevice, InputSystem.CommonUsages.RightHand);
            }
            else
            {
                Debug.LogError($"Failed to create {nameof(XRSimulatedController)} for {InputSystem.CommonUsages.RightHand}.", this);
            }
        }

        /// <summary>
        /// Remove simulated XR devices from the Input System.
        /// </summary>
        /// <seealso cref="InputSystem.RemoveDevice"/>
        protected virtual void RemoveDevices()
        {
            if (m_HMDDevice != null && m_HMDDevice.added)
                InputSystem.InputSystem.RemoveDevice(m_HMDDevice);

            if (m_LeftControllerDevice != null && m_LeftControllerDevice.added)
                InputSystem.InputSystem.RemoveDevice(m_LeftControllerDevice);

            if (m_RightControllerDevice != null && m_RightControllerDevice.added)
                InputSystem.InputSystem.RemoveDevice(m_RightControllerDevice);
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> that can be multiplied component-wise with another <see cref="Vector3"/>
        /// to reset components of the <see cref="Vector3"/>, based on axis constraint inputs.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="resetAction"/>
        /// <seealso cref="xConstraintAction"/>
        /// <seealso cref="yConstraintAction"/>
        /// <seealso cref="zConstraintAction"/>
        protected Vector3 GetResetScale()
        {
            return m_XConstraintInput || m_YConstraintInput || m_ZConstraintInput
                ? new Vector3(m_XConstraintInput ? 0f : 1f, m_YConstraintInput ? 0f : 1f, m_ZConstraintInput ? 0f : 1f)
                : Vector3.zero;
        }

        static void GetAxes(Space translateSpace, Transform cameraTransform, out Vector3 right, out Vector3 up, out Vector3 forward)
        {
            if (cameraTransform == null)
                throw new ArgumentNullException(nameof(cameraTransform));

            switch (translateSpace)
            {
                case Space.Local:
                    // Makes the assumption that the Camera and the Controllers are siblings
                    // (meaning they share a parent GameObject).
                    var cameraParent = cameraTransform.parent;
                    if (cameraParent != null)
                    {
                        right = cameraParent.TransformDirection(Vector3.right);
                        up = cameraParent.TransformDirection(Vector3.up);
                        forward = cameraParent.TransformDirection(Vector3.forward);
                    }
                    else
                    {
                        right = Vector3.right;
                        up = Vector3.up;
                        forward = Vector3.forward;
                    }

                    break;
                case Space.Parent:
                    right = Vector3.right;
                    up = Vector3.up;
                    forward = Vector3.forward;
                    break;
                case Space.Screen:
                    right = cameraTransform.TransformDirection(Vector3.right);
                    up = cameraTransform.TransformDirection(Vector3.up);
                    forward = cameraTransform.TransformDirection(Vector3.forward);
                    break;
                default:
                    right = Vector3.right;
                    up = Vector3.up;
                    forward = Vector3.forward;
                    Assert.IsTrue(false, $"Unhandled {nameof(translateSpace)}={translateSpace}.");
                    return;
            }
        }

        static Quaternion GetDeltaRotation(Space translateSpace, in XRSimulatedControllerState state, in Quaternion inverseCameraParentRotation)
        {
            switch (translateSpace)
            {
                case Space.Local:
                    return state.deviceRotation * inverseCameraParentRotation;
                case Space.Parent:
                    return Quaternion.identity;
                case Space.Screen:
                    return inverseCameraParentRotation;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(translateSpace)}={translateSpace}.");
                    return Quaternion.identity;
            }
        }

        static Quaternion GetDeltaRotation(Space translateSpace, in XRSimulatedHMDState state, in Quaternion inverseCameraParentRotation)
        {
            switch (translateSpace)
            {
                case Space.Local:
                    return state.centerEyeRotation * inverseCameraParentRotation;
                case Space.Parent:
                    return Quaternion.identity;
                case Space.Screen:
                    return inverseCameraParentRotation;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(translateSpace)}={translateSpace}.");
                    return Quaternion.identity;
            }
        }

        static void Subscribe(InputActionReference reference, Action<InputAction.CallbackContext> performed = null, Action<InputAction.CallbackContext> canceled = null)
        {
            var action = GetInputAction(reference);
            if (action != null)
            {
                if (performed != null)
                    action.performed += performed;
                if (canceled != null)
                    action.canceled += canceled;
            }
        }

        static void Unsubscribe(InputActionReference reference, Action<InputAction.CallbackContext> performed = null, Action<InputAction.CallbackContext> canceled = null)
        {
            var action = GetInputAction(reference);
            if (action != null)
            {
                if (performed != null)
                    action.performed -= performed;
                if (canceled != null)
                    action.canceled -= canceled;
            }
        }

        static TransformationMode Negate(TransformationMode mode)
        {
            switch (mode)
            {
                case TransformationMode.Rotate:
                    return TransformationMode.Translate;
                case TransformationMode.Translate:
                    return TransformationMode.Rotate;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(mode)}={mode}.");
                    return TransformationMode.Rotate;
            }
        }



        static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }
    }
}
