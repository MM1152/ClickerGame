using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;

public class FirebaseInitalizer : MonoBehaviour
{
    private static FirebaseInitalizer instance;
    public static FirebaseInitalizer Instance => instance;

    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    private FirebaseApp firebaseApp; // ���̾� ���̽� ����
    public FirebaseApp FirebaseApp => firebaseApp; // ��� ���̾�̽� API�� �񵿱�� ó���� �̷����

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
        Debug.Log("[Firebase] �ʱ�ȭ ����");

        try
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask(); // �񵿱� ���� �������� üũ�ؼ� status �Ѱ���

            if(status == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                isInitialized = true;

                Debug.Log($"[Firebase] �ʱ�ȭ ���� ! {firebaseApp.Name}");
            }
            else
            {
                Debug.Log($"[Firebase] �ʱ�ȭ ����: {status}");
                isInitialized = false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[Firebase] �ʱ�ȭ ����: {ex.Message}");
        }
    }
    public async UniTask WaitForInitalizationAsync()
    {
        await UniTask.WaitUntil(() => isInitialized);
    }
}
