using dotmob.Scripts.Items;
using UnityEngine;

namespace dotmob.Scripts
{
    public class ItemAnimEvents : MonoBehaviour {


        public Item item;

        public void SetAnimationDestroyingFinished()
        {
            item.SetAnimationDestroyingFinished();
        }
    }
}
