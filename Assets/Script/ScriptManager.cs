using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager Instance; // 싱글톤 인스턴스

    public int tutorId = 0; // 현재 튜토리얼 진행 상태
    public int endingorId = 0; // 현재 엔딩 진행 상태

    public List<Dialogue> tutorDialogue = new List<Dialogue>(); // 튜토리얼 대사
    public List<Dialogue> endingDialogue_1 = new List<Dialogue>(); // 엔딩 대사 1
    public List<Dialogue> endingDialogue_2 = new List<Dialogue>(); // 엔딩 대사 2
    public List<Dialogue> endingDialogue_3 = new List<Dialogue>(); // 엔딩 대사 3
    public List<Dialogue> endingDialogue_4 = new List<Dialogue>(); // 엔딩 대사 4
    public List<Dialogue> endingDialogue_5 = new List<Dialogue>(); // 엔딩 대사 5

    public bool eventCheck = false; // 이벤트를 체크하는 변수, 초기값은 false

    public Image HelperImg; // Inspector에서 지정할 수 있는 Image 변수
    public List<Sprite> HelperList = new List<Sprite>(); // 이미지 리스트
    //public List<GameObject> gameObjectsList = new List<GameObject>(); // Inspector에서 지정할 수 있는 게임오브젝트 리스트

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 저장된 tutorId 불러오기
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

    // tutorId가 특정 숫자가 되면 eventCheck를 true로 설정하는 메서드
    // 여기서 튜토리얼 분기점을 체크합니다.
    public void CheckEvent()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;

        if (clickObject != null)
        {
            Debug.Log(clickObject.name + ": 선택한 버튼");
        }
        else
        {
            Debug.Log("No object selected");
        }

        // 메세지 팝업을 멈추는 ID 
        if ((tutorId == 2 || tutorId == 6 || tutorId == 8 || tutorId == 10 || tutorId == 12 || tutorId == 16 || tutorId == 19 || tutorId == 21 || endingorId == 5) && eventCheck == false)
        {
            eventCheck = true;
            DeactivateHelper();
        }

        // 메세지 팝업을 시작하는 ID 
        if ((tutorId == 0 && eventCheck == false && clickObject != null && clickObject.name == "홈으로")
            || (tutorId == 3 && eventCheck == false && clickObject != null && clickObject.name == "카뱅")
            || (tutorId == 7 && eventCheck == false && clickObject != null && clickObject.name == "테무")
            || (tutorId == 9 && eventCheck == false && clickObject != null && clickObject.name == "구매버튼")
            || (tutorId == 11 && eventCheck == false && clickObject != null && clickObject.name == "인스타")
            || (tutorId == 13 && eventCheck == false && clickObject != null && clickObject.name == "디엠")
            || (tutorId == 17 && eventCheck == false && clickObject != null && (clickObject.name == "좋은답변" || clickObject.name == "나쁜답변"))
            || (tutorId == 20 && eventCheck == false && clickObject != null && clickObject.name == "유튜브")
            )
        {
            eventCheck = false;
            NextHelperMessage();
        }

    }

    // HelperImg 오브젝트를 비활성화하는 메서드
    public void DeactivateHelper()
    {
        if (HelperImg != null)
        {
            HelperImg.gameObject.SetActive(false);
        }
    }

    // HelperImg 오브젝트를 활성화하는 메서드
    public void ActivateHelper()
    {
        if (HelperImg != null)
        {
            HelperImg.gameObject.SetActive(true);
        }
    }

    // 엔딩 진행도를 초기화시킵니다.
    public void endingChogihwa()
    {
        endingorId = 0;
        countManager.Instance.Odap = 0;
        countManager.Instance.devilDM = 0;
    }
}
