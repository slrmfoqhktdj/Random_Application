using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public SoundItem[] sounds;
    private const string ButtonClickSoundName = "SD_BUTTON";
    private const string PlaySoundName = "SD_PLAY";
    private const string PlaySoundComplete = "SD_COMPLETE";
    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        foreach (SoundItem s in sounds) // 각 sound 객체의 AudioSource 컴포넌트 생성 및 여러 세팅.
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            //s.source.volume = PlayerPrefsManager.Instance.GetInt(s.name, 5) / 10f;
            s.source.volume = PlayerPrefsManager.GetInt(s.name, 5) / 10f;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void SetVolume(string name, int volume)
    {
        SoundItem s = Array.Find(sounds, sound => sound.name == name); // sounds 배열 내 name과 같은 이름을 가진 sound 찾기.

        if (s == null) // sound를 찾지 못한 경우,
        {
            Debug.LogWarning("Sound : " + name + " Not Found!");
            return;
        }
        s.source.volume = volume / 10f;
    }

    public void Play(string name)
    {
        SoundItem s = Array.Find(sounds, sound => sound.name == name); // sounds 배열 내 name과 같은 이름을 가진 sound 찾기.

        if (s == null) // sound를 찾지 못한 경우,
        {
            Debug.LogWarning("Sound : " + name + " Not Found!");
            return;
        }
        s.source.Play();
    }

    public void ButtonClickSoundPlay() => Play(ButtonClickSoundName);
    public void PlaySoundPlay() => Play(PlaySoundName);
    public void PlayCompleteSoundPlay() => Play(PlaySoundComplete);
    public void PlaySoundStop()
    {
        SoundItem s = Array.Find(sounds, sound => sound.name == PlaySoundName); // sounds 배열 내 name과 같은 이름을 가진 sound 찾기.

        if (s == null) // sound를 찾지 못한 경우,
        {
            Debug.LogWarning("Sound : " + PlaySoundName + " Not Found!");
            return;
        }
        s.source.Stop();
    }

    public void SetPlayVolume(int volume) => SetVolume(PlaySoundName, volume);
    public void SetButtonVolume(int volume) => SetVolume(ButtonClickSoundName, volume);
    public void SetCompleteVolume(int volume) => SetVolume(PlaySoundComplete, volume);
}