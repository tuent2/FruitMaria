using dotmob.Scripts.Core;
using dotmob.Scripts.System;
using UnityEngine;

namespace dotmob.Scripts.GUI
{
    /// <summary>
    /// Spends a life after game started or offers to buy a life
    /// </summary>
    public class GUIUtils : MonoBehaviour
    {
        public static GUIUtils THIS;

        private void Start()
        {
            if (!Equals(THIS, this)) THIS = this;
        }

        public void StartGame()
        {
            if (InitScript.lifes > 0)
            {
                InitScript.Instance.SpendLife(1);
                LevelManager.THIS.gameStatus = GameState.PrepareGame;
            }
            else
            {
                BuyLifeShop();
            }

        }

        public void BuyLifeShop()
        {

            if (InitScript.lifes < InitScript.Instance.CapOfLife)
                MenuReference.THIS.LiveShop.gameObject.SetActive(true);

        }
    }
}