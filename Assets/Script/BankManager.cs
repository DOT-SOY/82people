using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BankManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BankManager Instance;

    // 이벤트매니저 연결
    public EventManager eventManager; // Inspector에서 할당

    // Inspector에서 지정할 수 있는 TextMeshPro 변수
    public TextMeshProUGUI curruntMoney;

    // Inspector에서 지정할 수 있는 Image 리스트 변수
    public List<Sprite> bankImage;

    // Inspector에서 지정할 수 있는 프리팹 변수
    public GameObject bankPrefab;

    // Inspector에서 지정할 수 있는 게임 오브젝트 변수
    public GameObject bankTarget;

    // 생성된 프리팹을 관리하는 리스트
    private List<GameObject> createdPrefabs = new List<GameObject>();

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // curruntMoney의 내용을 업데이트하는 메서드
    public void UpdateCurruntMoney()
    {
        // GameManager.Instance.GetMoney()를 호출하여 반환된 값을 GameManager.Instance.FormatPrice()로 포맷
        long money = GameManager.Instance.GetMoney();
        string formattedMoney = GameManager.Instance.FormatPrice(money);

        // curruntMoney의 텍스트를 포맷된 값으로 변경
        if (curruntMoney != null)
        {
            curruntMoney.text = formattedMoney;
        }
    }

    // 템프 -> 은행 메서드
    public void temuToBank(string message)
    {
        CreateBankPrefab(message, 0);
    }

    // 카카오 -> 은행 메서드
    public void cacaoToBank(string message)
    {
        CreateBankPrefab(message, 1);
    }

    // 사람 -> 은행 메서드
    public void humanToBank(string message)
    {
        CreateBankPrefab(message, 2);
    }

    // 유튜브 -> 은행 메서드
    public void youtubeToBank(string message)
    {
        CreateBankPrefab(message, 3);
    }

    // 이벤트 -> 은행 메서드
    public void EventToBank(string message)
    {
        CreateBankPrefab(message, 4);
    }

    // 공통된 프리팹 생성 메서드
    public void CreateBankPrefab(string message, int imageIndex)
    {
        if (bankPrefab != null && bankTarget != null && bankImage != null && bankImage.Count > imageIndex)
        {
            // bankTarget 하위에 prefab 생성
            GameObject newBankPrefab = Instantiate(bankPrefab, bankTarget.transform);

            // 생성된 prefab 안의 이미지 컴포넌트 변경
            Image imageComponent = newBankPrefab.GetComponentInChildren<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = bankImage[imageIndex];
            }

            // 생성된 prefab 안의 TextMeshPro 컴포넌트 변경
            TextMeshProUGUI textComponent = newBankPrefab.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = message;
            }

            // 생성된 프리팹을 리스트에 추가
            createdPrefabs.Add(newBankPrefab);

            // 프리팹의 갯수가 30개를 넘으면 가장 오래된 프리팹 제거
            if (createdPrefabs.Count > 30)
            {
                Destroy(createdPrefabs[0]);
                createdPrefabs.RemoveAt(0);
            }
        }

        GameManager.Instance.BankBadgeInt++;

        
        // 아래는 랜덤 이벤트 인카운터 함수 호출
        eventManager.RandomEncounter();
        
    }
}
