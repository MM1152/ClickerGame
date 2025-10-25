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

        Debug.Log($"[Profile] ProfileManager 초기화 완료");
    }

    public async UniTask<(bool success , string error)> SaveProfileAsync(string nickname)
    {
        if(!AuthManager.Instance.IsLoggedIn)
        {
            return (false, "[Profile] 로그인 X");
        }

        string userId = AuthManager.Instance.UserId;
        string email = AuthManager.Instance.CureentUser.Email ?? "익명";

        try
        {
            Debug.Log($"[Profile] 프로필 저장 시도 {nickname}");

            UserProfile profile = new UserProfile(nickname, email);
            string json = profile.ToJson();

            await userReference.Child(userId).SetRawJsonValueAsync(json).AsUniTask();

            cachedProfile = profile;

            Debug.Log($"[Profile] 프로필 저장 성공 {nickname}");
            return (true , string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Profile] 프로필 저장 실패 {ex.Message}");
            return (false, ex.Message);
        }
    }

    public async UniTask<(UserProfile profile, string error)> LoadProfileAsync(string nickname)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return (null, "[Profile] 로그인 X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Profile] 프로필 로드 시도 {nickname}");

            DataSnapshot snapshot = await userReference.Child(userId).GetValueAsync().AsUniTask();
            if(!snapshot.Exists)
            {
                Debug.Log($"[Profile] 프로필 없음");
                return (null, "[Profile] 프로필 없음");
            }

            string json = snapshot.GetRawJsonValue();
            cachedProfile = UserProfile.FromJson(json);

            Debug.Log($"[Profile] 프로필 저장 성공 {nickname}");
            return (cachedProfile, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Profile] 프로필 저장 실패 {ex.Message}");
            return (null, ex.Message);
        }
    }

    public async UniTask<(UserProfile profile, string error)> UpdateNicknameAsync(string newNickname)
    {
        if (!AuthManager.Instance.IsLoggedIn)
        {
            return (null, "[Profile] 로그인 X");
        }

        string userId = AuthManager.Instance.UserId;

        try
        {
            Debug.Log($"[Profile] 닉네임 변경 시도 {newNickname}");

            await userReference.Child(userId).Child("nickname").SetValueAsync(newNickname).AsUniTask();
            cachedProfile.nickname = newNickname;

            Debug.Log($"[Profile] 닉네임 변경 성공 {newNickname}");
            return (cachedProfile, string.Empty);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Profile] 닉네임 실패 {ex.Message}");
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
            Debug.LogError($"[Profile] 프로필 확인 실패: {ex.Message}");
            return false;
        }
    }
}
