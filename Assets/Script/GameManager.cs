using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UIElements;

[System.Serializable]
public struct Product
{
    public string ID;
    public long Price;
    public string Instacontent;
    public Sprite InstaImage; // 새로운 InstaImage 필드 추가
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 인스턴스

    public long money; // 소지금 저장 변수
    public bool didTutorialComplete = false; // 튜토리얼 완료 여부 변수
    public bool InfiniteMod = false; // 무한 모드 변수 추가

    public event Action<long> MoneyIncreased; // 소지금 증가 이벤트
    public event Action<long> MoneyDecreased; // 소지금 감소 이벤트

    //뱃지 관리
    public List<GameObject> TemuBadges; //테무 신규상품의 new 뱃지
    public bool[] TemuBadgeBool = { true, true, true, true, true, true, true, true, true, true}; // 테무 뱃지의 상태 관리
    public int TemuBadgeNum = 10;
    public TextMeshProUGUI TemuBadgeText; // 홈화면 테무 뱃지의 숫자
    private const string TemuBadgeKey = "TBadgeNum"; // 뱃지 상태 저장 키
    private const string TemuBadgeBoolKey = "TBadgeBool"; // 각 뱃지 상태 저장 키 프리픽스

    public GameObject YoutubeBadge;
    public TextMeshProUGUI YoutubeBadgeText; // 홈화면 유튜브 뱃지의 숫자

    public GameObject InstaBadge;
    public TextMeshProUGUI InstaBadgeText;

    public GameObject BankBadge;
    public TextMeshProUGUI BankBadgeText;
    public int BankBadgeInt;
    //뱃지 관리 끝

    [SerializeField]
    private List<Product> products = new List<Product>(10); // 상품 목록

    private const string devilDMKey = "devilDM"; // 나쁜 DM 키
    private const string OdapKey = "Odap"; // 오답 DM 키
    private const string MoneyKey = "Money"; // 소지금 저장 키
    private const string TutorialKey = "DidTutorialComplete"; // 튜토리얼 완료 여부 저장 키
    private const string howfarTutor = "tutorId"; // 튜토리얼 진행도 저장 키
    private const string TotalEarningsKey = "TotalEarnings"; // ClickerManager totalEarnings 저장 키
    private const string IncomePerSecondKey = "IncomePerSecond"; // ClickerManager incomePerSecond 저장 키
    private const string IncomePerClickKey = "IncomePerClick"; // ClickerManager incomePerClick 저장 키
    private const string PerSecondKey = "PerSecond"; // ClickerManager PerSecond 저장 키
    private const string InfiniteModKey = "InfiniteMod"; // InfiniteMod 저장 키

    private void Start()
    {
        Application.targetFrameRate = 60;
        YoutubeBadgeSetting();
        InstarManager.Instance.DmCount = 0;
        InstaBadgeSetting();
        BankBadgeInt = 0;
        BankBadgeSetting();
    }

    private void Awake()
    {
        // 싱글톤 패턴 구현: 인스턴스가 없다면 현재 인스턴스를 설정하고 유지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 로드될 때 객체를 파괴하지 않음

            // ClickerManager 인스턴스가 초기화될 때까지 기다림
            StartCoroutine(InitializeAfterClickerManager());

            Debug.Log("소지금: " + money);
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 방지를 위해 파괴
        }
    }

    public IEnumerator InitializeAfterClickerManager()
    {
        // ClickerManager 인스턴스가 초기화될 때까지 기다림
        while (ClickerManager.Instance == null)
        {
            yield return null;
        }

        LoadData(); // 저장된 데이터 로드

        // 초기 소지금 설정
        if (ScriptManager.Instance != null && ScriptManager.Instance.tutorId == 0)
        {
            money = 20000; // 초기 소지금 설정
            // 아래는 클리커 초기 설정
            ClickerManager.Instance.PerSecond = 60;
            ClickerManager.Instance.incomePerSecond = 1000;
            ClickerManager.Instance.incomePerClick = 10;
            ClickerManager.Instance.totalEarnings = 0;
            ClickerManager.Instance.ResetClickerItems(); // 초기 아이템 데이터 설정
        }

        //뱃지 관리
        TemuBadgeText.text = TemuBadgeNum.ToString();
    }

    private void OnApplicationQuit()
    {
        SaveData(); // 데이터 저장
    }

    // 상품 목록을 반환하는 메서드
    public List<Product> GetProducts()
    {
        return products;
    }

    // 데이터 저장 메서드
    public void SaveData()
    {
        PlayerPrefs.SetString(MoneyKey, money.ToString()); // 소지금 저장
        if (countManager.Instance != null)
        {
            PlayerPrefs.SetString(devilDMKey, countManager.Instance.devilDM.ToString()); // 나쁜디엠 횟수 저장
            PlayerPrefs.SetString(OdapKey, countManager.Instance.Odap.ToString()); // 오답디엠 횟수 저장
        }
        PlayerPrefs.SetInt(PerSecondKey, ClickerManager.Instance.PerSecond); // 퍼세컨드 저장
        PlayerPrefs.SetInt(TutorialKey, didTutorialComplete ? 1 : 0); // 튜토리얼 완료 여부 저장
        if (ScriptManager.Instance != null)
        {
            PlayerPrefs.SetInt(howfarTutor, ScriptManager.Instance.tutorId);
        }
        PlayerPrefs.SetInt(InfiniteModKey, InfiniteMod ? 1 : 0); // InfiniteMod 저장

        // ClickerManager 데이터 저장
        PlayerPrefs.SetString(TotalEarningsKey, ClickerManager.Instance.totalEarnings.ToString());
        PlayerPrefs.SetString(IncomePerSecondKey, ClickerManager.Instance.incomePerSecond.ToString());
        PlayerPrefs.SetString(IncomePerClickKey, ClickerManager.Instance.incomePerClick.ToString());
        ClickerManager.Instance.SaveClickerItems(); // ClickerItem 리스트 저장

        //뱃지 관리
        PlayerPrefs.SetInt(TemuBadgeKey, TemuBadgeNum);

        // 테무 뱃지 상태 저장
        PlayerPrefs.SetInt(TemuBadgeKey, TemuBadgeNum);
        for (int i = 0; i < TemuBadgeBool.Length; i++)
        {
            PlayerPrefs.SetInt(TemuBadgeBoolKey + i, TemuBadgeBool[i] ? 1 : 0); // 각각의 뱃지 상태 저장
        }

        PlayerPrefs.Save(); // PlayerPrefs 저장
    }

    // 데이터 로드 메서드
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(MoneyKey))
        {
            money = long.Parse(PlayerPrefs.GetString(MoneyKey)); // 소지금 로드
        }

        ClickerManager.Instance.PerSecond = PlayerPrefs.GetInt(PerSecondKey, 60); // 퍼세컨드 로드
        if (countManager.Instance != null)
        {
            countManager.Instance.devilDM = PlayerPrefs.GetInt(devilDMKey, 0); // 나쁜답장 횟수 로드
            countManager.Instance.Odap = PlayerPrefs.GetInt(OdapKey, 0); // 오답 횟수 로드
        }
        didTutorialComplete = PlayerPrefs.GetInt(TutorialKey, 0) == 1; // 튜토리얼 완료 여부 로드

        if (ScriptManager.Instance != null)
        {
            if (didTutorialComplete)
            {
                ScriptManager.Instance.tutorId = 22;
            }
            else
            {
                ScriptManager.Instance.tutorId = PlayerPrefs.GetInt(howfarTutor, 0);
            }
        }

        InfiniteMod = PlayerPrefs.GetInt(InfiniteModKey, 0) == 1; // InfiniteMod 로드

        // ClickerManager 데이터 로드
        if (PlayerPrefs.HasKey(TotalEarningsKey))
        {
            ClickerManager.Instance.totalEarnings = long.Parse(PlayerPrefs.GetString(TotalEarningsKey));
        }

        if (PlayerPrefs.HasKey(IncomePerSecondKey))
        {
            ClickerManager.Instance.incomePerSecond = long.Parse(PlayerPrefs.GetString(IncomePerSecondKey));
        }

        if (PlayerPrefs.HasKey(IncomePerClickKey))
        {
            ClickerManager.Instance.incomePerClick = long.Parse(PlayerPrefs.GetString(IncomePerClickKey));
        }

        ClickerManager.Instance.LoadClickerItems(); // ClickerItem 리스트 로드

        //뱃지 관리
        TemuBadgeNum = PlayerPrefs.GetInt(TemuBadgeKey);
        // 테무 뱃지 상태 로드
        TemuBadgeNum = PlayerPrefs.GetInt(TemuBadgeKey, 10); // 기본값은 10
        for (int i = 0; i < TemuBadgeBool.Length; i++)
        {
            if (PlayerPrefs.HasKey(TemuBadgeBoolKey + i))
            {
                TemuBadgeBool[i] = PlayerPrefs.GetInt(TemuBadgeBoolKey + i) == 1;
            }
        }

        // TemuBadgeBool 배열에 따라 TemuBadges 오브젝트 상태 업데이트
        for (int i = 0; i < TemuBadges.Count; i++)
        {
            TemuBadges[i].SetActive(TemuBadgeBool[i]); // 뱃지 활성화/비활성화 설정
        }

        TemuBadgeText.text = TemuBadgeNum.ToString(); // 뱃지 숫자 업데이트
    }

    // 게임 데이터를 초기화하는 메서드
    public void ResetGameData()
    {
        PlayerPrefs.DeleteAll(); // 모든 데이터 삭제

        money = 20000; // 초기 소지금 설정
        didTutorialComplete = false; // 튜토리얼 완료 여부 초기화
        //EventManager.Instance.encounterCount = 0;

        if (ScriptManager.Instance != null)
        {
            ScriptManager.Instance.tutorId = 0;
        }
        InfiniteMod = false; // InfiniteMod 초기화
        if (ScriptManager.Instance != null)
        {
            ScriptManager.Instance.endingChogihwa(); // 엔딩 진행도 초기화
        }

        // ClickerManager 초기화
        ClickerManager.Instance.PerSecond = 60;
        ClickerManager.Instance.totalEarnings = 0;
        ClickerManager.Instance.incomePerSecond = 1000;
        ClickerManager.Instance.incomePerClick = 10;
        ClickerManager.Instance.ResetClickerItems(); // 초기 아이템 데이터 설정

        //뱃지 관리
        for(int i = 0; i < 9; i++)
        {
            TemuBadgeBool[i] = true;
            TemuBadges[i].SetActive(true);
        }
        TemuBadgeNum = 10;
        TemuBadgeText.text = TemuBadgeNum.ToString(); // 뱃지 숫자 업데이트
        YoutubeBadgeSetting();
        InstaBadgeSetting();
        BankBadgeSetting();

        SaveData(); // 초기 데이터 저장
    }

    // 게임 종료 메서드
    public void QuitGame()
    {
        SaveData(); // 데이터 저장 후 게임 종료

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 게임 종료
#else
        Application.Quit(); // 빌드된 게임 종료
#endif
    }

    // 소지금을 설정하는 메서드
    public void SetMoney(long amount)
    {
        if (money != amount)
        {
            long diff = amount - money;
            money = amount;

            // 소지금이 증가한 경우 이벤트 호출
            if (diff > 0)
            {
                MoneyIncreased?.Invoke(diff); // 소지금 증가 이벤트 호출
            }
            // 소지금이 감소한 경우 이벤트 호출
            else if (diff < 0)
            {
                MoneyDecreased?.Invoke(Math.Abs(diff)); // 소지금 감소 이벤트 호출
            }
        }
    }

    // 유튜브 뱃지 설정 메서드
    public void YoutubeBadgeSetting()
    {
        ClickerManager.Instance.UpdateItemPurchaseAvailability();

        if (ClickerManager.Instance.GetPurchasableItemCount() == 0)
        {
            YoutubeBadge.SetActive(false);
        }
        else
        {
            YoutubeBadge.SetActive(true);
            YoutubeBadgeText.text = ClickerManager.Instance.GetPurchasableItemCount().ToString();
        }
    }

    //인스타 뱃지 설정 메서드
    public void InstaBadgeSetting()
    {
        if (InstarManager.Instance.DmCount == 0)
        {
            InstaBadge.SetActive(false);
        }
        else
        {
            InstaBadge.SetActive(true);
            InstaBadgeText.text = InstarManager.Instance.DmCount.ToString();
        }
    }

    //카뱅 뱃지 설정 메서드
    public void BankBadgeSetting()
    {
        if (BankBadgeInt == 0)
        {
            BankBadge.SetActive(false);
        }
        else
        {
            BankBadge.SetActive(true);
            BankBadgeText.text = BankBadgeInt.ToString();
        }
    }

    public void BankIntDel()
    {
        BankBadgeInt = 0;
    }

    // 현재 소지금을 반환하는 메서드
    public long GetMoney()
    {
        return money;
    }

    // 특정 인덱스의 상품을 반환하는 메서드
    public Product GetProduct(int index)
    {
        if (index >= 0 && index < products.Count)
        {
            return products[index];
        }
        return new Product(); // 유효하지 않은 인덱스일 경우 빈 상품 반환
    }

    // 상품 가격을 설정하는 메서드
    public void SetProductPrice(int index, long price)
    {
        if (index >= 0 && index < products.Count)
        {
            Product product = products[index];
            product.Price = price;
            products[index] = product;
        }
    }

    // 상품 ID를 설정하는 메서드
    public void SetProductID(int index, string id)
    {
        if (index >= 0 && index < products.Count)
        {
            Product product = products[index];
            product.ID = id;
            products[index] = product;
        }
    }

    // 상품 인스타 게시글 내용을 설정하는 메서드
    public void SetProductPost(int index, string post)
    {
        if (index >= 0 && index < products.Count)
        {
            Product product = products[index];
            product.Instacontent = post;
            products[index] = product;
        }
    }

    // 가격을 포맷팅하는 메서드
    public string FormatPrice(long price)
    {
        if (price == 0)
        {
            return "0원";
        }

        if (price < 10000)
        {
            return price + "원";
        }

        string[] units = { "", "만", "억", "조", "경" };
        string result = "";
        int index = 0;

        while (price > 0)
        {
            long remainder = price % 10000;
            if (remainder > 0)
            {
                result = remainder + units[index] + " " + result;
            }
            price /= 10000;
            index++;
        }

        return result.Trim() + "원";
    }
}
