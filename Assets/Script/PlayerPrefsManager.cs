using UnityEngine;
using System;
using System.Collections.Generic;

//public class PlayerPrefsManager : MonoBehaviour
public class PlayerPrefsManager
{
    // private static PlayerPrefsManager instance;
    // public static PlayerPrefsManager Instance
    // {
    //     get
    //     {
    //         if (instance == null)
    //         {
    //             GameObject go = new GameObject("PlayerPrefsManager");
    //             instance = go.AddComponent<PlayerPrefsManager>();
    //             DontDestroyOnLoad(go);
    //         }
    //         return instance;
    //     }
    // }

    // 키 상수들 (오타 방지)
    public const string AD_COUNT = "AD_COUNT";
    public const string SD_BUTTON = "SD_BUTTON";
    public const string SD_PLAY = "SD_PLAY";

    // String 저장/로드
    public void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    // Int 저장/로드
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetIncreaseInt(string key)
    {
        PlayerPrefs.SetInt(key, GetInt(key) + 1);
        PlayerPrefs.Save();
    }

    // Float 저장/로드
    public void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    // Bool 저장/로드 (Int로 변환)
    public void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    // 키 존재 여부 확인
    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    // 키 삭제
    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    // 모든 데이터 삭제
    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    // 편의 메소드들
    public static void SetADCount(int count) => SetInt(AD_COUNT, count);
    public static void SetIncreaseADCount() => SetIncreaseInt(AD_COUNT);
    public static int GetADCount() => GetInt(AD_COUNT, 0);

    public static void SetSDButton(int volume) => SetInt(SD_BUTTON, volume);
    public static int GetSDButton() => GetInt(SD_BUTTON, 5);

    public static void SetSDPlay(int volume) => SetInt(SD_PLAY, volume);
    public static int GetSDPlay() => GetInt(SD_PLAY, 5);

}