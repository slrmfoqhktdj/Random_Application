using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Collections;

using GoogleMobileAds.Api;

using UnityEngine.Networking;
using HtmlAgilityPack;


public class UIMain : MonoBehaviour
{
    private const string styleResource = "UIMainStyleSheet";

    Button settingButton;
    Button startButton;
    Button saveButton;
    Button sideViewOpenButton;
    VisualElement sideView;
    Label toastMsgLabel;
    Label winningNumberLabel;
    Label winningOrderLabel;


    public static List<Label> LabelList = new List<Label>();

    public static Label BonusNumberLabel;

    public static Action setNumberPanel;
    public static Action showExitDialog;
    public static Action showOverListDialog;
    public static Action<string, Action> showLongClickDialog;
    public static Action setEnabledSaveButton;

    [SerializeField]
    VisualTreeAsset m_ListEntryTemplate;

    bool isStarted = false;

    BallListController ballListController;

    VisualElement root;

    private RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;
    private Popup.Dialog _exitDialog = null;
    private Popup.Dialog _longClickDialog = null;
    private Popup.AlertDialog _overListDialog = null;
    private Popup.SettingDialog _settingDialog = null;

    private float fadeInOutTime = 1f;
    private float toastDurationTime = 2.5f;

    private string apiUrl = "https://www.dhlottery.co.kr/common.do?method=getLottoNumber&drwNo=";

    private void Awake()
    {

        setNumberPanel = () => { SetNumberPanel(); };
        setEnabledSaveButton = () => { saveButton.SetEnabled(true); };
        showExitDialog = () => { ShowExitDialog(); };
        showOverListDialog = () => { ShowOverListDialog(); };
        showLongClickDialog = (msg, confirmedAction) => { ShowLongClickDialog(msg, confirmedAction); };

        //광고 init
        InitAllAd();
    }

    void ApplySafeArea()
    {
        Rect safeAreaRect = Screen.safeArea;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        float left = safeAreaRect.x;
        float right = screenSize.x - (safeAreaRect.x + safeAreaRect.width);
        float top = screenSize.y - (safeAreaRect.y + safeAreaRect.height);
        float bottom = safeAreaRect.y;

        root.style.paddingLeft = left;
        root.style.paddingRight = right;
        root.style.paddingTop = top;
        root.style.paddingBottom = bottom;


        //Debug.Log("ApplySafeArea : " + left + ", " + right + ", " + top + ", " + bottom);
    }

    private void OnEnable()
    {
        // UIDocument 컴포넌트를 가져옵니다.
        var uiDocument = GetComponent<UIDocument>();

        // 루트 요소를 가져옵니다.
        root = uiDocument.rootVisualElement;

        //uss 파일 지정
        root.styleSheets.Add(Resources.Load<StyleSheet>(styleResource));

        //세이프에어리어 적용
        // ApplySafeArea();

        settingButton = root.Q<Button>("SettingButton");
        startButton = root.Q<Button>("StartButton");
        saveButton = root.Q<Button>("SaveButton");
        sideViewOpenButton = root.Q<Button>("SideViewOpenButton");

        sideView = root.Q<VisualElement>("SideView");

        toastMsgLabel = root.Q<Label>("ToastMsgLabel");
        winningNumberLabel = root.Q<Label>("WinningNumber");
        winningOrderLabel = root.Q<Label>("WinningOrder");

        LabelList.Add(root.Q<Label>("NumberLabel1"));
        LabelList.Add(root.Q<Label>("NumberLabel2"));
        LabelList.Add(root.Q<Label>("NumberLabel3"));
        LabelList.Add(root.Q<Label>("NumberLabel4"));
        LabelList.Add(root.Q<Label>("NumberLabel5"));
        LabelList.Add(root.Q<Label>("NumberLabel6"));
        BonusNumberLabel = root.Q<Label>("NumberLabel7");

        settingButton.clicked += SettingButton_clicked;
        startButton.clicked += StartButton_clicked;
        saveButton.clicked += SaveButton_clicked;
        sideViewOpenButton.clicked += SideViewOpenButton_clicked;

        //비활성화
        saveButton.SetEnabled(false);

        //투명화
        toastMsgLabel.style.opacity = 0f;

        //ballListController = new BallListController();

        ballListController = gameObject.AddComponent<BallListController>();
        ballListController.InitializeList(root, m_ListEntryTemplate);

        //InitAds();

        RequestIntersitialAd();

        // StartCoroutine(GetLottoWinningNumber(1075));
        StartCoroutine(GetWebPageContent("https://dhlottery.co.kr/common.do?method=main"));

        System.GC.Collect();

    }


    private void SettingButton_clicked()
    {
        SoundManager.Instance.ButtonClickSoundPlay();

        //설정버튼
        ShowSettingDialog();
    }

    private void StartButton_clicked()
    {

        SoundManager.Instance.ButtonClickSoundPlay();

        //시작버튼
        if (!isStarted)
        {
            SoundManager.Instance.PlaySoundPlay();

            ShowIntersitialAd();
            //ShowAds();

            Wheel.waitAndOpenShield();

            startButton.text = "초기화";
            GlobalVal.WindZoneBool = true;
            //Debug.Log("StartButton!");

            isStarted = true;

            //비활성화
            saveButton.SetEnabled(false);
        }
        //초기화버튼
        else
        {

            SoundManager.Instance.PlaySoundStop();

            startButton.text = "시작";
            GlobalVal.WindZoneBool = false;
            Wheel.resetBallPostion();
            //Debug.Log("ResetButton!");

            Reset_Number();

            isStarted = false;

            //비활성화
            saveButton.SetEnabled(false);
        }
    }

    private void SaveButton_clicked()
    {
        SoundManager.Instance.ButtonClickSoundPlay();

        showToastMessage("번호를 저장했습니다.");

        if (ballListController.AddBallList())
        {
            //초기화
            GlobalVal.getBallNumber = new List<int>();
            GlobalVal.getBallBonusNumber = 0;

            //비활성화
            saveButton.SetEnabled(false);
        }
    }

    private void SideViewOpenButton_clicked()
    {
        SoundManager.Instance.ButtonClickSoundPlay();

        // Debug.Log("SideViewOpenButton_clicked : " + sideView.style.right.value);

        if (sideView.style.right.value == Length.Percent(0))
        {
            sideView.style.right = Length.Percent(-72);
            sideViewOpenButton.text = "<";
        }
        else
        {
            sideView.style.right = Length.Percent(0);
            sideViewOpenButton.text = ">";
        }

    }

    private void Reset_Number()
    {
        for (int i = 0; i < LabelList.Count; i++)
        {
            LabelList[i].text = string.Empty;
        }

        BonusNumberLabel.text = string.Empty;

        //초기화
        GlobalVal.getBallNumber = new List<int>();
        GlobalVal.getBallBonusNumber = 0;
    }

    //숫자 패널에 숫자 적용
    void SetNumberPanel()
    {
        //numberLabel1.text = "하이";
        //LabelList[0].text = "안녕";

        for (int i = 0; i < GlobalVal.getBallNumber.Count; i++)
        {
            LabelList[i].text = GlobalVal.getBallNumber[i].ToString();
            //Debug.Log("UIMain : " + string.Join(",", GlobalVal.getBallNumber));
            //Debug.Log("UIMain : " + GlobalVal.getBallNumber[i]);
        }

        if (GlobalVal.getBallBonusNumber != 0)
        {
            BonusNumberLabel.text = GlobalVal.getBallBonusNumber.ToString();
        }

    }

    //종료 다이어로그 띄우기
    void ShowExitDialog()
    {
        if (_exitDialog != null)
        {
            return;
        }

        if (_longClickDialog != null)
        {
            root.Remove(_longClickDialog);
        }

        if (_overListDialog != null)
        {
            root.Remove(_overListDialog);
        }

        if (_settingDialog != null)
        {
            root.Remove(_settingDialog);
        }

        _exitDialog = new Popup.Dialog();
        root.Add(_exitDialog);
        _exitDialog.setMsg("게임을 종료하시겠습니까?");
        _exitDialog.confirmed += () =>
        {
            //다른 다이어로그 모두 종료
            root.Remove(_exitDialog);
            _exitDialog = null;
            //게임종료
            Application.Quit();
        };
        _exitDialog.canceled += () => { root.Remove(_exitDialog); _exitDialog = null; };
    }

    //롱클릭 다이어로그 띄우기
    void ShowLongClickDialog(string msg, Action confirmedAction)
    {
        if (_longClickDialog != null)
        {
            return;
        }

        _longClickDialog = new Popup.Dialog();
        root.Add(_longClickDialog);
        _longClickDialog.setNumMsg(msg);
        _longClickDialog.setMsg("해당 번호를 삭제하시겠습니까?");

        _longClickDialog.confirmed += () =>
        {
            confirmedAction?.Invoke();
            root.Remove(_longClickDialog);
            _longClickDialog = null;
            showToastMessage("번호를 삭제했습니다.");
        };
        _longClickDialog.canceled += () => { root.Remove(_longClickDialog); _longClickDialog = null; };

    }

    //설정 다이어로그 띄우기
    void ShowSettingDialog()
    {
        if (_settingDialog != null)
        {
            return;
        }

        _settingDialog = new Popup.SettingDialog();
        root.Add(_settingDialog);
        _settingDialog.confirmed += () =>
        {
            root.Remove(_settingDialog);
            _settingDialog = null;
        };
        _settingDialog.canceled += () => { root.Remove(_settingDialog); _settingDialog = null; };
    }



    //저장초과 다이어로그 띄우기
    void ShowOverListDialog()
    {
        if (_overListDialog != null)
        {
            return;
        }

        _overListDialog = new Popup.AlertDialog();
        root.Add(_overListDialog);
        _overListDialog.setMsg("허용저장갯수를 초과하였습니다.");
        _overListDialog.confirmed += () => { root.Remove(_overListDialog); _overListDialog = null; };
    }

    //토스트 메시지 애니메이션 효과
    private IEnumerator fadeInOut(float durationTime, bool inOut)
    {
        float start, end;
        if (inOut)
        {
            start = 0.0f;
            end = 1.0f;
        }
        else
        {
            start = 1.0f;
            end = 0f;
        }

        // Color current = Color.clear; /* (0, 0, 0, 0) = 검은색 글자, 투명도 100% */
        float elapsedTime = 0.0f;

        while (elapsedTime < durationTime)
        {
            float alpha = Mathf.Lerp(start, end, elapsedTime / durationTime);

            toastMsgLabel.style.opacity = alpha;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

    }

    //토스트 메시지 코루틴
    private IEnumerator showToastMessageCoroutine(string msg)
    {
        toastMsgLabel.text = msg;


        yield return fadeInOut(fadeInOutTime, true);

        yield return new WaitForSeconds(toastDurationTime);

        yield return fadeInOut(fadeInOutTime, false);

        toastMsgLabel.text = string.Empty;
    }

    //토스트 메시지 보여주기
    public void showToastMessage(string msg)
    {
        StartCoroutine(showToastMessageCoroutine(msg));
    }

    //동행복권 최신화 숫자 가져오기
    IEnumerator GetWebPageContent(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // 요청 보내기
            yield return request.SendWebRequest();

            // 에러 체크
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("에러: " + request.error);
                winningOrderLabel.text = "당첨번호 가져오기 실패";
            }
            else
            {
                // 성공 - HTML 콘텐츠 가져오기
                string htmlContent = request.downloadHandler.text;
                // Debug.Log("페이지 내용: " + htmlContent);

                // HTML 파싱 작업 수행
                ParseHTMLWithLibrary(htmlContent);
            }
        }
    }

    // HTML 파싱 위치 및 요소 찾기
    void ParseHTMLWithLibrary(string html)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        // CSS 선택자로 요소 찾기
        var nodes = doc.DocumentNode.SelectNodes("//strong[@id='lottoDrwNo']");

        foreach (var node in nodes)
        {
            // Debug.Log(node.InnerText);
            StartCoroutine(GetLottoWinningNumber(node.InnerText.Trim()));
        }
    }
    
    //동행복권 이번주 당첨번호 가져오기
    private IEnumerator GetLottoWinningNumber(string drawNumber)
    {
        string url = apiUrl + drawNumber;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 요청 보내기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                winningOrderLabel.text = $"{drawNumber}회 당첨번호 가져오기 실패";
            }
            else
            {
                // Debug.Log("Response: " + webRequest.downloadHandler.text);

                // 응답 데이터 처리
                WinnerNumberJsonClass jsonResponse = JsonUtility.FromJson<WinnerNumberJsonClass>(webRequest.downloadHandler.text);
                // Debug.Log("Response: " + jsonResponse.drwtNo1 + ", " + jsonResponse.drwtNo2 + ", " + jsonResponse.drwtNo3 + ", " + jsonResponse.drwtNo4 + ", " + jsonResponse.drwtNo5 + ", " + jsonResponse.drwtNo6 + ", Bonus: " + jsonResponse.bnusNo);
                winningNumberLabel.text = $"{jsonResponse.drwtNo1}, {jsonResponse.drwtNo2}, {jsonResponse.drwtNo3}, {jsonResponse.drwtNo4}, {jsonResponse.drwtNo5}, {jsonResponse.drwtNo6} + {jsonResponse.bnusNo}";
                winningOrderLabel.text = $"{drawNumber}회 당첨번호";

                ballListController.ColorMatchNumber(new List<int>
                {
                    jsonResponse.drwtNo1,
                    jsonResponse.drwtNo2,
                    jsonResponse.drwtNo3,
                    jsonResponse.drwtNo4,
                    jsonResponse.drwtNo5,
                    jsonResponse.drwtNo6
                });
            }
        }
    }

    //광고 sdk 초기화
    public void InitAllAd()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {

        });
    }

    //광고 초기화 함수
    public void InitAds()
    {

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        string adUnitId;

#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        adUnitId = "unexpected_platform";
#endif

        RewardedAd.Load(adUnitId, new AdRequest(), LoadCallback);
    }

    //로드 콜백 함수
    public void LoadCallback(RewardedAd rewardedAd, LoadAdError loadAdError)
    {
        if (rewardedAd != null)
        {
            this.rewardedAd = rewardedAd;
            //Debug.Log("AD로드성공");
        }
        else
        {
            Debug.Log(loadAdError.GetMessage());
        }

    }

    //광고 보여주는 함수
    public void ShowAds()
    {

        //광고 횟수 점검
        //if (PlayerPrefsManager.Instance.GetADCount() <= GlobalVal.getADCount)
        if (PlayerPrefsManager.GetADCount() <= GlobalVal.getADCount)
        {
            return;
        }

        if (rewardedAd.CanShowAd())
        {
            rewardedAd.Show(GetReward);
            //PlayerPrefsManager.Instance.SetADCount(0);
            PlayerPrefsManager.SetADCount(0);
        }
        else
        {
            Debug.Log("광고 재생 실패");
        }
    }

    //보상 함수
    public void GetReward(Reward reward)
    {
        //messageBox.SetActive(true);
        //gold += (int)reward.Amount;
        //gold_txt.text = "골드 : " + gold.ToString();
        InitAds();
    }


    //전면광고 로드
    public void RequestIntersitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        string adUnitId;

#if UNITY_ANDROID
        adUnitId = "ca-app-pub-1383688319555916/8859730620";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/4411468910;
#else
        adUnitId = "unexpected_platform";
#endif

        InterstitialAd.Load(adUnitId, new AdRequest(), IntersitialLoadCallback);
    }

    //전면광고 로드 콜백
    public void IntersitialLoadCallback(InterstitialAd interstitialAd, LoadAdError loadAdError)
    {
        // If the load failed, we log the error and abort.
        if (loadAdError != null || interstitialAd == null)
        {
            Debug.LogError("interstitial ad failed to load an ad " + "with error : " + loadAdError);

        }
        else
        {
            this.interstitialAd = interstitialAd;

            //광고가 열릴때
            this.interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                //플레이음 정지
                SoundManager.Instance.PlaySoundStop();
            };

            //광고가 닫힐때
            this.interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                //광고 다시 로드
                RequestIntersitialAd();
                SoundManager.Instance.PlaySoundPlay();
            };
        }

    }

    //전면광고 보여주기
    public void ShowIntersitialAd()
    {
        //광고 횟수 점검
        //if (PlayerPrefsManager.Instance.GetADCount() <= GlobalVal.getADCount)
        if (PlayerPrefsManager.GetADCount() <= GlobalVal.getADCount)
        {
            return;
        }

        if (interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            //PlayerPrefsManager.Instance.SetADCount(0);
            PlayerPrefsManager.SetADCount(0);
        }
        else
        {
            Debug.Log("광고 재생 실패");
        }
    }


}
