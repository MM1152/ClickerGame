using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUi : MonoBehaviour
{
    public GameObject loginPanel;

    public Button anonymousLoginButton;
    public Button emailLoginButton;
    public Button createAuthButton;
    public Button profileButton;
    public TMP_InputField emailInputField;
    public TMP_InputField passwdInputField;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI profileText;
    public async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => AuthManager.Instance.IsInitalized);

        anonymousLoginButton.onClick.AddListener(() => OnClickAnoymouseLoginButton().Forget());
        emailLoginButton.onClick.AddListener(() => OnClickeEmailLoginButton().Forget());
        createAuthButton.onClick.AddListener(() => OnClickCreateAuthLoginButton().Forget());
        profileButton.onClick.AddListener(() => {
            AuthManager.Instance.SignOut();
            UpdateUI().Forget();
        });

        SetButtonsInteractable(true);
        UpdateUI().Forget();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        anonymousLoginButton.interactable = interactable;
        emailLoginButton.interactable = interactable;
        createAuthButton.interactable = interactable;
    }

    

    public async UniTaskVoid UpdateUI()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsInitalized) 
            return;

        bool isLoggined = AuthManager.Instance.IsLoggedIn;
        loginPanel.SetActive(!isLoggined);

        statusText.text = string.Empty;
        
        if(isLoggined)
        {
            string userId = AuthManager.Instance.UserId;
            profileText.text = userId;
        }
        else
        {
            profileText.text = string.Empty;
        }
    }

    private async UniTaskVoid OnClickAnoymouseLoginButton()
    {
        SetButtonsInteractable(false);
        var (sussecs , error) = await AuthManager.Instance.SignInAnonymouslyAsync();
        if(sussecs)
        {
            
        }
        else
        {

        }
        SetButtonsInteractable(true);

        UpdateUI().Forget();
    }

    private async UniTaskVoid OnClickeEmailLoginButton()
    {
        SetButtonsInteractable(false);
        var (sussecs, error) = await AuthManager.Instance.SignInWithEamilAsync(emailInputField.text, passwdInputField.text);
        if (sussecs)
        {

        }
        else
        {

        }
        SetButtonsInteractable(true);

        UpdateUI().Forget();
    }

    private async UniTaskVoid OnClickCreateAuthLoginButton()
    {
        SetButtonsInteractable(false);
        var (sussecs, error) = await AuthManager.Instance.CreateUserWithEmailAsync(emailInputField.text, passwdInputField.text);
        if (sussecs)
        {

        }
        else
        {

        }
        SetButtonsInteractable(true);

        UpdateUI().Forget();
    }
}
