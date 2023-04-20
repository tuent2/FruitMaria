using System;
using dotmob.Scripts.Core;
using dotmob.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace dotmob.Scripts.GUI
{
    /// <summary>
    /// Daily reward popup
    /// </summary>
    public class DailyReward : MonoBehaviour
    {
        public DayReward[] days;
        public TextMeshProUGUI description;
        int currentDay;
        public Button claim;
        bool isCheckingDaily;
        void OnEnable()
        {
            if (ServerTime.THIS.dateReceived)
            {
                CheckDaily();
            }



            ShowDailyReward();
        }

        private void LateUpdate()
        {
            if (isCheckingDaily == true)
            {
                claim.interactable = false;
                Debug.Log("111111111111111111111111111111111");
            }
        }

        private void ShowDailyReward()
        {
            if (!ServerTime.THIS.dateReceived)
            {
                ServerTime.OnDateReceived += ShowDailyReward;
                return;
            }

            var DateReward = PlayerPrefs.GetString("DateReward", default(DateTime).ToString());
            var dateTimeReward = DateTime.Parse(DateReward);
            DateTime testDate = ServerTime.THIS.serverTime;

            if (LevelManager.GetGameStatus() == GameState.Map)
            {
                if (DateReward == "" || DateReward == default(DateTime).ToString())
                    isCheckingDaily = false;
                else
                {
                    var timePassedDaily = testDate.Subtract(dateTimeReward).TotalDays;
                    if (timePassedDaily >= 1)
                        isCheckingDaily = false;
                    else
                        isCheckingDaily = true;
                }
            }
        }

        private void CheckDaily()
        {
            var previousDay = PlayerPrefs.GetInt("LatestDay", -1);
            var DateReward = PlayerPrefs.GetString("DateReward", ServerTime.THIS.serverTime.ToString());
            var timePassedDaily = (int)DateTime.Parse(DateReward).DayOfWeek;
            if (timePassedDaily > ((int)ServerTime.THIS.serverTime.DayOfWeek + 1) % 7 || previousDay == 6)
            {
                previousDay = -1;
            }

            for (var day = 0; day < days.Length; day++)
            {
                if (day <= previousDay)
                    days[day].SetPassedDay();
                if (day == previousDay + 1)
                {
                    days[day].SetCurrentDay();
                    currentDay = day;
                }

                if (day > previousDay + 1)
                    days[day].SetDayAhead();
            }
        }

        public void Ok()
        {
            PlayerPrefs.SetInt("LatestDay", currentDay);
            PlayerPrefs.SetString("DateReward", ServerTime.THIS.serverTime.ToString());
            PlayerPrefs.Save();
            var count = int.Parse(days[currentDay].count.text);
            InitScript.Instance.AddGems(count);
            description.text = "You got " + count + " coins";
            setCheckDaily();
            gameObject.SetActive(false);


        }

        private void setCheckDaily()
        {
            this.isCheckingDaily = true;
        }
        private void OnDisable()
        {
            ServerTime.OnDateReceived -= CheckDaily;

        }

        public void setDailyOff()
        {
            gameObject.SetActive(false);
        }
    }
}
