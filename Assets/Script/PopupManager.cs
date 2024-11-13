using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance; // 싱글톤 인스턴스

    private TextMeshProUGUI popupMessageText;
    [SerializeField] private Transform popupTarget;
    [SerializeField] private GameObject popupPrefab; // Prefab을 지정할 변수
    [SerializeField] private Transform messageTarget; // Inspector에서 지정할 수 있는 변수 추가
    [SerializeField] private GameObject messagePrefab; // Inspector에서 지정할 수 있는 변수 추가
    [SerializeField] private GameObject afterEndingPrefab; // Inspector에서 지정할 수 있는 변수 추가
    [SerializeField] private List<Sprite> ImgList; // Inspector에서 지정할 수 있는 이미지 리스트 추가
    private Vector3 originalPosition;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (GameManager.Instance != null)
            {
                // ClickerManager 인스턴스가 초기화될 때까지 기다림
                StartCoroutine(GameManager.Instance.InitializeAfterClickerManager());
            }
        }
        else
        {
            Destroy(gameObject);
        }

        if (ScriptManager.Instance != null && ScriptManager.Instance.tutorId == 0)
        {
            Debug.Log("??");
            ScriptManager.Instance.NextHelperMessage();
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            // GameManager의 MoneyIncreased 이벤트에 대한 구독 설정
            GameManager.Instance.MoneyIncreased += IncreaseMoneyPopup;

            // GameManager의 MoneyDecreased 이벤트에 대한 구독 설정
            GameManager.Instance.MoneyDecreased += DecreaseMoneyPopup;
        }

        // 팝업 창의 원래 위치 저장
        originalPosition = popupTarget.position;
    }

    // 소지금이 증가할 때 호출되는 메서드
    private void IncreaseMoneyPopup(long amount)
    {
        string formattedAmount = GameManager.Instance.FormatPrice(amount);
        ShowPopup(formattedAmount + "이 입금되었습니다.");
    }

    // 소지금이 감소할 때 호출되는 메서드
    private void DecreaseMoneyPopup(long amount)
    {
        string formattedAmount = GameManager.Instance.FormatPrice(amount);
        ShowPopup(formattedAmount + "이 출금되었습니다.");
    }

    // 팝업을 생성하고 메시지를 설정하는 메서드
    public void ShowPopup(string message)
    {
        countManager.Instance.startEnding(); // 엔딩 체크
        GameObject popupInstance = Instantiate(popupPrefab, popupTarget.position, Quaternion.identity, popupTarget); // popupTarget의 하위에 생성
        TextMeshProUGUI popupText = popupInstance.GetComponentInChildren<TextMeshProUGUI>();
        popupText.text = message;
        SoundManager.instance.PlaySFX(6); // 효과음 출력

        MovePopup(popupInstance, -0.9f);
    }

    // 팝업을 부드럽게 이동시키는 메서드
    private void MovePopup(GameObject popupInstance, float distance)
    {
        // LeanTween을 사용하여 Transform 이동 애니메이션 적용
        LeanTween.moveY(popupInstance, popupInstance.transform.position.y + distance, 0.5f)
            .setEaseOutQuad().setOnComplete(() =>
            {
                LeanTween.moveY(popupInstance, originalPosition.y, 0.5f).setEaseInQuad()
                    .setDelay(2f).setOnComplete(() =>
                    {
                        // 이동 완료 후 생성된 prefab 삭제
                        Destroy(popupInstance);
                    });
            });
    }

    // 메시지 팝업을 생성하고 메시지를 설정하는 메서드
    public void ShowMessage(string message, string img = null)
    {
        GameObject messageInstance = Instantiate(messagePrefab, messageTarget.position, Quaternion.identity, messageTarget); // messageTarget의 하위에 생성
        TextMeshProUGUI messageText = messageInstance.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = message;

        // 이미지 변경 로직 추가
        Image imgTarget = messageInstance.transform.Find("이미지").GetComponent<Image>();
        if (imgTarget != null && ImgList != null && ImgList.Count > 0)
        {
            if (!string.IsNullOrEmpty(img))
            {
                Sprite targetSprite = ImgList.Find(sprite => sprite.name == img);
                if (targetSprite != null)
                {
                    imgTarget.sprite = targetSprite;
                }
                else
                {
                    Debug.LogWarning("Image not found: " + img);
                    imgTarget.sprite = ImgList[0];
                }
            }
            else
            {
                imgTarget.sprite = ImgList[0];
            }
        }

        // 생성된 messagePrefab의 Scale을 0부터 1까지 0.3초 뒤에 0.5초 동안 커지도록 설정
        messageInstance.transform.localScale = Vector3.zero;
        LeanTween.delayedCall(0.3f, () =>
        {
            LeanTween.scale(messageInstance, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
            SoundManager.instance.PlaySFX(7); // 효과음 출력
        });

        // 버튼에 클릭 이벤트 추가
        Button closeButton = messageInstance.GetComponentInChildren<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => RemoveMessage(messageInstance));
        }

    }

    // 엔딩 이후 메세지를 출력하는 메서드
    public void afterEnding()
    {
        GameObject messageInstance = Instantiate(afterEndingPrefab, messageTarget.position, Quaternion.identity, messageTarget); // messageTarget의 하위에 생성

        // 생성된 messagePrefab의 Scale을 0부터 1까지 0.3초 뒤에 0.5초 동안 커지도록 설정
        messageInstance.transform.localScale = Vector3.zero;
        LeanTween.delayedCall(0.3f, () =>
        {
            LeanTween.scale(messageInstance, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
            SoundManager.instance.PlaySFX(7); // 효과음 출력
        });

        // 버튼에 클릭 이벤트 추가
        Transform buttonContainer = messageInstance.transform.Find("버튼");
        if (buttonContainer != null)
        {
            Button yesButton = buttonContainer.Find("yes").GetComponent<Button>();
            Button noButton = buttonContainer.Find("no").GetComponent<Button>();

            if (yesButton == null)
            {
                Debug.LogError("Yes button is not found!");
            }
            if (noButton == null)
            {
                Debug.LogError("No button is not found!");
            }

            if (yesButton != null)
            {
                yesButton.onClick.AddListener(() =>
                {
                    // Yes 버튼: 데이터를 소프트 리셋합니다.
                    // 튜토 진행도만 리셋하지 않음
                    PlayerPrefs.DeleteAll(); // 모든 데이터 삭제

                    ScriptManager.Instance.tutorId = 22;
                    GameManager.Instance.money = 20000; // 초기 소지금 설정
                    GameManager.Instance.didTutorialComplete = true; // 소프트리셋하면 튜토리얼 다시 진행하지 않음
                    ScriptManager.Instance.eventCheck = false;

                    // ClickerManager 초기화
                    ClickerManager.Instance.PerSecond = 60;
                    ClickerManager.Instance.totalEarnings = 0;
                    ClickerManager.Instance.incomePerSecond = 1000;
                    ClickerManager.Instance.incomePerClick = 10;
                    ClickerManager.Instance.ResetClickerItems(); // 초기 아이템 데이터 설정

                    GameManager.Instance.SaveData(); // 초기 데이터 저장

                    ShowPopup("데이터가 초기화 되었습니다.");

                    SoundManager.instance.PlayBGM(0);
                    ScriptManager.Instance.endingChogihwa(); // 엔딩 진행도 초기화

                    Destroy(messageInstance); // 메시지 인스턴스 제거
                });
            }

            if (noButton != null)
            {
                noButton.onClick.AddListener(() =>
                {
                    // No 버튼: 무한모드 시작.
                    Debug.Log("No 버튼 클릭됨");
                    SoundManager.instance.PlayBGM(0);
                    GameManager.Instance.InfiniteMod = true;
                    ShowPopup("무한 모드가 시작 되었습니다.");
                    Destroy(messageInstance); // 메시지 인스턴스 제거
                });
            }
        }
        else
        {
            Debug.LogError("Button container is not found!");
        }
    }

    // ShowMessage로 생성된 GameObject를 제거하는 메서드
    public void RemoveMessage(GameObject messageInstance)
    {
        ScriptManager.Instance.CheckEvent();
        SoundManager.instance.PlaySFX(8); // 효과음 출력

        if (GameManager.Instance.didTutorialComplete == false)
        {
            // tutorId 증가
            ScriptManager.Instance.tutorId++;

            // messageInstance 제거
            Destroy(messageInstance);

            if (ScriptManager.Instance.eventCheck == false)
            {
                // eventCheck가 false인 경우 메시지 팝업을 다시 생성
                if (ScriptManager.Instance.tutorId < ScriptManager.Instance.tutorDialogue.Count)
                {
                    Dialogue nextDialogue = ScriptManager.Instance.tutorDialogue[ScriptManager.Instance.tutorId];
                    ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                }
            }
            else
            {
                // eventCheck가 true인 경우 eventCheck를 false로 설정
                ScriptManager.Instance.eventCheck = false;
            }

            if (ScriptManager.Instance.tutorId == 22)
            {
                GameManager.Instance.didTutorialComplete = true;
            }
        }
        else
        {
            switch (countManager.Instance.curruntEndingID)
            {
                case 0:
                    // endingorId 증가
                    ScriptManager.Instance.endingorId++;

                    // messageInstance 제거
                    Destroy(messageInstance);

                    if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId < ScriptManager.Instance.endingDialogue_1.Count)
                    {
                        Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_1[ScriptManager.Instance.endingorId];
                        ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                    }
                    else if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId == 6)
                    {
                        afterEnding();
                    }
                    break;

                case 1:
                    // endingorId 증가
                    ScriptManager.Instance.endingorId++;

                    // messageInstance 제거
                    Destroy(messageInstance);

                    if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId < ScriptManager.Instance.endingDialogue_2.Count)
                    {
                        Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_2[ScriptManager.Instance.endingorId];
                        ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                    }
                    else if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId == 6)
                    {
                        afterEnding();
                    }
                    break;

                case 2:
                    // endingorId 증가
                    ScriptManager.Instance.endingorId++;

                    // messageInstance 제거
                    Destroy(messageInstance);

                    if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId < ScriptManager.Instance.endingDialogue_3.Count)
                    {
                        Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_3[ScriptManager.Instance.endingorId];
                        ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                    }
                    else if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId == 6)
                    {
                        afterEnding();
                    }
                    break;

                case 3:
                    // endingorId 증가
                    ScriptManager.Instance.endingorId++;

                    // messageInstance 제거
                    Destroy(messageInstance);

                    if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId < ScriptManager.Instance.endingDialogue_4.Count)
                    {
                        Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_4[ScriptManager.Instance.endingorId];
                        ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                    }
                    else if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId == 6)
                    {
                        afterEnding();
                    }
                    break;

                case 4:
                    // endingorId 증가
                    ScriptManager.Instance.endingorId++;

                    // messageInstance 제거
                    Destroy(messageInstance);

                    if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId < ScriptManager.Instance.endingDialogue_5.Count)
                    {
                        Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_5[ScriptManager.Instance.endingorId];
                        ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                    }
                    else if (ScriptManager.Instance != null && ScriptManager.Instance.endingorId == 6)
                    {
                        afterEnding();
                    }
                    break;

                default:
                    break;

            }
        }
    }
}
