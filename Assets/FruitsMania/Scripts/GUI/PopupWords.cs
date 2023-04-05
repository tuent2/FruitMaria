using dotmob.Scripts.Core;
using UnityEngine;

namespace dotmob.Scripts.GUI
{
    /// <summary>
    /// Greetings words for a combo
    /// </summary>
    public class PopupWords : MonoBehaviour
    {
        private void Update()
        {
            transform.position = LevelManager.THIS.field.GetPosition();
        }
    }
}