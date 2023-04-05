using dotmob.Scripts.Core;

namespace dotmob.Scripts.Items
{
    /// <summary>
    /// Game blocks handler. It lock ups the game on animations
    /// </summary>
    public class GameBlocker : UnityEngine.MonoBehaviour
    {
        private void Start()
        {
            LevelManager.THIS._stopFall.Add(this);
        }

        private void OnDisable()
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            LevelManager.THIS._stopFall.Remove(this);
        }
    }
}