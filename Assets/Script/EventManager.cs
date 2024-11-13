using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class EventData
{
    public string Title;
    public string Content;
    public string Button1Result;
    public string Button2Result;
    public long EventMoney1;
    public long EventMoney2;

    public EventData(string title, string content, string button1Result, string button2Result, long eventMoney1, long eventMoney2)
    {
        Title = title;
        Content = content;
        Button1Result = button1Result;
        Button2Result = button2Result;
        EventMoney1 = eventMoney1;
        EventMoney2 = eventMoney2;
    }
}

public class EventManager : MonoBehaviour
{
    public GameObject OkayButton;
    public GameObject EventPopup;

    public TextMeshProUGUI EventTitle;
    public TextMeshProUGUI EventText;

    public GameObject Buttons;

    public Animator eventAnimator;

    public List<EventData> eventList = new List<EventData>();

    private EventData currentEventData;

    public List<string> eventTitles = new List<string>();
    public List<string> guaranteedEventTitles = new List<string>();
    private int encounterCount = 0;

    public void DisplayEvent(string eventTitle)
    {
        EventPopup.SetActive(true);

        foreach (var eventData in eventList)
        {
            if (eventData.Title == eventTitle)
            {
                currentEventData = eventData;
                EventTitle.text = eventData.Title;
                EventText.text = eventData.Content;

                if (eventAnimator != null)
                {
                    eventAnimator.SetTrigger("����");
                }

                Buttons.SetActive(true);
                OkayButton.SetActive(false);

                SoundManager.instance.PlaySFX(17);

                return;
            }
        }

        EventTitle.text = "�̺�Ʈ ����";
        EventText.text = "�ش� �̺�Ʈ�� ã�� �� �����ϴ�.";
        Buttons.SetActive(false);
        OkayButton.SetActive(false);
    }

    public void OnDislikeButtonClicked()
    {
        if (currentEventData != null)
        {
            eventAnimator.SetTrigger("����");
            EventTitle.text = "�������� �ʱ�� �����ߴ�.";
            EventText.text = currentEventData.Button1Result;

            ProcessEventMoney(currentEventData.EventMoney1);
        }
        Buttons.SetActive(false);
        OkayButton.SetActive(true);
    }

    public void OnLikeButtonClicked()
    {
        if (currentEventData != null)
        {
            eventAnimator.SetTrigger("����");
            EventTitle.text = "�����ϱ�� �����ߴ�.";
            EventText.text = currentEventData.Button2Result;

            ProcessEventMoney(currentEventData.EventMoney2);
        }
        Buttons.SetActive(false);
        OkayButton.SetActive(true);
    }

    public void OnOkayButtonClicked()
    {
        if (eventAnimator != null)
        {
            eventAnimator.SetTrigger("�ݱ�");
        }
        Buttons.SetActive(false);
        OkayButton.SetActive(false);

        StartCoroutine(HideEventPopupAfterDelay());
    }

    private IEnumerator HideEventPopupAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        EventPopup.SetActive(false);
    }

    private void ProcessEventMoney(long eventMoney)
    {
        // eventMoney�� 0�� ��� �ƹ� �۾��� �������� �ʰ� �Լ� ����
        if (eventMoney == 0) return;

        string formattedEventMoney = GameManager.Instance.FormatPrice(System.Math.Abs(eventMoney));

        if (eventMoney < 0)
        {
            BankManager.Instance.CreateBankPrefab($"[�̺�Ʈ ���] {formattedEventMoney} ���", 2);
        }
        else
        {
            BankManager.Instance.CreateBankPrefab($"[�̺�Ʈ ���] {formattedEventMoney} �Ա�", 2);
        }

        GameManager.Instance.SetMoney(GameManager.Instance.money + eventMoney);
    }

    public void RandomEncounter()
    {
        bool guaranteedEventTriggered = GuaranteedEncounter(); // Ȯ�� ��ī���� üũ �� �÷��� ��ȯ

        if (guaranteedEventTriggered || EventPopup.activeSelf) // Ȯ�� �̺�Ʈ �߻� �� �Ǵ� �̹� �̺�Ʈ â�� �����ִ� ��� ���� ��ī���� ����
            return;

        int diceRoll = UnityEngine.Random.Range(1, 11);

        if (diceRoll < 2)
        {
            if (eventTitles.Count > 0)
            {
                string randomEventTitle = eventTitles[UnityEngine.Random.Range(0, eventTitles.Count)];
                DisplayEvent(randomEventTitle);
            }
            else
            {
                Debug.Log("�̺�Ʈ ���� ����Ʈ�� ��� �ֽ��ϴ�.");
            }
        }
    }

    public bool GuaranteedEncounter()
    {
        // ���� ȣ�� Ƚ���� ������Ű�� ������ Ȯ���մϴ�
        encounterCount++;

        if ((encounterCount & (encounterCount - 1)) == 0 && encounterCount >= 4) // 2�� �ŵ������� ����
        {
            int eventIndex = (int)Mathf.Log(encounterCount, 2) - 2;

            if (eventIndex < guaranteedEventTitles.Count)
            {
                string guaranteedEventTitle = guaranteedEventTitles[eventIndex];
                DisplayEvent(guaranteedEventTitle);
                return true; // Ȯ�� ��ī���� �߻�
            }
            else
            {
                Debug.Log("Ȯ�� �̺�Ʈ ����Ʈ�� ���� �����߽��ϴ�.");
            }
        }

        return false; // Ȯ�� ��ī���� �̹߻�
    }
}
