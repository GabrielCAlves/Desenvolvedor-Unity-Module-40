using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;

public class SoundManager : Singleton<SoundManager>
{
    public List<MusicSetup> musicSetups;
    public List<SFXSetup> sfxSetups;

    public AudioSource musicSource;

    public void PlayMusicByType(MusicType musicType)
    {
        var music = GetMusicByType(musicType);
        musicSource.clip = music.audioClip;
        musicSource.Play();
    }

    public MusicSetup GetMusicByType(MusicType musicType)
    {
        return musicSetups.Find(i => i.musicType == musicType);
    }

    public SFXSetup GetSFXByType(SFXType sfxType)
    {
        return sfxSetups.Find(i => i.sfxType == sfxType);
    }
}

public enum MusicType
{
    BACKGROUND_01,
    BACKGROUND_02,
    BACKGROUND_03,
}

[System.Serializable]
public class MusicSetup
{
    public MusicType musicType;
    public AudioClip audioClip;
}

public enum SFXType
{
    NONE,
    JUMP_01,
    ENDGAME_01,
    ENEMY_DEATH,
    COIN_01,
    ENDGAME_02,
    SHOOT_01,
    COIN_02,
    JUMP_02,
    SHOOT_02,
    PLAYER_WALK,
    PLAYER_DEATH,
    CHEST_OPEN
}

[System.Serializable]
public class SFXSetup
{
    public SFXType sfxType;
    public AudioClip audioClip;
}

