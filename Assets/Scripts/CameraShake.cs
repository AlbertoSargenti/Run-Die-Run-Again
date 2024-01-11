using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        // Get the CinemachineVirtualCamera component from the main camera
        virtualCamera = Camera.main.GetComponent<CinemachineVirtualCamera>();

        // Ensure the CinemachineVirtualCamera component is not null
        if (virtualCamera != null)
        {
            // Get the CinemachineBasicMultiChannelPerlin component from the virtual camera
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera component not found on the main camera.");
        }
    }

    // Function to shake the camera
    public void ShakeCamera(float intensity, float duration)
    {
        if (noise != null)
        {
            // Set the noise parameters for the shake
            noise.m_AmplitudeGain = intensity;
            noise.m_FrequencyGain = 1f / duration;

            // Invoke a method to reset the camera shake after the specified duration
            Invoke("ResetCameraShake", duration);
        }
        else
        {
            Debug.LogError("CinemachineBasicMultiChannelPerlin component not found.");
        }
    }

    // Function to reset the camera shake parameters
    private void ResetCameraShake()
    {
        if (noise != null)
        {
            // Reset the noise parameters
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }
    }
}
