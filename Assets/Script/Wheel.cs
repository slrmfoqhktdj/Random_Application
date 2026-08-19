using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public static Action startRotation;
    public static Action resetBallPostion;
    public static Action waitAndOpenShield;

    [SerializeField]
    float _RotationSpeed = 40f;

    [SerializeField]
    GameObject[] _Shields = new GameObject[] { };

    [SerializeField]
    GameObject[] _Magnets = new GameObject[] { };

    [SerializeField]
    GameObject _BasicBall;

    [SerializeField]
    Camera _mainCamera;

    [SerializeField]
    Camera _topCamera;

    [SerializeField]
    Camera _bottomCamera;

    [SerializeField]
    Camera _resultCamera;


    private bool isRotating = false;
    //private int[] targetZ = new int[] {0, 90, 0, 270};
    private int[] targetZ = new int[] {270, 180, 90, 0 };
    private int isRotatingCount = 0;

    //리스폰 지점 리스트
    private List<GameObject> rpList = new List<GameObject>();

    //리스폰된 공들
    private List<GameObject> sponeBalls = new List<GameObject>();

    //초반 입구 막기 코루틴
    IEnumerator _WaitAndOpenShieldIEnumerator;

    private void Awake()
    {
        startRotation = () => { StartRotation(); };
        resetBallPostion = () => { ResetBallPostion(); };
        waitAndOpenShield = () => { _WaitAndOpenShieldIEnumerator = WaitAndOpenShield();  StartCoroutine(_WaitAndOpenShieldIEnumerator); };
    }

    // Start is called before the first frame update
    void Start()
    {
        //Rotation();
        //StartRotation();

        //
        //_Shields = new GameObject[] { GameObject.Find("Shield01"), GameObject.Find("Shield02"), GameObject.Find("Shield03"), GameObject.Find("Shield04") };

        //리스폰 포인트 오브젝트 찾기
        GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        //배열을 리스트로
        rpList = new List<GameObject>(respawnPoints);

        ShuffleList(rpList);

        StartCoroutine(SpawnBallsGradually());

        _topCamera.enabled = false;
        _bottomCamera.enabled = false;
        _resultCamera.enabled = false;

        _WaitAndOpenShieldIEnumerator = WaitAndOpenShield();
    }

    // 공 생성 코루틴
    IEnumerator SpawnBallsGradually()
    {
        
        int object_name_count = 0;

        foreach (GameObject point in rpList)
        {
            GameObject ObjactBall = Instantiate(_BasicBall, point.transform.position, Quaternion.identity);
            object_name_count++;
            ObjactBall.name = $"b{object_name_count}";

            //텍스쳐 입히기
            Texture2D tx = Resources.Load<Texture2D>($"b{object_name_count}");

            if (ObjactBall.TryGetComponent<Renderer>(out Renderer rd))
            {
                rd.material.SetTexture("_MainTex", tx);
            }

            //ObjactBall.GetComponent<Renderer>().material.SetTexture("_MainTex", tx);

            sponeBalls.Add(ObjactBall);

            // 2-3개마다 한 프레임 대기
            if (object_name_count % 2 == 0) yield return null;
        }
    }

    //시작시 공바로 들어가는 것 방지
    IEnumerator WaitAndOpenShield()
    {
        yield return new WaitForSeconds(GlobalVal.getShieldWaitTime);
        _Shields[0].SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isRotating)
        {
            return;
        }

        //Debug.Log("x = " + Mathf.Abs(transform.eulerAngles.x));
        //Debug.Log("y = " + Mathf.Abs(transform.eulerAngles.y));
        //Debug.Log("z = " + Mathf.Abs(transform.eulerAngles.z));

        transform.Rotate(new Vector3(0f, 0f, -_RotationSpeed) * Time.deltaTime);

        //x += Time.deltaTime * 20f;
        //transform.rotation = Quaternion.Euler(90, 0, x);

        //Debug.Log("Wheel : transform.eulerAngles.x = " + transform.eulerAngles.x + " targetZ[isRotatingCount % 4 = "+ isRotatingCount +", "+ targetZ[isRotatingCount % 4]);

        if (Mathf.Abs(transform.eulerAngles.z - targetZ[isRotatingCount % 4]) < 0.03f)
        {
            isRotating = false;

            //Debug.Log("Wheel : FixedUpdate() = " + GlobalVal.getBallCount);

            if (isRotatingCount > 0)
            {
                ReleaseObjCommand();
            }

            isRotatingCount += 1;
        }


    }

    void StartRotation()
    {
        /*targetZ = transform.eulerAngles.z + 90;

        if (transform.eulerAngles.z > 265)
        {
            targetZ = 0;
        }*/

        //Debug.Log("StartRotation() "+ GlobalVal.getBallCount);

        //Debug.Log("current x rotation = "+transform.eulerAngles.x + ", target z rotation = " + targetZ[isRotatingCount % 4]);
        isRotating = true;

        //Debug.Log("Euler Angles: " + transform.rotation.eulerAngles);


        if (isRotatingCount == 1)
        {
            _topCamera.enabled = true;
        }
        else if (isRotatingCount == 3)
        {
            _bottomCamera.enabled = true;
        }
        else if (isRotatingCount == 5)
        {
            _resultCamera.enabled = true;
        }

        //_topCamera.enabled = true;
        //_bottomCamera.enabled = true;
        //_resultCamera.enabled = true;
        //_mainCamera.enabled = false;
    }

    void ReleaseObjCommand()
    {
        //Transform r_obj = transform.GetChild((isRotatingCount-1)%4);

        //Debug.Log("Wheel : releaseObjCommand = " + GlobalVal.getBallCount);

        //WheelMagnet childScript = r_obj.GetComponentInChildren<WheelMagnet>();

        WheelMagnet childScript = transform.GetChild((isRotatingCount - 1) % 4).GetComponent<WheelMagnet>();
        childScript.ReleaseObject();

        //공 입구 막기
        if (GlobalVal.getBallCount > (GlobalVal.getGoalBallCount - 3))
        {
            //Debug.Log("Wheel : releaseObjCommand = " + ((isRotatingCount - 1) % 4));
            _Shields[((isRotatingCount - 1) % 4)].SetActive(true);
        }
    }

    // Fisher-Yates 셔플 알고리즘 (가장 권장되는 방법)
    public void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void ResetBallPostion()
    {
        isRotatingCount = 0;
        GlobalVal.getBallCount = 0;

        isRotating = false;

        //입구 막기 켜짐
        // foreach (GameObject shield in _Shields)
        // {
        //     shield.SetActive(false);
        // }
        
        StopCoroutine(_WaitAndOpenShieldIEnumerator);
        _WaitAndOpenShieldIEnumerator = WaitAndOpenShield();
        
        for (int i = 0; i < _Shields.Length; i++)
        {
            if (i == 0) _Shields[i].SetActive(true);
            else
                _Shields[i].SetActive(false);
        }

        //자석 함수 설정 초기화
        foreach (GameObject magnet in _Magnets)
        {

            if (magnet.TryGetComponent<WheelMagnet>(out WheelMagnet wm))
            {
                //진행중인 회전 코루틴 정지 (공이 막 들어갔을때에 초기화 버튼 누를시)
                wm.StopCoroutines();
            }
            //WheelMagnet wmScript = magnet.GetComponent<WheelMagnet>();
            //진행중인 회전 코루틴 정지 (공이 막 들어갔을때에 초기화 버튼 누를시)
            //wmScript.StopCoroutines();

        }

        //포인터 셔플
        ShuffleList(rpList);
        
        for (int i = 0; i < rpList.Count; i++)
        {
            if (sponeBalls[i].TryGetComponent(out FixedJoint fj))
            {
                Destroy(fj);
            }
            // else
            // {
            //     Debug.Log("FixedJoint 제자리로 돌아가지 못함 "+ i.ToString());
            // }

            if (sponeBalls[i].TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.position = rpList[i].transform.position;     //trasform.position이동시 물리연산과 충돌이나 제대로 이동이 안됨
                rb.GetComponent<Rigidbody>().isKinematic = false;
                rb.GetComponent<Rigidbody>().useGravity = true;
            }
            
        }

        if (this.TryGetComponent(out Rigidbody rby))
        {
            rby.rotation = Quaternion.Euler(0,180,0);
        }

        _topCamera.enabled = false;
        _bottomCamera.enabled = false;
        _resultCamera.enabled = false;
    }

    void OnApplicationQuit()
    {
        //Debug.Log("게임 종료 중...");

        foreach (GameObject obj in rpList)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        foreach (GameObject obj in sponeBalls)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

    }
}
