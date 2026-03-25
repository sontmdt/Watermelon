using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI detailsText;
    public void Start()
    {
        SignIn();   
    }
    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string avatar = PlayGamesPlatform.Instance.GetUserImageUrl();

            detailsText.text = "Success \n " + name;
        }
        else
        {
            detailsText.text = "Sign in Failed!!";
        }
    }
}
