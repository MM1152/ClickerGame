using Cysharp.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance;
    public static AuthManager Instance => instance;

    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private bool isInitalized = false;

    public FirebaseUser CureentUser => currentUser;
    public bool IsLoggedIn => currentUser != null;
    public string UserId => currentUser?.UserId ?? string.Empty;
    public bool IsInitalized => isInitalized;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private async UniTaskVoid Start()
    {
        await FirebaseInitalizer.Instance.WaitForInitalizationAsync();

        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += OnAuthStateChanged;

        currentUser = auth.CurrentUser;

        if(currentUser != null)
        {
            Debug.Log($"[Auth] 이미 로그인됨: {UserId}"); // 유출되면 안됨
        }
        else
        {
            Debug.Log($"[Auth] 로그인이 필요함");
        }

        isInitalized = true;
    }

    private void OnDestroy()
    {
        if(auth != null)
        {
            auth.StateChanged -= OnAuthStateChanged;
        }
    }

    public async UniTask<(bool success , string error)> SignInAnonymouslyAsync()
    {
        try
        {
            Debug.Log($"[Auth] 익명 로그인 시도...");

            AuthResult result = await auth.SignInAnonymouslyAsync().AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] 익명 로그인 성공 {UserId}");
            return (true, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] 익명 로그인 실패: {ex.Message}");
            return (false, ex.Message);
        }
    }
    public async UniTask<(bool success, string error)> CreateUserWithEmailAsync(string email , string password)
    {
        try
        {
            Debug.Log($"[Auth] 회원가입 시도...");

            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password).AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] 회원가입 성공 {UserId}");
            return (true, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] 회원가입 실패: {ex.Message}");
            return (false, ex.Message);
        }
    }
    public async UniTask<(bool success, string error)> SignInWithEamilAsync(string email , string password)
    {
        try
        {
            Debug.Log($"[Auth] 로그인 시도...");

            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password).AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] 로그인 성공 {UserId}");
            return (true, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] 로그인 실패: {ex.Message}");
            return (false, ex.Message);
        }
    }

    public void SignOut()
    {
        if(auth != null && currentUser != null)
        {
            Debug.Log($"[Auth] 로그아웃");
            auth.SignOut();
            currentUser = null;
        }
    }

    private void OnAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if(auth.CurrentUser != currentUser)
        {
            bool signedIn = auth.CurrentUser != currentUser && auth.CurrentUser != null;

            if(!signedIn && currentUser != null)
            {
                Debug.Log($"[Auth] 로그아웃 됨");
            }
            else if(signedIn)
            {
                Debug.Log($"[Auth] 로그인 됨");
            }

            currentUser = auth.CurrentUser;
        }
    }

    private string ParseFairebaseError(string error)
    {
        return "";
    }
}
