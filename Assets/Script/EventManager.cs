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
                    eventAnimator.SetTrigger("열기");
                }

                Buttons.SetActive(true);
                OkayButton.SetActive(false);

                SoundManager.instance.PlaySFX(17);

                return;
            }
        }

        EventTitle.text = "이벤트 없음";
        EventText.text = "해당 이벤트를 찾을 수 없습니다.";
        Buttons.SetActive(false);
        OkayButton.SetActive(false);
    }

    public void OnDislikeButtonClicked()
    {
        if (currentEventData != null)
        {
            eventAnimator.SetTrigger("열기");
            EventTitle.text = "진행하지 않기로 결정했다.";
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
            eventAnimator.SetTrigger("열기");
            EventTitle.text = "진행하기로 결정했다.";
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
            eventAnimator.SetTrigger("닫기");
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
        // eventMoney가 0일 경우 아무 작업도 수행하지 않고 함수 종료
        if (eventMoney == 0) return;

        string formattedEventMoney = GameManager.Instance.FormatPrice(System.Math.Abs(eventMoney));

        if (eventMoney < 0)
        {
            BankManager.Instance.CreateBankPrefab($"[이벤트 결과] {formattedEventMoney} 출금", 2);
        }
        else
        {
            BankManager.Instance.CreateBankPrefab($"[이벤트 결과] {formattedEventMoney} 입금", 2);
        }

        GameManager.Instance.SetMoney(GameManager.Instance.money + eventMoney);
    }

    public void RandomEncounter()
    {
        bool guaranteedEventTriggered = GuaranteedEncounter(); // 확정 인카운터 체크 후 플래그 반환

        if (guaranteedEventTriggered || EventPopup.activeSelf) // 확정 이벤트 발생 시 또는 이미 이벤트 창이 열려있는 경우 랜덤 인카운터 방지
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
                Debug.Log("이벤트 제목 리스트가 비어 있습니다.");
            }
        }
    }

    public bool GuaranteedEncounter()
    {
        // 먼저 호출 횟수를 증가시키고 조건을 확인합니다
        encounterCount++;

        if ((encounterCount & (encounterCount - 1)) == 0 && encounterCount >= 4) // 2의 거듭제곱일 때만
        {
            int eventIndex = (int)Mathf.Log(encounterCount, 2) - 2;

            if (eventIndex < guaranteedEventTitles.Count)
            {
                string guaranteedEventTitle = guaranteedEventTitles[eventIndex];
                DisplayEvent(guaranteedEventTitle);
                return true; // 확정 인카운터 발생
            }
            else
            {
                Debug.Log("확정 이벤트 리스트의 끝에 도달했습니다.");
            }
        }

        return false; // 확정 인카운터 미발생
    }
}
