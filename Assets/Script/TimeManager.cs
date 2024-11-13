using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class TimeManager : MonoBehaviour
{
    // Inspector에서 지정할 수 있는 TextMeshPro 변수
    public TextMeshProUGUI timeText;

    // Inspector에서 지정할 수 있는 Image 변수
    public Image batteryImg;

    // Inspector에서 지정할 수 있는 Image 리스트 변수
    public List<Sprite> batteryList;

    // Inspector에서 지정할 수 있는 TextMeshPro 변수
    public TextMeshProUGUI pendingText;

    // Inspector에서 지정할 수 있는 TextMeshPro 변수 (날짜와 요일)
    public TextMeshProUGUI dateText;

    private DateTime lastInterestTime;
    private long pendingInterest;

    void Start()
    {
        // 매초마다 UpdateTimeAndBattery 메서드를 호출
        InvokeRepeating("UpdateTimeAndBattery", 0f, 1f);

        // 마지막 이자 지급 시간을 불러옴 (저장된 데이터가 없다면 현재 시간으로 설정)
        if (PlayerPrefs.HasKey("LastInterestTime"))
        {
            lastInterestTime = DateTime.Parse(PlayerPrefs.GetString("LastInterestTime"));
        }
        else
        {
            lastInterestTime = DateTime.Now;
        }

        // 이자 지급 체크 코루틴 시작
        StartCoroutine(CheckDailyInterest());
    }

    void OnApplicationQuit()
    {
        // 마지막 이자 지급 시간을 저장
        PlayerPrefs.SetString("LastInterestTime", lastInterestTime.ToString());
    }

    // 현재 시간과 배터리 상태를 가져와 TextMeshPro 및 Image 컴포넌트를 업데이트하는 메서드
    void UpdateTimeAndBattery()
    {
        // 현재 시간을 시스템에서 가져옴
        DateTime now = DateTime.Now;

        // 시간을 포맷하여 문자열로 변환 (예: "HH:mm" 형식)
        string formattedTime = now.ToString("HH:mm");

        // 배터리 잔량을 퍼센트로 변환
        float batteryLevel = SystemInfo.batteryLevel * 100; // 배터리 잔량 (퍼센트)

        // TextMeshPro 텍스트를 업데이트
        if (timeText != null)
        {
            timeText.text = formattedTime;
        }

        // 날짜와 요일을 업데이트 (예: "10월 23일 월요일" 형식)
        if (dateText != null)
        {
            string formattedDate = now.ToString("M월 d일 dddd", new CultureInfo("ko-KR"));
            dateText.text = formattedDate;
        }

        // 배터리 상태에 따라 이미지를 업데이트
        if (batteryImg != null && batteryList != null && batteryList.Count == 5)
        {
            if (batteryLevel <= 10)
            {
                batteryImg.sprite = batteryList[0];
            }
            else if (batteryLevel <= 50)
            {
                batteryImg.sprite = batteryList[1];
            }
            else if (batteryLevel <= 80)
            {
                batteryImg.sprite = batteryList[2];
            }
            else if (batteryLevel <= 90)
            {
                batteryImg.sprite = batteryList[3];
            }
            else
            {
                batteryImg.sprite = batteryList[4];
            }
        }
    }

    IEnumerator CheckDailyInterest()
    {
        while (true)
        {
            DateTime now = DateTime.Now;

            DateTime nextMidnight = now.Date.AddDays(1); // 다음 00시
            TimeSpan timeUntilMidnight = nextMidnight - now;

            yield return new WaitForSeconds((float)timeUntilMidnight.TotalSeconds);

            // 현재 시간과 마지막 이자 지급 시간을 비교하여 이자 계산
            DateTime lastCheckDate = lastInterestTime.Date;
            DateTime currentDate = now.Date;

            while (lastCheckDate < currentDate)
            {
                // 매일 이자를 계산
                CalculateDailyInterest();
                lastCheckDate = lastCheckDate.AddDays(1);
            }

            lastInterestTime = currentDate;
        }
    }

    void CalculateDailyInterest() // 이자가 붙는 메서드
    {
        long currentMoney = GameManager.Instance.GetMoney();
        long interest = (long)(currentMoney * 0.15f);
        pendingInterest += interest;

        Debug.Log($"Interest calculated: {interest}. Pending interest: {pendingInterest}");
        UpdatePending(); // 이자 계산 후 즉시 업데이트
    }

    public void ClaimDailyInterest() // 이자 수령 메서드
    {
        if (pendingInterest > 0)
        {
            long currentMoney = GameManager.Instance.GetMoney();
            GameManager.Instance.SetMoney(currentMoney + pendingInterest);
            Debug.Log($"Interest claimed: {pendingInterest}. New balance: {GameManager.Instance.GetMoney()}");
            BankManager.Instance.CreateBankPrefab("[코코아뱅크] pendingInterest 정기이자 입금", 1);

            pendingInterest = 0;
            UpdatePending(); // 이자 지급 후 즉시 업데이트
            BankManager.Instance.UpdateCurruntMoney();
        }
        else
        {
            Debug.Log("No pending interest to claim.");
        }
    }

    public void UpdatePending() // 이자 텍스트 갱신
    {
        if (pendingText != null)
        {
            pendingText.text = GameManager.Instance.FormatPrice(pendingInterest);
        }
    }

    // 배터리 상태 시뮬레이션 메서드
    void SimulateBatteryLevel(float level)
    {
        float batteryLevel = level * 100; // 배터리 잔량 (퍼센트)

        if (batteryImg != null && batteryList != null && batteryList.Count == 5)
        {
            if (batteryLevel <= 10)
            {
                batteryImg.sprite = batteryList[0];
            }
            else if (batteryLevel <= 50)
            {
                batteryImg.sprite = batteryList[1];
            }
            else if (batteryLevel <= 80)
            {
                batteryImg.sprite = batteryList[2];
            }
            else if (batteryLevel <= 90)
            {
                batteryImg.sprite = batteryList[3];
            }
            else
            {
                batteryImg.sprite = batteryList[4];
            }
        }

        Debug.Log($"Battery level simulated: {batteryLevel}%");
    }
}
