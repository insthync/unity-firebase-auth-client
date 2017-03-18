using System;
using Firebase.Auth;

[Serializable]
public class FirebaseUserResult
{
    public string UserId { get; protected set; }
    public string DisplayName { get; protected set; }
    public Uri PhotoUrl { get; protected set; }
    public string Email { get; protected set; }
    public bool IsEmailVerified { get; protected set; }
    public bool IsAnonymous { get; protected set; }
    public bool IsFaulted { get; protected set; }
    public bool IsCanceled { get; protected set; }
    public Exception Exception { get; protected set; }

    public static FirebaseUserResult CreateResultByUser(FirebaseUser user)
    {
        FirebaseUserResult result = new FirebaseUserResult();
        result.UserId = user.UserId;
        result.DisplayName = user.DisplayName;
        result.PhotoUrl = user.PhotoUrl;
        result.Email = user.Email;
        result.IsEmailVerified = user.IsEmailVerified;
        result.IsAnonymous = user.IsAnonymous;
        return result;
    }

    public static FirebaseUserResult CreateSuccessResult(string uid, string displayName, string photoUrl, string email, bool isEmailVerified, bool isAnonymous)
    {
        FirebaseUserResult result = new FirebaseUserResult();
        result.UserId = uid;
        result.DisplayName = displayName;
        result.PhotoUrl = string.IsNullOrEmpty(photoUrl) ? null : new Uri(photoUrl);
        result.Email = email;
        result.IsEmailVerified = isEmailVerified;
        result.IsAnonymous = isAnonymous;
        return result;
    }

    public static FirebaseUserResult CreateFaultedResult(Exception exception)
    {
        FirebaseUserResult result = new FirebaseUserResult();
        result.IsFaulted = true;
        result.Exception = exception;
        return result;
    }

    public static FirebaseUserResult CreateCanceledResult()
    {
        FirebaseUserResult result = new FirebaseUserResult();
        result.IsCanceled = true;
        return result;
    }
}
