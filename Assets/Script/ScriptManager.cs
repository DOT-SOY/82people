using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager Instance; // �̱��� �ν��Ͻ�

    public int tutorId = 0; // ���� Ʃ�丮�� ���� ����
    public int endingorId = 0; // ���� ���� ���� ����

    public List<Dialogue> tutorDialogue = new List<Dialogue>(); // Ʃ�丮�� ���
    public List<Dialogue> endingDialogue_1 = new List<Dialogue>(); // ���� ��� 1
    public List<Dialogue> endingDialogue_2 = new List<Dialogue>(); // ���� ��� 2
    public List<Dialogue> endingDialogue_3 = new List<Dialogue>(); // ���� ��� 3
    public List<Dialogue> endingDialogue_4 = new List<Dialogue>(); // ���� ��� 4
    public List<Dialogue> endingDialogue_5 = new List<Dialogue>(); // ���� ��� 5

    public bool eventCheck = false; // �̺�Ʈ�� üũ�ϴ� ����, �ʱⰪ�� false

    public Image HelperImg; // Inspector���� ������ �� �ִ� Image ����
    public List<Sprite> HelperList = new List<Sprite>(); // �̹��� ����Ʈ
    //public List<GameObject> gameObjectsList = new List<GameObject>(); // Inspector���� ������ �� �ִ� ���ӿ�����Ʈ ����Ʈ

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // ����� tutorId �ҷ�����
        tutorId = PlayerPrefs.GetInt("TutorId", 0);
    }

    public void NextHelperMessage()
    {
        if (ScriptManager.Instance != null && !GameManager.Instance.didTutorialComplete)
        {
            ActivateHelper();
            Dialogue nextDialogue = ScriptManager.Instance.tutorDialogue[ScriptManager.Instance.tutorId];
            PopupManager.Instance.ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
        }
    }

    // tutorId�� Ư�� ���ڰ� �Ǹ� eventCheck�� true�� �����ϴ� �޼���
    // ���⼭ Ʃ�丮�� �б����� üũ�մϴ�.
    public void CheckEvent()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;

        if (clickObject != null)
        {
            Debug.Log(clickObject.name + ": ������ ��ư");
        }
        else
        {
            Debug.Log("No object selected");
        }

        // �޼��� �˾��� ���ߴ� ID 
        if ((tutorId == 2 || tutorId == 6 || tutorId == 8 || tutorId == 10 || tutorId == 12 || tutorId == 16 || tutorId == 19 || tutorId == 21 || endingorId == 5) && eventCheck == false)
        {
            eventCheck = true;
            DeactivateHelper();
        }

        // �޼��� �˾��� �����ϴ� ID 
        if ((tutorId == 0 && eventCheck == false && clickObject != null && clickObject.name == "Ȩ����")
            || (tutorId == 3 && eventCheck == false && clickObject != null && clickObject.name == "ī��")
            || (tutorId == 7 && eventCheck == false && clickObject != null && clickObject.name == "�׹�")
            || (tutorId == 9 && eventCheck == false && clickObject != null && clickObject.name == "���Ź�ư")
            || (tutorId == 11 && eventCheck == false && clickObject != null && clickObject.name == "�ν�Ÿ")
            || (tutorId == 13 && eventCheck == false && clickObject != null && clickObject.name == "��")
            || (tutorId == 17 && eventCheck == false && clickObject != null && (clickObject.name == "�����亯" || clickObject.name == "���۴亯"))
            || (tutorId == 20 && eventCheck == false && clickObject != null && clickObject.name == "��Ʃ��")
            )
        {
            eventCheck = false;
            NextHelperMessage();
        }

    }

    // HelperImg ������Ʈ�� ��Ȱ��ȭ�ϴ� �޼���
    public void DeactivateHelper()
    {
        if (HelperImg != null)
        {
            HelperImg.gameObject.SetActive(false);
        }
    }

    // HelperImg ������Ʈ�� Ȱ��ȭ�ϴ� �޼���
    public void ActivateHelper()
    {
        if (HelperImg != null)
        {
            HelperImg.gameObject.SetActive(true);
        }
    }

    // ���� ���൵�� �ʱ�ȭ��ŵ�ϴ�.
    public void endingChogihwa()
    {
        endingorId = 0;
        countManager.Instance.Odap = 0;
        countManager.Instance.devilDM = 0;
    }
}
