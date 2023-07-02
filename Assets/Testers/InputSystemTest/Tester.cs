using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using HapticSystem;

public class Tester : MonoBehaviour
{
    public HapticClip clip = null;
    public HapticClip loopClip = null;


    private HapticClipInstance loopClipInstance = null;
    PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void DeviceId(InputAction.CallbackContext callback)
    {
        if (!callback.started) return;
        int id = playerInput.devices[0].deviceId;
        HapticManager.PlayClip(clip, id);
        Debug.Log("Play with device ID");
    }

    public void InputDevice(InputAction.CallbackContext callback)
    {
        if (!callback.started) return;
        HapticManager.PlayClip(clip, playerInput.devices[0].device);
        Debug.Log("Play with InputDevice");
    }

    public void PlayerInput(InputAction.CallbackContext callback)
    {
        if (!callback.started) return;
        HapticManager.PlayClip(clip, playerInput);
        Debug.Log("Play withPlayerInput");
    }

    public void Gamepad_(InputAction.CallbackContext callback)
    {
        if (!callback.started) return;
        HapticManager.PlayClip(clip, Gamepad.current);
    }

    public void Loop(InputAction.CallbackContext callback)
    {
        if (callback.started)
        {
            HapticManager.StopClipInstance(loopClipInstance);
            loopClipInstance = HapticManager.PlayClip(loopClip, playerInput);
            Debug.Log("Play Loop start");
        }
        else if (callback.canceled)
        {
            HapticManager.StopClipInstance(loopClipInstance);
            Debug.Log("Play Loop stop");
        }
    }
}
