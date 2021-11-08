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


    }
}
