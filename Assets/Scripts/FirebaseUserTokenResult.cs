using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FirebaseUserTokenResult
{
    public string AccessToken { get; protected set; }
    public bool IsFaulted { get; protected set; }
    public bool IsCanceled { get; protected set; }
    public Exception Exception { get; protected set; }

    public static FirebaseUserTokenResult CreateSuccessResult(string accessToken)
    {
        FirebaseUserTokenResult result = new FirebaseUserTokenResult();
        result.AccessToken = accessToken;
        return result;
    }

    public static FirebaseUserTokenResult CreateFaultedResult(Exception exception)
    {
        FirebaseUserTokenResult result = new FirebaseUserTokenResult();
        result.IsFaulted = true;
        result.Exception = exception;
        return result;
    }

    public static FirebaseUserTokenResult CreateCanceledResult()
    {
        FirebaseUserTokenResult result = new FirebaseUserTokenResult();
        result.IsCanceled = true;
        return result;
    }
}
