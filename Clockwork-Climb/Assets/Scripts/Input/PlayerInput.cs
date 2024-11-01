//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/Input/PlayerInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""8cc95500-70b3-4eb8-b64b-513eaf999ecd"",
            ""actions"": [
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""acc592a7-995c-4e99-9b6d-62e630db8a96"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraPan"",
                    ""type"": ""Value"",
                    ""id"": ""b8b80c65-e3dc-4a42-9cc3-7be1ef65d3ec"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CameraZoom"",
                    ""type"": ""Value"",
                    ""id"": ""27ac0079-e303-438f-8e39-07d2bad3cd71"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CameraToggle"",
                    ""type"": ""Button"",
                    ""id"": ""ebc383c1-93b2-46af-80d7-ad1cdeb3a252"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PrimaryTouchContact"",
                    ""type"": ""Button"",
                    ""id"": ""394ec182-f14f-46dd-8b27-e606183732fb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PrimaryTouchPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""29c58cda-1914-4c05-bf73-cd6bdee8081f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SecondaryTouchContact"",
                    ""type"": ""Button"",
                    ""id"": ""9b6d98bd-a8f9-44e7-b080-2ab3b75a94d4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SecondaryTouchPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""039ea679-e8a2-4c6e-a70a-2f82a38d5a91"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TertiaryTouchContact"",
                    ""type"": ""Button"",
                    ""id"": ""30181d06-6a40-420e-b909-52f5eb44772b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TertiaryTouchPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d8f90618-0613-4194-8987-91e544902458"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0785f4f0-8b31-451c-b558-c75cf96f123b"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ecf8830a-b0a1-4ed2-a2bc-1fe4def33a62"",
                    ""path"": ""<Joystick>/stick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD [Keyboard]"",
                    ""id"": ""4af2c418-69a5-4f00-a615-5a4f11743824"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d27bfabe-2726-4db8-8ad6-f32329f80adc"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2f0ea64d-4e29-4665-8382-41a49dff49d3"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""19b977c4-f31f-48f5-9f46-bf00e1f3772e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""1298c1fd-15af-4036-9eda-9cde5486a454"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys [Keyboard]"",
                    ""id"": ""b96d0f81-d06f-4b1c-bcf0-dab03aa37111"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f0168354-7b14-4107-98e3-daa297b86498"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""51250423-44f8-4012-8e2a-03b3808b153f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""151f90d7-f741-4f20-9425-8e8f5018a6c0"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4ecc4c5d-dd08-461d-a38c-f396f6633f0e"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraPan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Scroll Wheel [Mouse]"",
                    ""id"": ""90d9560c-2078-472d-9bec-9291b8c3bfc8"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0.1)"",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""0e25eacd-e78b-4b3a-87e7-17427674609b"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""aee0f181-353d-4f82-8d36-cb26ef465f97"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Page Up/Down [Keyboard]"",
                    ""id"": ""4ce54d2c-f368-428b-8a45-cf1529f7a8f3"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c1a63528-cfd6-41f1-a2d0-842ee3b8cdf3"",
                    ""path"": ""<Keyboard>/pageDown"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""434fc4bd-8046-4f1e-be87-8d7ef7b1e2f8"",
                    ""path"": ""<Keyboard>/pageUp"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""2f732680-88ad-4ea1-b9c0-728ea795b1b8"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e468b61-a207-4475-b874-09ebd461ded2"",
                    ""path"": ""<Touchscreen>/touch0/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimaryTouchContact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5428fc96-e65d-4e11-9f8f-56b6e14c6983"",
                    ""path"": ""<Touchscreen>/touch0/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PrimaryTouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8186491-df28-49f2-b243-ca2c2a1fb59f"",
                    ""path"": ""<Touchscreen>/touch1/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondaryTouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""807c1970-fed8-4c95-aabc-3ece3062cbb2"",
                    ""path"": ""<Touchscreen>/touch1/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SecondaryTouchContact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0dae98dc-9d27-42a6-bb28-b1f44b0d818b"",
                    ""path"": ""<Touchscreen>/touch2/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TertiaryTouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f09965b-3461-4df4-a2a4-042edb6ac739"",
                    ""path"": ""<Touchscreen>/touch2/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TertiaryTouchContact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Menu = m_Gameplay.FindAction("Menu", throwIfNotFound: true);
        m_Gameplay_CameraPan = m_Gameplay.FindAction("CameraPan", throwIfNotFound: true);
        m_Gameplay_CameraZoom = m_Gameplay.FindAction("CameraZoom", throwIfNotFound: true);
        m_Gameplay_CameraToggle = m_Gameplay.FindAction("CameraToggle", throwIfNotFound: true);
        m_Gameplay_PrimaryTouchContact = m_Gameplay.FindAction("PrimaryTouchContact", throwIfNotFound: true);
        m_Gameplay_PrimaryTouchPosition = m_Gameplay.FindAction("PrimaryTouchPosition", throwIfNotFound: true);
        m_Gameplay_SecondaryTouchContact = m_Gameplay.FindAction("SecondaryTouchContact", throwIfNotFound: true);
        m_Gameplay_SecondaryTouchPosition = m_Gameplay.FindAction("SecondaryTouchPosition", throwIfNotFound: true);
        m_Gameplay_TertiaryTouchContact = m_Gameplay.FindAction("TertiaryTouchContact", throwIfNotFound: true);
        m_Gameplay_TertiaryTouchPosition = m_Gameplay.FindAction("TertiaryTouchPosition", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
    private readonly InputAction m_Gameplay_Menu;
    private readonly InputAction m_Gameplay_CameraPan;
    private readonly InputAction m_Gameplay_CameraZoom;
    private readonly InputAction m_Gameplay_CameraToggle;
    private readonly InputAction m_Gameplay_PrimaryTouchContact;
    private readonly InputAction m_Gameplay_PrimaryTouchPosition;
    private readonly InputAction m_Gameplay_SecondaryTouchContact;
    private readonly InputAction m_Gameplay_SecondaryTouchPosition;
    private readonly InputAction m_Gameplay_TertiaryTouchContact;
    private readonly InputAction m_Gameplay_TertiaryTouchPosition;
    public struct GameplayActions
    {
        private @PlayerInput m_Wrapper;
        public GameplayActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Menu => m_Wrapper.m_Gameplay_Menu;
        public InputAction @CameraPan => m_Wrapper.m_Gameplay_CameraPan;
        public InputAction @CameraZoom => m_Wrapper.m_Gameplay_CameraZoom;
        public InputAction @CameraToggle => m_Wrapper.m_Gameplay_CameraToggle;
        public InputAction @PrimaryTouchContact => m_Wrapper.m_Gameplay_PrimaryTouchContact;
        public InputAction @PrimaryTouchPosition => m_Wrapper.m_Gameplay_PrimaryTouchPosition;
        public InputAction @SecondaryTouchContact => m_Wrapper.m_Gameplay_SecondaryTouchContact;
        public InputAction @SecondaryTouchPosition => m_Wrapper.m_Gameplay_SecondaryTouchPosition;
        public InputAction @TertiaryTouchContact => m_Wrapper.m_Gameplay_TertiaryTouchContact;
        public InputAction @TertiaryTouchPosition => m_Wrapper.m_Gameplay_TertiaryTouchPosition;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
            @Menu.started += instance.OnMenu;
            @Menu.performed += instance.OnMenu;
            @Menu.canceled += instance.OnMenu;
            @CameraPan.started += instance.OnCameraPan;
            @CameraPan.performed += instance.OnCameraPan;
            @CameraPan.canceled += instance.OnCameraPan;
            @CameraZoom.started += instance.OnCameraZoom;
            @CameraZoom.performed += instance.OnCameraZoom;
            @CameraZoom.canceled += instance.OnCameraZoom;
            @CameraToggle.started += instance.OnCameraToggle;
            @CameraToggle.performed += instance.OnCameraToggle;
            @CameraToggle.canceled += instance.OnCameraToggle;
            @PrimaryTouchContact.started += instance.OnPrimaryTouchContact;
            @PrimaryTouchContact.performed += instance.OnPrimaryTouchContact;
            @PrimaryTouchContact.canceled += instance.OnPrimaryTouchContact;
            @PrimaryTouchPosition.started += instance.OnPrimaryTouchPosition;
            @PrimaryTouchPosition.performed += instance.OnPrimaryTouchPosition;
            @PrimaryTouchPosition.canceled += instance.OnPrimaryTouchPosition;
            @SecondaryTouchContact.started += instance.OnSecondaryTouchContact;
            @SecondaryTouchContact.performed += instance.OnSecondaryTouchContact;
            @SecondaryTouchContact.canceled += instance.OnSecondaryTouchContact;
            @SecondaryTouchPosition.started += instance.OnSecondaryTouchPosition;
            @SecondaryTouchPosition.performed += instance.OnSecondaryTouchPosition;
            @SecondaryTouchPosition.canceled += instance.OnSecondaryTouchPosition;
            @TertiaryTouchContact.started += instance.OnTertiaryTouchContact;
            @TertiaryTouchContact.performed += instance.OnTertiaryTouchContact;
            @TertiaryTouchContact.canceled += instance.OnTertiaryTouchContact;
            @TertiaryTouchPosition.started += instance.OnTertiaryTouchPosition;
            @TertiaryTouchPosition.performed += instance.OnTertiaryTouchPosition;
            @TertiaryTouchPosition.canceled += instance.OnTertiaryTouchPosition;
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @Menu.started -= instance.OnMenu;
            @Menu.performed -= instance.OnMenu;
            @Menu.canceled -= instance.OnMenu;
            @CameraPan.started -= instance.OnCameraPan;
            @CameraPan.performed -= instance.OnCameraPan;
            @CameraPan.canceled -= instance.OnCameraPan;
            @CameraZoom.started -= instance.OnCameraZoom;
            @CameraZoom.performed -= instance.OnCameraZoom;
            @CameraZoom.canceled -= instance.OnCameraZoom;
            @CameraToggle.started -= instance.OnCameraToggle;
            @CameraToggle.performed -= instance.OnCameraToggle;
            @CameraToggle.canceled -= instance.OnCameraToggle;
            @PrimaryTouchContact.started -= instance.OnPrimaryTouchContact;
            @PrimaryTouchContact.performed -= instance.OnPrimaryTouchContact;
            @PrimaryTouchContact.canceled -= instance.OnPrimaryTouchContact;
            @PrimaryTouchPosition.started -= instance.OnPrimaryTouchPosition;
            @PrimaryTouchPosition.performed -= instance.OnPrimaryTouchPosition;
            @PrimaryTouchPosition.canceled -= instance.OnPrimaryTouchPosition;
            @SecondaryTouchContact.started -= instance.OnSecondaryTouchContact;
            @SecondaryTouchContact.performed -= instance.OnSecondaryTouchContact;
            @SecondaryTouchContact.canceled -= instance.OnSecondaryTouchContact;
            @SecondaryTouchPosition.started -= instance.OnSecondaryTouchPosition;
            @SecondaryTouchPosition.performed -= instance.OnSecondaryTouchPosition;
            @SecondaryTouchPosition.canceled -= instance.OnSecondaryTouchPosition;
            @TertiaryTouchContact.started -= instance.OnTertiaryTouchContact;
            @TertiaryTouchContact.performed -= instance.OnTertiaryTouchContact;
            @TertiaryTouchContact.canceled -= instance.OnTertiaryTouchContact;
            @TertiaryTouchPosition.started -= instance.OnTertiaryTouchPosition;
            @TertiaryTouchPosition.performed -= instance.OnTertiaryTouchPosition;
            @TertiaryTouchPosition.canceled -= instance.OnTertiaryTouchPosition;
        }

        public void RemoveCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMenu(InputAction.CallbackContext context);
        void OnCameraPan(InputAction.CallbackContext context);
        void OnCameraZoom(InputAction.CallbackContext context);
        void OnCameraToggle(InputAction.CallbackContext context);
        void OnPrimaryTouchContact(InputAction.CallbackContext context);
        void OnPrimaryTouchPosition(InputAction.CallbackContext context);
        void OnSecondaryTouchContact(InputAction.CallbackContext context);
        void OnSecondaryTouchPosition(InputAction.CallbackContext context);
        void OnTertiaryTouchContact(InputAction.CallbackContext context);
        void OnTertiaryTouchPosition(InputAction.CallbackContext context);
    }
}
