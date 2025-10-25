using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;

public class FirebaseInitalizer : MonoBehaviour
{
    private static FirebaseInitalizer instance;
    public static FirebaseInitalizer Instance => instance;

    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    private FirebaseApp firebaseApp; // 파이어 베이스 관리
    public FirebaseApp FirebaseApp => firebaseApp; // 모든 파이어베이스 API는 비동기로 처리가 이루어짐

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitalizedFirebaseAsync().Forget();
    }

    private void OnDestroy()
    {
        
    }

    private async UniTaskVoid InitalizedFirebaseAsync()
    {
        Debug.Log("[Firebase] 초기화 시작");

        try
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask(); // 비동기 실행 가능한지 체크해서 status 넘겨줌

            if(status == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                isInitialized = true;

                Debug.Log($"[Firebase] 초기화 성공 ! {firebaseApp.Name}");
            }
            else
            {
                Debug.Log($"[Firebase] 초기화 오류: {status}");
                isInitialized = false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Firebase] 초기화 오류: {ex.Message}");
        }
    }
    public async UniTask WaitForInitalizationAsync()
    {
        await UniTask.WaitUntil(() => isInitialized);
    }
}
