using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GetPlayerPrefabAnimator : MonoBehaviour
{
    private Player _player;
    private Animator _animator;

    private CinemachineStateDrivenCamera _cinemachineStateDrivenCamera;
    private CinemachineVirtualCamera _currentVirtualCamera;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_cinemachineStateDrivenCamera == null)
        {
            _cinemachineStateDrivenCamera = gameObject.GetComponent<CinemachineStateDrivenCamera>();
        }

        _player = GameObject.FindObjectOfType<Player>();

        if (_player != null)
        {
            _animator = _player.GetComponentInChildren<Animator>();

            // Passando a referência da virtual camera para o Player
            _player.SetVirtualCameraReference(this);
        }

        if (_animator != null)
        {
            var stateDrivenCAM = _cinemachineStateDrivenCamera;
            stateDrivenCAM.m_AnimatedTarget = _animator;
        }
    }

    private void Update()
    {
        // Obtém a virtual camera atualmente ativa
        if (_cinemachineStateDrivenCamera != null && _currentVirtualCamera != _cinemachineStateDrivenCamera.LiveChild)
        {
            Debug.Log("_currentVirtualCamera = "+ _currentVirtualCamera);
            Debug.Log("_cinemachineStateDrivenCamera.LiveChild = "+ _cinemachineStateDrivenCamera.LiveChild);

            _currentVirtualCamera = _cinemachineStateDrivenCamera.LiveChild as CinemachineVirtualCamera;
        }
    }

    // Método para acessar a virtual camera atual
    public CinemachineVirtualCamera GetCurrentVirtualCamera()
    {
        return _currentVirtualCamera;
    }

    // Aplica o noise na camera atual
    public void ApplyCameraNoise(float amplitudeGain, float frequencyGain, float duration)
    {
        if (_currentVirtualCamera != null)
        {
            var noise = _currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null)
            {
                noise.m_AmplitudeGain = amplitudeGain;
                noise.m_FrequencyGain = frequencyGain;

                if (duration > 0)
                {
                    StartCoroutine(ResetCameraNoise(noise, duration));
                }
            }
        }
    }

    // Reseta o noise na camera atual
    private IEnumerator ResetCameraNoise(CinemachineBasicMultiChannelPerlin noise, float duration)
    {
        yield return new WaitForSeconds(duration);

        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }
}