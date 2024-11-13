using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance; // �̱��� �ν��Ͻ�

    private TextMeshProUGUI popupMessageText;
    [SerializeField] private Transform popupTarget;
    [SerializeField] private GameObject popupPrefab; // Prefab�� ������ ����
    [SerializeField] private Transform messageTarget; // Inspector���� ������ �� �ִ� ���� �߰�
    [SerializeField] private GameObject messagePrefab; // Inspector���� ������ �� �ִ� ���� �߰�
    [SerializeField] private GameObject afterEndingPrefab; // Inspector���� ������ �� �ִ� ���� �߰�
    [SerializeField] private List<Sprite> ImgList; // Inspector���� ������ �� �ִ� �̹��� ����Ʈ �߰�
    private Vector3 originalPosition;

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (GameManager.Instance != null)
            {
                // ClickerManager �ν��Ͻ��� �ʱ�ȭ�� ������ ��ٸ�
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
            // GameManager�� MoneyIncreased �̺�Ʈ�� ���� ���� ����
            GameManager.Instance.MoneyIncreased += IncreaseMoneyPopup;

            // GameManager�� MoneyDecreased �̺�Ʈ�� ���� ���� ����
            GameManager.Instance.MoneyDecreased += DecreaseMoneyPopup;
        }

        // �˾� â�� ���� ��ġ ����
        originalPosition = popupTarget.position;
    }

    // �������� ������ �� ȣ��Ǵ� �޼���
    private void IncreaseMoneyPopup(long amount)
    {
        string formattedAmount = GameManager.Instance.FormatPrice(amount);
        ShowPopup(formattedAmount + "�� �ԱݵǾ����ϴ�.");
    }

    // �������� ������ �� ȣ��Ǵ� �޼���
    private void DecreaseMoneyPopup(long amount)
    {
        string formattedAmount = GameManager.Instance.FormatPrice(amount);
        ShowPopup(formattedAmount + "�� ��ݵǾ����ϴ�.");
    }

    // �˾��� �����ϰ� �޽����� �����ϴ� �޼���
    public void ShowPopup(string message)
    {
        countManager.Instance.startEnding(); // ���� üũ
        GameObject popupInstance = Instantiate(popupPrefab, popupTarget.position, Quaternion.identity, popupTarget); // popupTarget�� ������ ����
        TextMeshProUGUI popupText = popupInstance.GetComponentInChildren<TextMeshProUGUI>();
        popupText.text = message;
        SoundManager.instance.PlaySFX(6); // ȿ���� ���

        MovePopup(popupInstance, -0.9f);
    }

    // �˾��� �ε巴�� �̵���Ű�� �޼���
    private void MovePopup(GameObject popupInstance, float distance)
    {
        // LeanTween�� ����Ͽ� Transform �̵� �ִϸ��̼� ����
        LeanTween.moveY(popupInstance, popupInstance.transform.position.y + distance, 0.5f)
            .setEaseOutQuad().setOnComplete(() =>
            {
                LeanTween.moveY(popupInstance, originalPosition.y, 0.5f).setEaseInQuad()
                    .setDelay(2f).setOnComplete(() =>
                    {
                        // �̵� �Ϸ� �� ������ prefab ����
                        Destroy(popupInstance);
                    });
            });
    }

    // �޽��� �˾��� �����ϰ� �޽����� �����ϴ� �޼���
    public void ShowMessage(string message, string img = null)
    {
        GameObject messageInstance = Instantiate(messagePrefab, messageTarget.position, Quaternion.identity, messageTarget); // messageTarget�� ������ ����
        TextMeshProUGUI messageText = messageInstance.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = message;

        // �̹��� ���� ���� �߰�
        Image imgTarget = messageInstance.transform.Find("�̹���").GetComponent<Image>();
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

        // ������ messagePrefab�� Scale�� 0���� 1���� 0.3�� �ڿ� 0.5�� ���� Ŀ������ ����
        messageInstance.transform.localScale = Vector3.zero;
        LeanTween.delayedCall(0.3f, () =>
        {
            LeanTween.scale(messageInstance, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
            SoundManager.instance.PlaySFX(7); // ȿ���� ���
        });

        // ��ư�� Ŭ�� �̺�Ʈ �߰�
        Button closeButton = messageInstance.GetComponentInChildren<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => RemoveMessage(messageInstance));
        }

    }

    // ���� ���� �޼����� ����ϴ� �޼���
    public void afterEnding()
    {
        GameObject messageInstance = Instantiate(afterEndingPrefab, messageTarget.position, Quaternion.identity, messageTarget); // messageTarget�� ������ ����

        // ������ messagePrefab�� Scale�� 0���� 1���� 0.3�� �ڿ� 0.5�� ���� Ŀ������ ����
        messageInstance.transform.localScale = Vector3.zero;
        LeanTween.delayedCall(0.3f, () =>
        {
            LeanTween.scale(messageInstance, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
            SoundManager.instance.PlaySFX(7); // ȿ���� ���
        });

        // ��ư�� Ŭ�� �̺�Ʈ �߰�
        Transform buttonContainer = messageInstance.transform.Find("��ư");
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
                    // Yes ��ư: �����͸� ����Ʈ �����մϴ�.
                    // Ʃ�� ���൵�� �������� ����
                    PlayerPrefs.DeleteAll(); // ��� ������ ����

                    ScriptManager.Instance.tutorId = 22;
                    GameManager.Instance.money = 20000; // �ʱ� ������ ����
                    GameManager.Instance.didTutorialComplete = true; // ����Ʈ�����ϸ� Ʃ�丮�� �ٽ� �������� ����
                    ScriptManager.Instance.eventCheck = false;

                    // ClickerManager �ʱ�ȭ
                    ClickerManager.Instance.PerSecond = 60;
                    ClickerManager.Instance.totalEarnings = 0;
                    ClickerManager.Instance.incomePerSecond = 1000;
                    ClickerManager.Instance.incomePerClick = 10;
                    ClickerManager.Instance.ResetClickerItems(); // �ʱ� ������ ������ ����

                    GameManager.Instance.SaveData(); // �ʱ� ������ ����

                    ShowPopup("�����Ͱ� �ʱ�ȭ �Ǿ����ϴ�.");

                    SoundManager.instance.PlayBGM(0);
                    ScriptManager.Instance.endingChogihwa(); // ���� ���൵ �ʱ�ȭ

                    Destroy(messageInstance); // �޽��� �ν��Ͻ� ����
                });
            }

            if (noButton != null)
            {
                noButton.onClick.AddListener(() =>
                {
                    // No ��ư: ���Ѹ�� ����.
                    Debug.Log("No ��ư Ŭ����");
                    SoundManager.instance.PlayBGM(0);
                    GameManager.Instance.InfiniteMod = true;
                    ShowPopup("���� ��尡 ���� �Ǿ����ϴ�.");
                    Destroy(messageInstance); // �޽��� �ν��Ͻ� ����
                });
            }
        }
        else
        {
            Debug.LogError("Button container is not found!");
        }
    }

    // ShowMessage�� ������ GameObject�� �����ϴ� �޼���
    public void RemoveMessage(GameObject messageInstance)
    {
        ScriptManager.Instance.CheckEvent();
        SoundManager.instance.PlaySFX(8); // ȿ���� ���

        if (GameManager.Instance.didTutorialComplete == false)
        {
            // tutorId ����
            ScriptManager.Instance.tutorId++;

            // messageInstance ����
            Destroy(messageInstance);

            if (ScriptManager.Instance.eventCheck == false)
            {
                // eventCheck�� false�� ��� �޽��� �˾��� �ٽ� ����
                if (ScriptManager.Instance.tutorId < ScriptManager.Instance.tutorDialogue.Count)
                {
                    Dialogue nextDialogue = ScriptManager.Instance.tutorDialogue[ScriptManager.Instance.tutorId];
                    ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                }
            }
            else
            {
                // eventCheck�� true�� ��� eventCheck�� false�� ����
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
                    // endingorId ����
                    ScriptManager.Instance.endingorId++;

                    // messageInstance ����
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
                    // endingorId ����
                    ScriptManager.Instance.endingorId++;

                    // messageInstance ����
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
                    // endingorId ����
                    ScriptManager.Instance.endingorId++;

                    // messageInstance ����
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
                    // endingorId ����
                    ScriptManager.Instance.endingorId++;

                    // messageInstance ����
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
                    // endingorId ����
                    ScriptManager.Instance.endingorId++;

                    // messageInstance ����
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
