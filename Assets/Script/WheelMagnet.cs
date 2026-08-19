using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WheelMagnet : MonoBehaviour
{

    /*public static Action releaseObj;*/

    public GameObject hitObj = null;

    public bool catchedObj = true;

    [SerializeField]
    float _StartRotationDelay = 1.5f;

    IEnumerator _WaitAndStartRotationIEnumerator;

    void Start()
    {
        _WaitAndStartRotationIEnumerator = WaitAndStartRotation();
    }

    void FixedUpdate()
    {
        /*if (hitObj != null)
        {
            //hitObj.transform.position = transform.position;
            //hitObj.transform.parent = this.gameObject.transform;
            //hitObj.GetComponent<Rigidbody>().MovePosition(transform.position);
            hitObj.GetComponent<Rigidbody>().position = transform.position;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hitObj != null || !other.gameObject.CompareTag("Ball") || !catchedObj || GlobalVal.getBallCount > (GlobalVal.getGoalBallCount - 1))
        {
            return;   
        }

        hitObj = other.gameObject;

        GlobalVal.getBallCount += 1;

        if (hitObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        //hitObj.GetComponent<Rigidbody>().useGravity = false;
        //hitObj.GetComponent<Rigidbody>().isKinematic = true;

        FixedJoint joint = hitObj.AddComponent<FixedJoint>();

        if (this.gameObject.TryGetComponent<Rigidbody>(out Rigidbody tb))
        {
            joint.connectedBody = tb;
        }
        //joint.connectedBody = this.gameObject.GetComponent<Rigidbody>();

        //잡힌 공 번호 저장
        int number = int.Parse(hitObj.name.Replace("b", ""));

        if (GlobalVal.getBallCount < 7)
        {
            //전체 변수 저장
            GlobalVal.getBallNumber.Add(number);
        }
        else
        {
            GlobalVal.getBallBonusNumber = number;
            //공 뽑기 종료 시점
            //세이브 버튼 활성화 패널에 반영
            UIMain.setEnabledSaveButton();

            //배경음 정지
            SoundManager.Instance.PlaySoundStop();
            //완료음 재생
            SoundManager.Instance.PlayCompleteSoundPlay();

            //광고횟수 카운트
            //PlayerPrefsManager.Instance.SetIncreaseADCount();
            PlayerPrefsManager.SetIncreaseADCount();
        }
        
        //Debug.Log("WheelMagnet : " + string.Join(",", GlobalVal.getBallNumber));

        //패널에 반영
        UIMain.setNumberPanel();


        //딜레이
        //Debug.Log("WaitAndStartRotation() " + GlobalVal.getBallCount + " " + (_WaitAndStartRotationIEnumerator != null));
        //완료된 코루틴은 재사용 x, 재할당
        _WaitAndStartRotationIEnumerator = WaitAndStartRotation();
        StartCoroutine(_WaitAndStartRotationIEnumerator);
        

        //Wheel.startRotation();

    }

    private void OnTriggerStay(Collider other)
    {
        if(hitObj == null || !catchedObj)
        {
            return;
        }

        if (hitObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.MovePosition(transform.position);
        }
        //hitObj.GetComponent<Rigidbody>().MovePosition(transform.position);

    }


    public void ReleaseObject()
    {
        //Debug.Log("ReleaseObject() "+ GlobalVal.getBallCount + " "+ (GlobalVal.getBallCount > (GlobalVal.getGoalBallCount - 1) && GlobalVal.getBallCount < (GlobalVal.getGoalBallCount + 1)));

        StartCoroutine(ReleaseSettings());

        StartCoroutine(ReleaseObjectForce());

        StartCoroutine(ReleaseObjectReset());

        if (GlobalVal.getBallCount > (GlobalVal.getGoalBallCount -1) && GlobalVal.getBallCount < (GlobalVal.getGoalBallCount + 1))
        {
            //Debug.Log("ReleaseObject() : " + GlobalVal.getBallCount + " StartRotation()");

            //딜레이
            //완료된 코루틴은 재사용 x, 재할당
            _WaitAndStartRotationIEnumerator = WaitAndStartRotation();
            StartCoroutine(_WaitAndStartRotationIEnumerator);
            GlobalVal.getBallCount += 1;
        }


        //종료 시점
        if(GlobalVal.getBallCount == (GlobalVal.getGoalBallCount + 1))
        {
            GlobalVal.WindZoneBool = false;
            //Wheel.resetBallPostion();
        }

        //공 뽑기 종료 시점
        /*if (GlobalVal.getBallNumber.Count == (GlobalVal.getGoalBallCount - 1))
        {
            //세이브 버튼 활성화 패널에 반영
            UIMain.setEnabledSaveButton();
        }*/

        //Debug.Log("WheelMagnet : " + GlobalVal.getBallNumber.Count);

    }

    IEnumerator ReleaseSettings()
    {

        if(hitObj != null)
        {
            if (hitObj.TryGetComponent<FixedJoint>(out FixedJoint rb))
            {
                Destroy(rb);
            }
            //Destroy(hitObj.GetComponent<FixedJoint>());
        }

        yield return new WaitForFixedUpdate();

        if (hitObj != null)
        {

            if (hitObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                //rb.linearVelocity = Vector3.zero;
                //rb.angularVelocity = Vector3.zero;
            }

            // hitObj.GetComponent<Rigidbody>().isKinematic = false;
            // hitObj.GetComponent<Rigidbody>().useGravity = true;

            // hitObj.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            // hitObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;


            catchedObj = false;
        }
        
        
    }


    IEnumerator WaitAndStartRotation()
    {
        //Debug.Log("WaitAndStartRotation() " + GlobalVal.getBallCount);
        yield return new WaitForSeconds(_StartRotationDelay);
        Wheel.startRotation();
    }

    IEnumerator ReleaseObjectForce()
    {
        yield return new WaitForSeconds(0.2f);
        ReleaseForce.releaseForceTrue();
    }

    IEnumerator ReleaseObjectReset()
    {
        yield return new WaitForSeconds(1.0f);
        catchedObj = true;
        hitObj = null;
        ReleaseForce.releaseForceFalse();
    }


    public void StopCoroutines()
    {

        //Debug.Log("StopCoroutines() "+ GlobalVal.getBallCount);
        StopCoroutine(_WaitAndStartRotationIEnumerator);
        catchedObj = true;
        hitObj = null;
        //완료된 코루틴은 재사용 x, 재할당
        _WaitAndStartRotationIEnumerator = WaitAndStartRotation();
    }
}
