using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    private static ProfileManager instance;
    public static ProfileManager Instance => instance;

    private DatabaseReference databaseReference;
    private DatabaseReference userReference;

    private UserProfile cachedProfile;
    public UserProfile CachedProfile => cachedProfile;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private async UniTaskVoid Start()
    {
        await FirebaseInitalizer.Instance.WaitForInitalizationAsync();

        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        userReference = databaseReference.Child("users");

        Debug.Log($"[Profile] ProfileManager �ʱ�ȭ �Ϸ�");
    }

    public async UniTask<(bool success , string error)> SaveProfileAsync(string nickname)
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "[Profile] �α��� X");
        }

        string userId = AuthManager.Instance.UserId;
        string email = AuthManager.Instance.CureentUser.Email ?? "�͸�";

        try
        {
            Debug.Log($"[Profile] ������ ���� �õ� {nickname}");

            UserProfile profile = new UserProfile(nickname, email);
            string json = profile.ToJson();

            await userReference.Child(userId).SetRawJsonValueAsync(json).AsUniTask();

            cachedProfile = profile;

            Debug.Log($"[Profile] ������ ���� ���� {nickname}");
            return (true , string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Profile] ������ ���� ���� {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<(UserProfile profile, string error)> LoadProfileAsync(string nickname)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return (null, "[Profile] �α��� X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Profile] ������ �ε� �õ� {nickname}");

            DataSnapshot snapshot = await userReference.Child(userId).GetValueAsync().AsUniTask();
            if(!snapshot.Exists)
            {
                Debug.Log($"[Profile] ������ ����");
                return (null, "[Profile] ������ ����");
            }

            string json = snapshot.GetRawJsonValue();
            cachedProfile = UserProfile.FromJson(json);

            Debug.Log($"[Profile] ������ ���� ���� {nickname}");
            return (cachedProfile, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Profile] ������ ���� ���� {ex.Message}");
            return (null, ex.Message);
        }
    }

    public async UniTask<(UserProfile profile, string error)> UpdateNicknameAsync(string newNickname)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return (null, "[Profile] �α��� X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Profile] �г��� ���� �õ� {newNickname}");

            await userReference.Child(userId).Child("nickname").SetValueAsync(newNickname).AsUniTask();
            cachedProfile.nickname = newNickname;

            Debug.Log($"[Profile] �г��� ���� ���� {newNickname}");
            return (cachedProfile, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Profile] �г��� ���� {ex.Message}");
            return (null, ex.Message);
        }
    }

    public async UniTask<bool> ProfileExistAsync()
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return false; ;
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            DataSnapshot snapshot = await userReference.Child(userId).GetValueAsync().AsUniTask();
            return snapshot.Exists;
        }
        catch(System.Exception ex)
        {
            Debug.LogError($"[Profile] ������ Ȯ�� ����: {ex.Message}");
            return false;
        }
    }
}
