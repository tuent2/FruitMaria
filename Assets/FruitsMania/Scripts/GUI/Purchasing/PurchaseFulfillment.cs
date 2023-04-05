using dotmob.Scripts.Core;
using UnityEngine;

namespace dotmob.Scripts.GUI.Purchasing
{
    public class PurchaseFulfillment : MonoBehaviour
    {
        public void GrandCoins(int amount)
        {
            InitScript.Instance.AddGems(amount);
        }
    }
}