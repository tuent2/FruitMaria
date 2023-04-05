using UnityEngine;
using UnityEngine.Events;

namespace dotmob.Scripts.System
{
    public class OnEnableAction : MonoBehaviour
    {
        public UnityEvent[] myEvent;
 
        void OnEnable()
        {
            foreach (var unityEvent in myEvent)
            {
                unityEvent.Invoke();
            
            }
        }
    }
}