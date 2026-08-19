using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //안드로이드인 경우
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape)) //뒤로가기 키 입력
            {

                //종료 다이어로그 불러오기
                UIMain.showExitDialog();
            }

        }
        
    }
}
