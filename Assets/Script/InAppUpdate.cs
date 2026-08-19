using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Google.Play.AppUpdate;
using Google.Play.Common;

public class InAppUpdate : MonoBehaviour
{
    private AppUpdateManager appUpdateManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("InAppUpdate Start");

#if UNITY_EDITOR
#else
        StartCoroutine(CheckForUpdate());
#endif
    }

    IEnumerator CheckForUpdate()
    {
        appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), ... and decide whether to ask the user
            // to start an in-app update.

            //업데이트가 가능한상태 
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                //즉시 업데이트
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                yield return startUpdateRequest;
            }
            //업데이트가 없는경우
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                Debug.Log("업데이트 없음");
                yield return (int)UpdateAvailability.UpdateNotAvailable;
            }
            //업데이트 확인 불가
            else
            {
                Debug.Log("정보 없음");
                yield return (int)UpdateAvailability.Unknown;
            }
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
            Debug.Log("InAppUpdate Error: " + appUpdateInfoOperation.Error.ToString());
        }
    }
}
