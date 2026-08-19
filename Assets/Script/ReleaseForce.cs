using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReleaseForce : MonoBehaviour
{
    [SerializeField]
    float _WindForce = 30f;

    bool _run = false;

    public static Action releaseForceTrue;
    public static Action releaseForceFalse;

    private void Awake()
    {
        releaseForceTrue = () => { ReleaseForceTrue(); };
        releaseForceFalse = () => { ReleaseForceFalse(); };
    }


    private void OnTriggerStay(Collider other)
    {

        if (!other.gameObject.CompareTag("Ball"))
        {
            return;
        }

        if (!_run)
        {
            return;
        }

        var hitObj = other.gameObject;
        if(hitObj != null)
        {
            if (hitObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                var dir = transform.forward;
                var randomWindForce = _WindForce;
                rb.AddForce(dir * randomWindForce);
            }
            //var rb = hitObj.GetComponent<Rigidbody>();
            //var dir = transform.forward;
            //var randomWindForce = _WindForce;
            //rb.AddForce(dir * randomWindForce);
            //rb.AddForce(dir * _windForce);
            //rb.AddForce(dir * 10.7f);
        }
    }

    void ReleaseForceTrue()
    {
        //Debug.Log("ReleaseForceTrue() = " + this.name);
        _run = true;
    }

    void ReleaseForceFalse()
    {
        //Debug.Log("ReleaseForceFalse() = " + this.name);
        _run = false;
    }
}
