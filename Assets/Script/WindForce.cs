using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindForce : MonoBehaviour
{
    //[SerializeField]
    //float _minWindForce = 10f;
    [SerializeField]
    float _maxWindForce = 60f;


    private void OnTriggerStay(Collider other)
    {
        if (!GlobalVal.WindZoneBool)
        {
            return;
        }

        if (!other.gameObject.CompareTag("Ball"))
        {
            return;
        }

        var hitObj = other.gameObject;
        if(hitObj != null)
        {
            if (hitObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                //var rb = hitObj.GetComponent<Rigidbody>();
                var dir = transform.up;
                //var randomWindForce = Random.Range(_minWindForce, _maxWindForce);
                rb.AddForce(dir *  _maxWindForce);    
            } 
            //rb.AddForce(dir * _windForce);
            //rb.AddForce(dir * 10.7f);
        }
    }
}
