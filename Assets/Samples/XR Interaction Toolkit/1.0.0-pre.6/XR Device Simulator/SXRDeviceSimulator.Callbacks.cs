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

        /* ==========================================================================
        *  Awake()
        *  ======================================================================= */
        protected virtual void Awake()
        {
            //reset the 3 devices
            m_HMDState.Reset();
            m_LeftControllerState.Reset();
            m_RightControllerState.Reset();

            //store the starting position for the 3 devices
            m_LeftHandStartingPosition = m_LeftControllerState.devicePosition;
            m_RightHandStartingPosition = m_RightControllerState.devicePosition;
            m_HeadStartingPosition = m_HMDState.devicePosition;

            //reset position and orientation
            ResetLocation(true, true);


            // Set tracked states
            m_LeftControllerState.isTracked = true;
            m_RightControllerState.isTracked = true;
            m_HMDState.isTracked = true;
            m_LeftControllerState.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);
            m_RightControllerState.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);
            m_HMDState.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);

        }


        /* ==========================================================================
        *  OnEnable
        *  ======================================================================= */
        protected virtual void OnEnable()
        {
            /*
            // Find the Camera if necessary
            if (m_CameraTransform == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                    m_CameraTransform = mainCamera.transform;
            }
            */

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

        /* ==========================================================================
        *  OnDisable()
        *  ======================================================================= */
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

        /* ==========================================================================
        *  Update()
        *  ======================================================================= */
        protected virtual void Update()
        {
            ProcessControlInputs();

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



    }
}
