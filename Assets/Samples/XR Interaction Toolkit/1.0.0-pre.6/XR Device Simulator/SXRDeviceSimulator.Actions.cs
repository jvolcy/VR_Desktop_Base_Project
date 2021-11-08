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
        *  Reset
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Reset")]
        InputActionReference m_ResetAction;
        public InputActionReference Reset
        {
            get => m_ResetAction;
            set
            {
                UnsubscribeResetAction();
                m_ResetAction = value;
                SubscribeResetAction();
            }
        }

        void SubscribeResetAction() => Subscribe(m_ResetAction, OnResetPerformed, OnResetCanceled);
        void UnsubscribeResetAction() => Unsubscribe(m_ResetAction, OnResetPerformed, OnResetCanceled);

        void OnResetPerformed(InputAction.CallbackContext context) => m_Reset = true;
        void OnResetCanceled(InputAction.CallbackContext context) => m_Reset = false;


        /* ==========================================================================
        *  Right Thumbstick
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Right Thumbstick")]
        InputActionReference m_RightThumbstickAction;
        public InputActionReference RightThumbstick
        {
            get => m_RightThumbstickAction;
            set
            {
                UnsubscribeRightThumbstickAction();
                m_RightThumbstickAction = value;
                SubscribeRightThumbstickAction();
            }
        }

        void SubscribeRightThumbstickAction() => Subscribe(m_RightThumbstickAction, OnRightThumbstickPerformed, OnRightThumbstickCanceled);
        void UnsubscribeRightThumbstickAction() => Unsubscribe(m_RightThumbstickAction, OnRightThumbstickPerformed, OnRightThumbstickCanceled);

        void OnRightThumbstickPerformed(InputAction.CallbackContext context) => m_RightThumbstick = context.ReadValue<Vector2>();
        void OnRightThumbstickCanceled(InputAction.CallbackContext context) => m_RightThumbstick = Vector2.zero;


        /* ==========================================================================
        *  Left Thumbstick
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Left Thumbstick")]
        InputActionReference m_LeftThumbstickAction;
        public InputActionReference LeftThumbstick
        {
            get => m_LeftThumbstickAction;
            set
            {
                UnsubscribeLeftThumbstickAction();
                m_LeftThumbstickAction = value;
                SubscribeLeftThumbstickAction();
            }
        }

        void SubscribeLeftThumbstickAction() => Subscribe(m_LeftThumbstickAction, OnLeftThumbstickPerformed, OnLeftThumbstickCanceled);
        void UnsubscribeLeftThumbstickAction() => Unsubscribe(m_LeftThumbstickAction, OnLeftThumbstickPerformed, OnLeftThumbstickCanceled);

        void OnLeftThumbstickPerformed(InputAction.CallbackContext context) => m_LeftThumbstick = context.ReadValue<Vector2>();
        void OnLeftThumbstickCanceled(InputAction.CallbackContext context) => m_LeftThumbstick = Vector2.zero;


        /* ==========================================================================
        *  Head Control
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Head Control")]
        InputActionReference m_HeadControlAction;
        public InputActionReference HeadControl
        {
            get => m_HeadControlAction;
            set
            {
                UnsubscribeHeadControlAction();
                m_HeadControlAction = value;
                SubscribeHeadControlAction();
            }
        }

        void SubscribeHeadControlAction() => Subscribe(m_HeadControlAction, OnHeadControlPerformed, OnHeadControlCanceled);
        void UnsubscribeHeadControlAction() => Unsubscribe(m_HeadControlAction, OnHeadControlPerformed, OnHeadControlCanceled);

        void OnHeadControlPerformed(InputAction.CallbackContext context) => m_HeadControl = context.ReadValue<Vector2>();
        void OnHeadControlCanceled(InputAction.CallbackContext context) => m_HeadControl = Vector2.zero;


        /* ==========================================================================
        *  Left Grip
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Left Grip")]
        InputActionReference m_LeftGripAction;
        public InputActionReference LeftGrip
        {
            get => m_LeftGripAction;
            set
            {
                UnsubscribeLeftGripAction();
                m_LeftGripAction = value;
                SubscribeLeftGripAction();
            }
        }

        void SubscribeLeftGripAction() => Subscribe(m_LeftGripAction, OnLeftGripPerformed, OnLeftGripCanceled);
        void UnsubscribeLeftGripAction() => Unsubscribe(m_LeftGripAction, OnLeftGripPerformed, OnLeftGripCanceled);

        void OnLeftGripPerformed(InputAction.CallbackContext context) => m_LeftGrip = context.ReadValue<float>();
        void OnLeftGripCanceled(InputAction.CallbackContext context) => m_LeftGrip = 0f;


        /* ==========================================================================
        *  Right Grip
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Right Grip")]
        InputActionReference m_RightGripAction;
        public InputActionReference RightGrip
        {
            get => m_RightGripAction;
            set
            {
                UnsubscribeRightGripAction();
                m_RightGripAction = value;
                SubscribeRightGripAction();
            }
        }

        void SubscribeRightGripAction() => Subscribe(m_RightGripAction, OnRightGripPerformed, OnRightGripCanceled);
        void UnsubscribeRightGripAction() => Unsubscribe(m_RightGripAction, OnRightGripPerformed, OnRightGripCanceled);

        void OnRightGripPerformed(InputAction.CallbackContext context) => m_RightGrip = context.ReadValue<float>();
        void OnRightGripCanceled(InputAction.CallbackContext context) => m_RightGrip = 0f;


        /* ==========================================================================
        *  Left Trigger
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Left Trigger")]
        InputActionReference m_LeftTriggerAction;
        public InputActionReference LeftTrigger
        {
            get => m_LeftTriggerAction;
            set
            {
                UnsubscribeLeftTriggerAction();
                m_LeftTriggerAction = value;
                SubscribeLeftTriggerAction();
            }
        }

        void SubscribeLeftTriggerAction() => Subscribe(m_LeftTriggerAction, OnLeftTriggerPerformed, OnLeftTriggerCanceled);
        void UnsubscribeLeftTriggerAction() => Unsubscribe(m_LeftTriggerAction, OnLeftTriggerPerformed, OnLeftTriggerCanceled);

        void OnLeftTriggerPerformed(InputAction.CallbackContext context) => m_LeftTrigger = context.ReadValue<float>();
        void OnLeftTriggerCanceled(InputAction.CallbackContext context) => m_LeftTrigger = 0f;


        /* ==========================================================================
        *  Right Trigger
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Right Trigger")]
        InputActionReference m_RightTriggerAction;
        public InputActionReference RightTrigger
        {
            get => m_RightTriggerAction;
            set
            {
                UnsubscribeRightTriggerAction();
                m_RightTriggerAction = value;
                SubscribeRightTriggerAction();
            }
        }

        void SubscribeRightTriggerAction() => Subscribe(m_RightTriggerAction, OnRightTriggerPerformed, OnRightTriggerCanceled);
        void UnsubscribeRightTriggerAction() => Unsubscribe(m_RightTriggerAction, OnRightTriggerPerformed, OnRightTriggerCanceled);

        void OnRightTriggerPerformed(InputAction.CallbackContext context) => m_RightTrigger = context.ReadValue<float>();
        void OnRightTriggerCanceled(InputAction.CallbackContext context) => m_RightTrigger = 0f;


        /* ==========================================================================
        *  Button A
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Button A")]
        InputActionReference m_ButtonAAction;
        public InputActionReference ButtonA
        {
            get => m_ButtonAAction;
            set
            {
                UnsubscribeButtonAAction();
                m_ButtonAAction = value;
                SubscribeButtonAAction();
            }
        }

        void SubscribeButtonAAction() => Subscribe(m_ButtonAAction, OnButtonAPerformed, OnButtonACanceled);
        void UnsubscribeButtonAAction() => Unsubscribe(m_ButtonAAction, OnButtonAPerformed, OnButtonACanceled);

        void OnButtonAPerformed(InputAction.CallbackContext context) => m_ButtonA = true;
        void OnButtonACanceled(InputAction.CallbackContext context) => m_ButtonA = false;


        /* ==========================================================================
        *  Button B
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Button B")]
        InputActionReference m_ButtonBAction;
        public InputActionReference ButtonB
        {
            get => m_ButtonBAction;
            set
            {
                UnsubscribeButtonBAction();
                m_ButtonBAction = value;
                SubscribeButtonBAction();
            }
        }

        void SubscribeButtonBAction() => Subscribe(m_ButtonBAction, OnButtonBPerformed, OnButtonBCanceled);
        void UnsubscribeButtonBAction() => Unsubscribe(m_ButtonBAction, OnButtonBPerformed, OnButtonBCanceled);

        void OnButtonBPerformed(InputAction.CallbackContext context) => m_ButtonB = true;
        void OnButtonBCanceled(InputAction.CallbackContext context) => m_ButtonB = false;


        /* ==========================================================================
        *  Button X
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Button X")]
        InputActionReference m_ButtonXAction;
        public InputActionReference ButtonX
        {
            get => m_ButtonXAction;
            set
            {
                UnsubscribeButtonXAction();
                m_ButtonXAction = value;
                SubscribeButtonXAction();
            }
        }

        void SubscribeButtonXAction() => Subscribe(m_ButtonXAction, OnButtonXPerformed, OnButtonXCanceled);
        void UnsubscribeButtonXAction() => Unsubscribe(m_ButtonXAction, OnButtonXPerformed, OnButtonXCanceled);

        void OnButtonXPerformed(InputAction.CallbackContext context) => m_ButtonX = true;
        void OnButtonXCanceled(InputAction.CallbackContext context) => m_ButtonX = false;


        /* ==========================================================================
        *  Button Y
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Button Y")]
        InputActionReference m_ButtonYAction;
        public InputActionReference ButtonY
        {
            get => m_ButtonYAction;
            set
            {
                UnsubscribeButtonYAction();
                m_ButtonYAction = value;
                SubscribeButtonYAction();
            }
        }

        void SubscribeButtonYAction() => Subscribe(m_ButtonYAction, OnButtonYPerformed, OnButtonYCanceled);
        void UnsubscribeButtonYAction() => Unsubscribe(m_ButtonYAction, OnButtonYPerformed, OnButtonYCanceled);

        void OnButtonYPerformed(InputAction.CallbackContext context) => m_ButtonY = true;
        void OnButtonYCanceled(InputAction.CallbackContext context) => m_ButtonY = false;


        /* ==========================================================================
        *  Oculus Button
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Oculus Button")]
        InputActionReference m_OculusButtonAction;
        public InputActionReference OculusButton
        {
            get => m_OculusButtonAction;
            set
            {
                UnsubscribeOculusButtonAction();
                m_OculusButtonAction = value;
                SubscribeOculusButtonAction();
            }
        }

        void SubscribeOculusButtonAction() => Subscribe(m_OculusButtonAction, OnOculusButtonPerformed, OnOculusButtonCanceled);
        void UnsubscribeOculusButtonAction() => Unsubscribe(m_OculusButtonAction, OnOculusButtonPerformed, OnOculusButtonCanceled);

        void OnOculusButtonPerformed(InputAction.CallbackContext context) => m_OculusButton = true;
        void OnOculusButtonCanceled(InputAction.CallbackContext context) => m_OculusButton = false;


        /* ==========================================================================
        *  Menu Button
        *  ======================================================================= */
        [SerializeField]
        [Tooltip("Menu Button")]
        InputActionReference m_MenuButtonAction;
        public InputActionReference MenuButton
        {
            get => m_MenuButtonAction;
            set
            {
                UnsubscribeMenuButtonAction();
                m_MenuButtonAction = value;
                SubscribeMenuButtonAction();
            }
        }

        void SubscribeMenuButtonAction() => Subscribe(m_MenuButtonAction, OnMenuButtonPerformed, OnMenuButtonCanceled);
        void UnsubscribeMenuButtonAction() => Unsubscribe(m_MenuButtonAction, OnMenuButtonPerformed, OnMenuButtonCanceled);

        void OnMenuButtonPerformed(InputAction.CallbackContext context) => m_MenuButton = true;
        void OnMenuButtonCanceled(InputAction.CallbackContext context) => m_MenuButton = false;



        //==============================================================
        /*

        [SerializeField]
        [Tooltip("The Input System Action used to translate or rotate the left hand by a scaled amount along or about the x- and y-axes. Must be a Value Vector2 Control.")]
        InputActionReference m_ManipulateLeftHandAction;
        public InputActionReference manipulateLeftHandAction
        {
            get => m_ManipulateLeftHandAction;
            set
            {
                UnsubscribeManipulateLeftHandAction();
                m_ManipulateLeftHandAction = value;
                SubscribeManipulateLeftHandAction();
            }
        }

        [SerializeField]
        [Tooltip("The Input System Action used to translate or rotate the right hand by a scaled amount along or about the x- and y-axes. Must be a Value Vector2 Control.")]
        InputActionReference m_ManipulateRightHandAction;
        public InputActionReference manipulateRightHandAction
        {
            get => m_ManipulateRightHandAction;
            set
            {
                UnsubscribeManipulateRightHandAction();
                m_ManipulateRightHandAction = value;
                SubscribeManipulateRightHandAction();
            }
        }

        [SerializeField]
        [Tooltip("The Input System Action used to translate or rotate the head by a scaled amount along or about the x- and y-axes. Must be a Value Vector2 Control.")]
        InputActionReference m_ManipulateHeadAction;
        public InputActionReference manipulateHeadAction
        {
            get => m_ManipulateHeadAction;
            set
            {
                UnsubscribeManipulateHeadAction();
                m_ManipulateHeadAction = value;
                Debug.Log("H" + value);
                SubscribeManipulateHeadAction();
            }
        }

        */

        /*
        void SubscribeManipulateLeftHandAction() => Subscribe(m_ManipulateLeftHandAction, OnManipulateLeftHandPerformed, OnManipulateLeftHandCanceled);
        void UnsubscribeManipulateLeftHandAction() => Unsubscribe(m_ManipulateLeftHandAction, OnManipulateLeftHandPerformed, OnManipulateLeftHandCanceled);

        void SubscribeManipulateRightHandAction() => Subscribe(m_ManipulateRightHandAction, OnManipulateRightHandPerformed, OnManipulateRightHandCanceled);
        void UnsubscribeManipulateRightHandAction() => Unsubscribe(m_ManipulateRightHandAction, OnManipulateRightHandPerformed, OnManipulateRightHandCanceled);

        void SubscribeManipulateHeadAction() => Subscribe(m_ManipulateHeadAction, OnManipulateHeadPerformed, OnManipulateHeadCanceled);
        void UnsubscribeManipulateHeadAction() => Unsubscribe(m_ManipulateHeadAction, OnManipulateHeadPerformed, OnManipulateHeadCanceled);

        

        void OnManipulateLeftHandPerformed(InputAction.CallbackContext context) { m_ManipulateLeftHandInput = 10*context.ReadValue<Vector2>();
            //Debug.Log("raw left=" + m_ManipulateLeftHandInput);
        }
        void OnManipulateLeftHandCanceled(InputAction.CallbackContext context) => m_ManipulateLeftHandInput = Vector2.zero;

        void OnManipulateRightHandPerformed(InputAction.CallbackContext context) { m_ManipulateRightHandInput = 10*context.ReadValue<Vector2>();
            //Debug.Log("raw right=" + m_ManipulateRightHandInput);
        }
        void OnManipulateRightHandCanceled(InputAction.CallbackContext context) => m_ManipulateRightHandInput = Vector2.zero;

        void OnManipulateHeadPerformed(InputAction.CallbackContext context) { m_ManipulateHeadInput = 2*context.ReadValue<Vector2>();
            //Debug.Log("raw head=" + m_ManipulateHeadInput);

        }
        void OnManipulateHeadCanceled(InputAction.CallbackContext context) => m_ManipulateHeadInput = Vector2.zero;
        */
        
    }
}
