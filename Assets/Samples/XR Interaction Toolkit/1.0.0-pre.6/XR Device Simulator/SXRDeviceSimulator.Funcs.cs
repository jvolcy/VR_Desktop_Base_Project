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

    public partial class SXRDeviceSimulator : MonoBehaviour
    {


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


            Vector3 anglesLeft, anglesRight, anglesHead;

                    anglesLeft = new Vector3(scaledManipulateLeftHandInput.y, scaledManipulateLeftHandInput.x, 0f);
                    anglesRight = new Vector3(scaledManipulateRightHandInput.y, scaledManipulateRightHandInput.x, 0f);
                    anglesHead = new Vector3(scaledManipulateHeadInput.y, scaledManipulateHeadInput.x, 0f);
                

                    m_LeftControllerEuler += anglesLeft;
                    m_LeftControllerState.deviceRotation = Quaternion.Euler(m_LeftControllerEuler);

                    m_RightControllerEuler += anglesRight;
                    m_RightControllerState.deviceRotation = Quaternion.Euler(m_RightControllerEuler);

                    m_CenterEyeEuler += anglesHead;
                    m_HMDState.centerEyeRotation = Quaternion.Euler(m_CenterEyeEuler);

            // Reset
    
            if (m_Reset)
                {
                Debug.Log("Reset!");
                        m_LeftControllerEuler = Vector3.Scale(m_LeftControllerEuler, Vector3.zero);
                        m_LeftControllerState.deviceRotation = Quaternion.Euler(m_LeftControllerEuler);

                        m_RightControllerEuler = Vector3.Scale(m_RightControllerEuler, Vector3.zero);
                        m_RightControllerState.deviceRotation = Quaternion.Euler(m_RightControllerEuler);

                        m_CenterEyeEuler = Vector3.Scale(m_CenterEyeEuler, Vector3.zero);
                        m_HMDState.centerEyeRotation = Quaternion.Euler(m_CenterEyeEuler);
                }
                
        }

        /// <summary>
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to input controls.
        /// </summary>
        protected virtual void ProcessControlInput()
        {
            ProcessAxis2DControlInput();
            ProcessLeftButtonControlInput(ref m_LeftControllerState);
            ProcessRightButtonControlInput(ref m_RightControllerState);
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


        static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }
    }
}
