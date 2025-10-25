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
            Debug.Log($"[Auth] �̹� �α��ε�: {UserId}"); // ����Ǹ� �ȵ�
        }
        else
        {
            Debug.Log($"[Auth] �α����� �ʿ���");
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
            Debug.Log($"[Auth] �͸� �α��� �õ�...");

            AuthResult result = await auth.SignInAnonymouslyAsync().AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] �͸� �α��� ���� {UserId}");
            return (true, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] �͸� �α��� ����: {ex.Message}");
            return (false, ex.Message);
        }
    }
    public async UniTask<(bool success, string error)> CreateUserWithEmailAsync(string email , string password)
    {
        try
        {
            Debug.Log($"[Auth] ȸ������ �õ�...");

            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password).AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] ȸ������ ���� {UserId}");
            return (true, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] ȸ������ ����: {ex.Message}");
            return (false, ex.Message);
        }
    }
    public async UniTask<(bool success, string error)> SignInWithEamilAsync(string email , string password)
    {
        try
        {
            Debug.Log($"[Auth] �α��� �õ�...");

            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password).AsUniTask();
            currentUser = result.User;

            Debug.Log($"[Auth] �α��� ���� {UserId}");
            return (true, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Auth] �α��� ����: {ex.Message}");
            return (false, ex.Message);
        }
    }

    public void SignOut()
    {
        if(auth != null && currentUser != null)
        {
            Debug.Log($"[Auth] �α׾ƿ�");
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
                Debug.Log($"[Auth] �α׾ƿ� ��");
            }
            else if(signedIn)
            {
                Debug.Log($"[Auth] �α��� ��");
            }

            currentUser = auth.CurrentUser;
        }
    }

    private string ParseFairebaseError(string error)
    {
        return "";
    }
}
