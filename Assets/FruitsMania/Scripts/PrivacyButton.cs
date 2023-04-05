using UnityEngine;

namespace dotmob.Scripts
{
    public class PrivacyButton : UnityEngine.MonoBehaviour
    {
        public void OpenPrivacy()
        {
            Application.OpenURL("https://www.google.com/");
        }
    }
}