using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

// 웹 크롤링을 하기위한 보안 정책 우회 클래스
public class UnsafeSecurityPolicy
{
    public static bool Validator(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors policyErrors)
    {
        Debug.Log("Validation successful!");
        return true;
    }

    public static void Instate()
    {
        ServicePointManager.ServerCertificateValidationCallback = Validator;
    }
}