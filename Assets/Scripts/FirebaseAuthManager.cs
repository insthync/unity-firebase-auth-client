using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;

public class FirebaseAuthManager
{
    private string serviceUrl;
    private MonoBehaviour targetBehaviour;
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseUserResult currentUser;
    private FirebaseUserTokenResult currentToken;
    private Dictionary<string, object> stsTokenManager;

    public FirebaseUserResult CurrentUser
    {
        get { return currentUser; }
    }

    public FirebaseAuthManager(string url, MonoBehaviour behaviour, FirebaseAuth firebaseAuth)
    {
        serviceUrl = url;
        targetBehaviour = behaviour;
        auth = firebaseAuth;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
        currentUser = null;
    }

    // Track state changes of the auth object.
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
        if (!signedIn && user != null)
        {
            Debug.Log("Signed out " + user.UserId);
        }
        user = auth.CurrentUser;
        if (currentUser != null)
        {
            currentUser = FirebaseUserResult.CreateResultByUser(user);
        }
        else
        {
            currentUser = null;
        }
        if (signedIn)
        {
            Debug.Log("Signed in " + user.UserId);
        }
    }

    public bool IsAuthenticated()
    {
        return currentUser != null;
    }

    public void CreateUserWithEmailAndPassword(string email, string password, Action<FirebaseUserResult> callback)
    {
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
        {
            targetBehaviour.StartCoroutine(ProcessFirebaseUserTaskRoutine(auth.CreateUserWithEmailAndPasswordAsync(email, password), callback));
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            WWW www = new WWW(serviceUrl + "/createUserWithEmailAndPassword", form);
            targetBehaviour.StartCoroutine(ProcessWebServiceUserRoutine(www, callback));
        }
    }

    public void SignInWithEmailAndPassword(string email, string password, Action<FirebaseUserResult> callback)
    {
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
        {
            targetBehaviour.StartCoroutine(ProcessFirebaseUserTaskRoutine(auth.SignInWithEmailAndPasswordAsync(email, password), callback));
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            WWW www = new WWW(serviceUrl + "/signInWithEmailAndPassword", form);
            targetBehaviour.StartCoroutine(ProcessWebServiceUserRoutine(www, callback));
        }
    }

    public void SignInWithFacebook(string accessToken, Action<FirebaseUserResult> callback)
    {
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
        {
            Credential credential = FacebookAuthProvider.GetCredential(accessToken);
            targetBehaviour.StartCoroutine(ProcessFirebaseUserTaskRoutine(auth.SignInWithCredentialAsync(credential), callback));
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("access_token", accessToken);
            WWW www = new WWW(serviceUrl + "/signInWithFacebook", form);
            targetBehaviour.StartCoroutine(ProcessWebServiceUserRoutine(www, callback));
        }
    }

    public void SignInWithGithub(string accessToken, Action<FirebaseUserResult> callback)
    {
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
        {
            Credential credential = GitHubAuthProvider.GetCredential(accessToken);
            targetBehaviour.StartCoroutine(ProcessFirebaseUserTaskRoutine(auth.SignInWithCredentialAsync(credential), callback));
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("access_token", accessToken);
            WWW www = new WWW(serviceUrl + "/signInWithGithub", form);
            targetBehaviour.StartCoroutine(ProcessWebServiceUserRoutine(www, callback));
        }
    }

    public void SignInWithGoogle(string idToken, string accessToken, Action<FirebaseUserResult> callback)
    {
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            targetBehaviour.StartCoroutine(ProcessFirebaseUserTaskRoutine(auth.SignInWithCredentialAsync(credential), callback));
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("id_token", idToken);
            form.AddField("access_token", accessToken);
            WWW www = new WWW(serviceUrl + "/signInWithGoogle", form);
            targetBehaviour.StartCoroutine(ProcessWebServiceUserRoutine(www, callback));
        }
    }

    public void SignInWithTwitter(string token, string secret, Action<FirebaseUserResult> callback)
    {
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
        {
            Credential credential = TwitterAuthProvider.GetCredential(token, secret);
            targetBehaviour.StartCoroutine(ProcessFirebaseUserTaskRoutine(auth.SignInWithCredentialAsync(credential), callback));
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("token", token);
            form.AddField("secret", secret);
            WWW www = new WWW(serviceUrl + "/signInWithTwitter", form);
            targetBehaviour.StartCoroutine(ProcessWebServiceUserRoutine(www, callback));
        }
    }

    public void SignInAnonymously(Action<FirebaseUserResult> callback)
    {
        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor)
        {
            targetBehaviour.StartCoroutine(ProcessFirebaseUserTaskRoutine(auth.SignInAnonymouslyAsync(), callback));
        }
        else
        {
            WWWForm form = new WWWForm();
            WWW www = new WWW(serviceUrl + "/signInAnonymously", form);
            targetBehaviour.StartCoroutine(ProcessWebServiceUserRoutine(www, callback));
        }
    }

    public void GetAccessToken(bool forceRefresh, Action<FirebaseUserTokenResult> callback)
    {
        if (!forceRefresh && currentToken != null)
        {
            if (callback != null)
                callback(currentToken);
            return;
        }

        if (user != null)
        {
            targetBehaviour.StartCoroutine(ProcessFirebaseUserTokenTaskRoutine(user.TokenAsync(forceRefresh), callback));
        }
        else
        {
            if (callback != null)
                callback(currentToken);
        }
    }

    public void SignOut()
    {
        if (auth != null)
            auth.SignOut();

        currentUser = null;
        currentToken = null;
        stsTokenManager = null;
    }

    public IEnumerator ProcessWebServiceUserRoutine(WWW www, Action<FirebaseUserResult> callback)
    {
        yield return www;

        if (callback != null)
        {
            if (!string.IsNullOrEmpty(www.error))
            {
                currentUser = null;
                currentToken = null;
                stsTokenManager = null;
                callback(FirebaseUserResult.CreateFaultedResult(new Exception(www.error)));
            }
            else
            {
                // Parsing json
                var result = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                stsTokenManager = result["stsTokenManager"] as Dictionary<string, object>;
                currentUser = FirebaseUserResult.CreateSuccessResult(result.GetString("uid"), result.GetString("displayName"), result.GetString("photoURL"), result.GetString("email"), result.GetBool("emailVerified"), result.GetBool("isAnonymous"));
                currentToken = FirebaseUserTokenResult.CreateSuccessResult(stsTokenManager.GetString("accessToken"));
                callback(currentUser);
            }
        }
    }

    public IEnumerator ProcessFirebaseUserTaskRoutine(Task<FirebaseUser> t, Action<FirebaseUserResult> callback)
    {
        while (!t.IsCompleted)
            yield return null;

        if (callback != null)
        {
            if (t.IsFaulted)
                callback(FirebaseUserResult.CreateFaultedResult(t.Exception));
            else if (t.IsCanceled)
                callback(FirebaseUserResult.CreateCanceledResult());
            else
            {
                currentUser = FirebaseUserResult.CreateResultByUser(user);
                callback(currentUser);
            }
        }
    }

    public IEnumerator ProcessFirebaseUserTokenTaskRoutine(Task<string> t, Action<FirebaseUserTokenResult> callback)
    {
        while (!t.IsCompleted)
            yield return null;

        if (callback != null)
        {
            if (t.IsFaulted)
                callback(FirebaseUserTokenResult.CreateFaultedResult(t.Exception));
            else if (t.IsCanceled)
                callback(FirebaseUserTokenResult.CreateCanceledResult());
            else
            {
                currentToken = FirebaseUserTokenResult.CreateSuccessResult(t.Result);
                callback(currentToken);
            }
        }
    }
}
