using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class TimeManager : MonoBehaviour
{
    // Inspector���� ������ �� �ִ� TextMeshPro ����
    public TextMeshProUGUI timeText;

    // Inspector���� ������ �� �ִ� Image ����
    public Image batteryImg;

    // Inspector���� ������ �� �ִ� Image ����Ʈ ����
    public List<Sprite> batteryList;

    // Inspector���� ������ �� �ִ� TextMeshPro ����
    public TextMeshProUGUI pendingText;

    // Inspector���� ������ �� �ִ� TextMeshPro ���� (��¥�� ����)
    public TextMeshProUGUI dateText;

    private DateTime lastInterestTime;
    private long pendingInterest;

    void Start()
    {
        // ���ʸ��� UpdateTimeAndBattery �޼��带 ȣ��
        InvokeRepeating("UpdateTimeAndBattery", 0f, 1f);

        // ������ ���� ���� �ð��� �ҷ��� (����� �����Ͱ� ���ٸ� ���� �ð����� ����)
        if (PlayerPrefs.HasKey("LastInterestTime"))
        {
            lastInterestTime = DateTime.Parse(PlayerPrefs.GetString("LastInterestTime"));
        }
        else
        {
            lastInterestTime = DateTime.Now;
        }

        // ���� ���� üũ �ڷ�ƾ ����
        StartCoroutine(CheckDailyInterest());
    }

    void OnApplicationQuit()
    {
        // ������ ���� ���� �ð��� ����
        PlayerPrefs.SetString("LastInterestTime", lastInterestTime.ToString());
    }

    // ���� �ð��� ���͸� ���¸� ������ TextMeshPro �� Image ������Ʈ�� ������Ʈ�ϴ� �޼���
    void UpdateTimeAndBattery()
    {
        // ���� �ð��� �ý��ۿ��� ������
        DateTime now = DateTime.Now;

        // �ð��� �����Ͽ� ���ڿ��� ��ȯ (��: "HH:mm" ����)
        string formattedTime = now.ToString("HH:mm");

        // ���͸� �ܷ��� �ۼ�Ʈ�� ��ȯ
        float batteryLevel = SystemInfo.batteryLevel * 100; // ���͸� �ܷ� (�ۼ�Ʈ)

        // TextMeshPro �ؽ�Ʈ�� ������Ʈ
        if (timeText != null)
        {
            timeText.text = formattedTime;
        }

        // ��¥�� ������ ������Ʈ (��: "10�� 23�� ������" ����)
        if (dateText != null)
        {
            string formattedDate = now.ToString("M�� d�� dddd", new CultureInfo("ko-KR"));
            dateText.text = formattedDate;
        }

        // ���͸� ���¿� ���� �̹����� ������Ʈ
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

            DateTime nextMidnight = now.Date.AddDays(1); // ���� 00��
            TimeSpan timeUntilMidnight = nextMidnight - now;

            yield return new WaitForSeconds((float)timeUntilMidnight.TotalSeconds);

            // ���� �ð��� ������ ���� ���� �ð��� ���Ͽ� ���� ���
            DateTime lastCheckDate = lastInterestTime.Date;
            DateTime currentDate = now.Date;

            while (lastCheckDate < currentDate)
            {
                // ���� ���ڸ� ���
                CalculateDailyInterest();
                lastCheckDate = lastCheckDate.AddDays(1);
            }

            lastInterestTime = currentDate;
        }
    }

    void CalculateDailyInterest() // ���ڰ� �ٴ� �޼���
    {
        long currentMoney = GameManager.Instance.GetMoney();
        long interest = (long)(currentMoney * 0.15f);
        pendingInterest += interest;

        Debug.Log($"Interest calculated: {interest}. Pending interest: {pendingInterest}");
        UpdatePending(); // ���� ��� �� ��� ������Ʈ
    }

    public void ClaimDailyInterest() // ���� ���� �޼���
    {
        if (pendingInterest > 0)
        {
            long currentMoney = GameManager.Instance.GetMoney();
            GameManager.Instance.SetMoney(currentMoney + pendingInterest);
            Debug.Log($"Interest claimed: {pendingInterest}. New balance: {GameManager.Instance.GetMoney()}");
            BankManager.Instance.CreateBankPrefab("[���ھƹ�ũ] pendingInterest �������� �Ա�", 1);

            pendingInterest = 0;
            UpdatePending(); // ���� ���� �� ��� ������Ʈ
            BankManager.Instance.UpdateCurruntMoney();
        }
        else
        {
            Debug.Log("No pending interest to claim.");
        }
    }

    public void UpdatePending() // ���� �ؽ�Ʈ ����
    {
        if (pendingText != null)
        {
            pendingText.text = GameManager.Instance.FormatPrice(pendingInterest);
        }
    }

    // ���͸� ���� �ùķ��̼� �޼���
    void SimulateBatteryLevel(float level)
    {
        float batteryLevel = level * 100; // ���͸� �ܷ� (�ۼ�Ʈ)

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
