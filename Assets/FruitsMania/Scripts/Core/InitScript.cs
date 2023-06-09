using System;
using System.Collections.Generic;
using dotmob.Scripts.GUI;
using dotmob.Scripts.GUI.Boost;
using dotmob.Scripts.Integrations;
using dotmob.Scripts.Integrations.Network;
using dotmob.Scripts.Level;
using dotmob.Scripts.MapScripts;
using dotmob.Scripts.System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using TMPro;
namespace dotmob.Scripts.Core
{
    /// <summary>
    /// class for main system variables, ads control and in-app purchasing
    /// </summary>
    public class InitScript : MonoBehaviour
    {
        public static InitScript Instance;

        /// opening level in Menu Play
        public static int openLevel;

        ///life gaining timer
        public static float RestLifeTimer;

        ///date of exit for life timer
        public static string DateOfExit;

        //reward which can be receive after watching rewarded ads
        public RewardsType currentReward;

        ///amount of life
        public static int lifes { get; set; }

        //EDITOR: max amount of life
        public int CapOfLife = 5;

        //EDITOR: time for rest life
        public float TotalTimeForRestLifeHours;

        //EDITOR: time for rest life
        public float TotalTimeForRestLifeMin = 15;

        //EDITOR: time for rest life
        public float TotalTimeForRestLifeSec = 60;

        //EDITOR: coins gifted in start
        public int FirstGems = 20;

        //amount of coins
        public static int Gems;

        //wait for purchasing of coins succeed
        public static int waitedPurchaseGems;

        //EDITOR: how often to show the "Rate us on the store" popup
        public int ShowRateEvery;

        //EDITOR: rate url
        public string RateURL;

        public string RateURLIOS;
        public string RateURLHuawei;

        //rate popup reference
        private GameObject rate;

        //EDITOR: amount for rewarded ads
        public int rewardedGems = 5;

        //EDITOR: should player lose a life for every passed level
        public bool losingLifeEveryGame;

        //daily reward popup reference
        public GameObject DailyMenu;

        private int packFristCount = 0;
        private int packSecondCount = 0;
        private int packThirdCount = 0;
        private int packFourCount = 0;
        public int totalPackFristCount = 1;
        public int totalPackSecondCount = 2;
        public int totalPackThirdCount = 3;
        public int totalPackFourCount = 4;
        [SerializeField] TextMeshProUGUI pack1Text;
        [SerializeField] TextMeshProUGUI pack2Text;
        [SerializeField] TextMeshProUGUI pack3Text;
        [SerializeField] TextMeshProUGUI pack4Text;

        // Use this for initialization
        void Awake()
        {
            Application.targetFrameRate = 60;
            Instance = this;
            RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");
            DateOfExit = PlayerPrefs.GetString("DateOfExit", "");
            if (DateOfExit == "" || DateOfExit == default(DateTime).ToString())
                DateOfExit = ServerTime.THIS.serverTime.ToString();
            DebugLogKeeper.Init();
            Gems = PlayerPrefs.GetInt("Gems");
            lifes = PlayerPrefs.GetInt("Lifes");
            if (PlayerPrefs.GetInt("Lauched") == 0)
            {
                //First lauching
                lifes = CapOfLife;
                PlayerPrefs.SetInt("Lifes", lifes);
                Gems = FirstGems;
                PlayerPrefs.SetInt("Gems", Gems);
                PlayerPrefs.SetInt("Music", 1);
                PlayerPrefs.SetInt("Sound", 1);

                PlayerPrefs.SetInt("Lauched", 1);
                PlayerPrefs.Save();
            }

            if (!PlayerPrefs.HasKey("packFristCount"))
            {
                this.packFristCount = 0;
            }
            else
            {
                this.packFristCount = PlayerPrefs.GetInt("packFristCount");
            }
            //pack2
            if (!PlayerPrefs.HasKey("packSecondCount"))
            {
                this.packSecondCount = 0;
            }
            else
            {
                this.packSecondCount = PlayerPrefs.GetInt("packSecondCount");
            }
            // pack 3
            if (!PlayerPrefs.HasKey("packThirdCount"))
            {
                this.packThirdCount = 0;
            }
            else
            {
                this.packThirdCount = PlayerPrefs.GetInt("packThirdCount");
            }
            //pack 4
            if (!PlayerPrefs.HasKey("packFourCount"))
            {
                this.packFourCount = 0;
            }
            else
            {
                this.packFourCount = PlayerPrefs.GetInt("packFourCount");
            }

            rate = Instantiate(Resources.Load("Prefabs/Rate")) as GameObject;
            rate.SetActive(false);
            rate.transform.SetParent(MenuReference.THIS.transform);
            rate.transform.localPosition = Vector3.zero;
            rate.GetComponent<RectTransform>().offsetMin = new Vector2(-5, -5);
            rate.GetComponent<RectTransform>().offsetMax = new Vector2(5, 5);
            //        rate.GetComponent<RectTransform>().anchoredPosition = (Resources.Load("Prefabs/Rate") as GameObject).GetComponent<RectTransform>().anchoredPosition;
            rate.transform.localScale = Vector3.one;
            var g = MenuReference.THIS.Reward.gameObject;
            g.SetActive(true);
            g.SetActive(false);
            if (CrosssceneData.totalLevels == 0)
                CrosssceneData.totalLevels = LoadingManager.GetLastLevelNum();
#if FACEBOOK
            FacebookManager fbManager = new GameObject("FacebookManager").AddComponent<FacebookManager>();
#endif
        }



        public void SaveLevelStarsCount(int level, int starsCount)
        {
            Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
            PlayerPrefs.SetInt(GetLevelKey(level), starsCount);

        }

        private string GetLevelKey(int number)
        {
            return string.Format("Level.{0:000}.StarsCount", number);
        }

        private void Update()
        {
            pack1Text.text = packFristCount + "/" + totalPackFristCount;
            pack2Text.text = packSecondCount + "/" + totalPackSecondCount;
            pack3Text.text = packThirdCount + "/" + totalPackThirdCount;
            pack4Text.text = packFourCount + "/" + totalPackFourCount;
        }



        public void ShowRate()
        {
            rate.SetActive(true);
        }


        public void ShowReward()
        {
            var reward = MenuReference.THIS.Reward.GetComponent<RewardIcon>();
            if (currentReward == RewardsType.GetGems)
            {
                reward.SetIconSprite(0);

                reward.gameObject.SetActive(true);
                AddGems(rewardedGems);
                MenuReference.THIS.GemsShop.GetComponent<AnimationEventManager>().CloseMenu();
            }
            else if (currentReward == RewardsType.GetLifes)
            {
                reward.SetIconSprite(1);
                reward.gameObject.SetActive(true);
                RestoreLifes();
                MenuReference.THIS.LiveShop.GetComponent<AnimationEventManager>().CloseMenu();
            }
            else if (currentReward == RewardsType.GetGoOn)
            {
                MenuReference.THIS.PreFailed.GetComponent<AnimationEventManager>().GoOnFailed();
            }

        }

        public void SetGems(int count)
        {
            Gems = count;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();
        }


        public void AddGems(int count)
        {
            Debug.Log("Gems" + Gems);
            Gems += count;
            Debug.Log("Gems" + Gems);
            PlayerPrefs.SetInt("Gems", Gems);
            Debug.Log("Gems" + PlayerPrefs.GetInt("Gems"));
            PlayerPrefs.Save();
            // #if PLAYFAB || GAMESPARKS
            //             NetworkManager.currencyManager.IncBalance(count);
            // #endif

        }

        public void AddCount(int pack)
        {
            if (pack == 2)
            {
                packFristCount++;
                if (packFristCount == totalPackFristCount)
                {
                    InitScript.Instance.AddGems(10);
                    packFristCount = 0;
                }
                PlayerPrefs.SetInt("packFristCount", packFristCount);
                PlayerPrefs.Save();
            }
            else if (pack == 3)
            {
                packSecondCount++;
                if (packSecondCount == totalPackSecondCount)
                {
                    InitScript.Instance.AddGems(30);
                    packSecondCount = 0;
                }
                PlayerPrefs.SetInt("packSecondCount", packSecondCount);
                PlayerPrefs.Save();
            }
            else if (pack == 4)
            {
                packThirdCount++;
                if (packThirdCount == totalPackThirdCount)
                {
                    InitScript.Instance.AddGems(60);
                    packThirdCount = 0;
                }
                PlayerPrefs.SetInt("packThirdCount", packThirdCount);
                PlayerPrefs.Save();
            }
            else
            {
                packFourCount++;
                if (packFourCount == totalPackFourCount)
                {
                    InitScript.Instance.AddGems(90);
                    packFourCount = 0;
                }
                PlayerPrefs.SetInt("packFourCount", packFourCount);
                PlayerPrefs.Save();
            }
        }

        public void SpendGems(int count)
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.cash);
            Gems -= count;
            PlayerPrefs.SetInt("Gems", Gems);
            PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
            NetworkManager.currencyManager.DecBalance(count);
#endif

        }


        public void RestoreLifes()
        {
            lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.Save();
        }

        public void AddLife(int count)
        {
            lifes += count;
            if (lifes > CapOfLife)
                lifes = CapOfLife;
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.Save();
        }

        public int GetLife()
        {
            if (lifes > CapOfLife)
            {
                lifes = CapOfLife;
                PlayerPrefs.SetInt("Lifes", lifes);
                PlayerPrefs.Save();
            }

            return lifes;
        }

        public void PurchaseSucceded()
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.cash);
            AddGems(waitedPurchaseGems);
            waitedPurchaseGems = 0;
        }

        public void SpendLife(int count)
        {
            if (lifes > 0)
            {
                lifes -= count;
                PlayerPrefs.SetInt("Lifes", lifes);
                PlayerPrefs.Save();
            }

            //else
            //{
            //    GameObject.Find("Canvas").transform.Find("RestoreLifes").gameObject.SetActive(true);
            //}
        }

        public void BuyBoost(BoostType boostType, int price, int count)
        {
            PlayerPrefs.SetInt("" + boostType, PlayerPrefs.GetInt("" + boostType) + count);
            PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
            NetworkManager.dataManager.SetBoosterData();
#endif
        }

        public void SpendBoost(BoostType boostType)
        {
            PlayerPrefs.SetInt("" + boostType, PlayerPrefs.GetInt("" + boostType) - 1);
            PlayerPrefs.Save();
#if PLAYFAB || GAMESPARKS
            NetworkManager.dataManager.SetBoosterData();
#endif
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (RestLifeTimer > 0)
                {
                    PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
                }

                PlayerPrefs.SetInt("Lifes", lifes);
                PlayerPrefs.SetString("DateOfExit", ServerTime.THIS.serverTime.ToString());
                PlayerPrefs.Save();
            }
        }

        void OnApplicationQuit()
        {
            //1.4  added 
            if (RestLifeTimer > 0)
            {
                PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
            }

            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.SetString("DateOfExit", ServerTime.THIS.serverTime.ToString());
            PlayerPrefs.Save();
            //print(RestLifeTimer)
        }

        public void OnLevelClicked(object sender, LevelReachedEventArgs args)
        {
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;
            if (!GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.activeSelf &&
                !GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.activeSelf &&
                !GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.activeSelf)
            {
                SoundBase.Instance.PlayOneShot(SoundBase.Instance.click);
                OpenMenuPlay(args.Number);
            }
        }

        public static void OpenMenuPlay(int num)
        {
            PlayerPrefs.SetInt("OpenLevel", num);
            PlayerPrefs.Save();
            LevelManager.THIS.MenuPlayEvent();
            LevelManager.THIS.LoadLevel();
            openLevel = num;
            CrosssceneData.openNextLevel = false;
            MenuReference.THIS.MenuPlay.gameObject.SetActive(true);
        }

        void OnEnable()
        {
            LevelsMap.LevelSelected += OnLevelClicked;
            LevelsMap.OnLevelReached += OnLevelReached;

        }

        void OnDisable()
        {
            LevelsMap.LevelSelected -= OnLevelClicked;
            LevelsMap.OnLevelReached -= OnLevelReached;

            //		if(RestLifeTimer>0){
            PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
            //		}
            PlayerPrefs.SetInt("Lifes", lifes);
            PlayerPrefs.SetString("DateOfExit", ServerTime.THIS.serverTime.ToString());
            PlayerPrefs.Save();


        }

        void OnLevelReached()
        {
            var num = PlayerPrefs.GetInt("OpenLevel");
            if (CrosssceneData.openNextLevel && CrosssceneData.totalLevels >= num)
            {
                OpenMenuPlay(num);
            }
        }
    }

    /// moves or time is level limit type
    public enum LIMIT
    {
        MOVES,
        TIME
    }

    /// reward type for rewarded ads watching
    public enum RewardsType
    {
        GetLifes,
        GetGems,
        GetGoOn
    }
}
