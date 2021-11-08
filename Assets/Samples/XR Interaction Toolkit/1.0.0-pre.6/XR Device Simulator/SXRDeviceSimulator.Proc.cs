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
        /// Process input from the user and update the state of manipulated controller device(s)
        /// related to button input controls.
        /// </summary>
        /// <param name="controllerState">The controller state that will be processed.</param>
        protected virtual void ProcessControlInputs()
        {
            //m_LeftControllerState.primary2DAxis = m_LeftThumbstick;
            m_LeftControllerState.grip = m_LeftGrip;
            m_LeftControllerState.trigger = m_LeftTrigger;
            m_LeftControllerState.WithButton(ControllerButton.PrimaryButton, m_ButtonX);
            m_LeftControllerState.WithButton(ControllerButton.SecondaryButton, m_ButtonY);
            m_LeftControllerState.WithButton(ControllerButton.MenuButton, m_MenuButton);
            //m_LeftControllerState.WithButton(ControllerButton.Primary2DAxisClick, TBD);


            //m_RightControllerState.primary2DAxis = m_RightThumbstick;
            m_RightControllerState.grip = m_RightGrip;
            m_RightControllerState.trigger = m_RightTrigger;
            m_RightControllerState.WithButton(ControllerButton.PrimaryButton, m_ButtonA);
            m_RightControllerState.WithButton(ControllerButton.SecondaryButton, m_ButtonB);
            m_RightControllerState.WithButton(ControllerButton.MenuButton, m_OculusButton);
            //m_RightControllerState.WithButton(ControllerButton.Primary2DAxisClick, TBD);

            Process2DAxes();        //sets the rotation for Left, Rigth and Head
        }


        /// <summary>
        /// Process input from the user and update the state of manipulated device(s)
        /// related to position and rotation.
        /// </summary>
        protected virtual void Process2DAxes()
        {
            // Reset

            if (m_Reset)
            {
                Debug.Log("Reset!");
                //reset orientation only
                ResetLocation(true, false);
                return;
            }


            var anglesLeft = new Vector3(
                     m_LeftThumbstick.y * m_YHandRotateSensitivity * (m_InvertHandY ? 1f : -1f),
                     m_LeftThumbstick.x * m_XHandRotateSensitivity,
                     0);

            var anglesRight = new Vector3(
                    m_RightThumbstick.y * m_YHandRotateSensitivity * (m_InvertHandY ? 1f : -1f),
                    m_RightThumbstick.x * m_XHandRotateSensitivity,
                    0);

            var anglesHead = new Vector3(
                    m_HeadControl.y * m_YHeadRotateSensitivity * (m_InvertHeadY ? 1f : -1f),
                    m_HeadControl.x * m_XHeadRotateSensitivity,
                    0);


            m_LeftControllerEuler += anglesLeft;
            m_LeftControllerState.deviceRotation = Quaternion.Euler(m_LeftControllerEuler);

            m_RightControllerEuler += anglesRight;
            m_RightControllerState.deviceRotation = Quaternion.Euler(m_RightControllerEuler);

            m_CenterEyeEuler += anglesHead;
            m_HMDState.centerEyeRotation = Quaternion.Euler(m_CenterEyeEuler);
        
        }

        protected virtual void ResetLocation(bool resetRotation = true, bool resetPosition = false)
        {
            if (resetRotation)
            {
                m_LeftControllerEuler = Vector3.zero;
                m_LeftControllerState.deviceRotation = Quaternion.Euler(Vector3.zero);

                m_RightControllerEuler = Vector3.zero; // Vector3.Scale(m_RightControllerEuler, Vector3.zero);
                m_RightControllerState.deviceRotation = Quaternion.Euler(Vector3.zero);

                m_CenterEyeEuler = Vector3.zero; // Vector3.Scale(m_CenterEyeEuler, Vector3.zero);
                m_HMDState.centerEyeRotation = Quaternion.Euler(Vector3.zero);
            }

            if (resetPosition)
            {
                m_LeftControllerState.devicePosition = m_LeftHandStartingPosition + new Vector3(-DEFAULT_DISTANCE_BETWEEN_HANDS / 2, 0f, 0f);
                m_RightControllerState.devicePosition = m_RightHandStartingPosition + new Vector3(DEFAULT_DISTANCE_BETWEEN_HANDS / 2, 0f, 0f);
                m_HMDState.devicePosition = m_HeadStartingPosition;
            }
        }

    }
}
