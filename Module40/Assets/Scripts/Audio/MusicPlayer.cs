using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    public MusicType musicType;
    public AudioSource audioSource;

    public List<Sprite> iconSprites;
    public Image substitute;

    private bool _turnOnOff = true;

    private MusicSetup _currentMusicSetup;

    void Start()
    {
        Play();
    }

    public void TurnOnOffMusic()
    {
        _turnOnOff = !_turnOnOff;

        if (!_turnOnOff)
        {
            Debug.Log("Dentro de !_turnOnOff. _turnOnOff = " + _turnOnOff);
            //SoundManager.Instance.musicSource.Pause();
            audioSource.Pause();
            substitute.sprite = iconSprites[1];
        }
        else
        {
            Debug.Log("Dentro de _turnOnOff. _turnOnOff = " + _turnOnOff);
            //SoundManager.Instance.musicSource.Play();
            audioSource.Play();
            substitute.sprite = iconSprites[0];
        }
    }

    private void Play()
    {
        _currentMusicSetup = SoundManager.Instance.GetMusicByType(musicType);

        audioSource.clip = _currentMusicSetup.audioClip;
        audioSource.Play();
    }
}
