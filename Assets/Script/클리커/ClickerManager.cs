using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class ClickerItem
{
    public long itemCost; // 아이템의 금액
    public int itemID; // 아이템의 ID
    public Button itemButton; // 아이템의 버튼 변수
    public bool canPurchase; // 구매 가능 여부 변수
    public float costByeondong; // 비용 변경 변수
    public float floatByeondong; // 다른 변수
}

public class ClickerManager : MonoBehaviour
{
    public static ClickerManager Instance { get; private set; } // 싱글톤 인스턴스

    public int PerSecond; // 몇 초당 돈을 버나요?
    public long incomePerSecond; // 초당 버는 금액
    public long incomePerClick; // 클릭당 버는 금액
    public long totalEarnings; // 지금까지 번 금액

    public List<ClickerItem> items; // 아이템의 금액, ID 및 구매 횟수를 저장할 리스트

    // 추가된 변수
    public List<Sprite> Youtube; // Inspector에서 설정할 수 있는 이미지 리스트
    public Image YoutubeThumbnail; // Inspector에서 설정할 수 있는 이미지
    public TextMeshProUGUI YoutubeJemok; // Inspector에서 설정할 수 있는 TextMeshPro 변수
    public TextMeshProUGUI YoutubeMoney; // Inspector에서 설정할 수 있는 TextMeshPro 변수

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
        UpdateYoutubeMoney();
    }

    // 메서드에 string을 전달하면 string과 동일한 이름을 가진 Youtube의 요소로 YoutubeThumbnail의 이미지를 변경하는 메서드
    public void SetYoutubeThumbnail(string imageName)
    {
        Sprite newThumbnail = Youtube.Find(sprite => sprite.name == imageName);
        if (newThumbnail != null)
        {
            YoutubeThumbnail.sprite = newThumbnail;
        }
        else
        {
            Debug.LogWarning("Image not found: " + imageName);
        }
    }

    // 메서드에 string을 전달하면 YoutubeJemok의 내용을 string으로 변경하는 메서드
    public void SetYoutubeJemok(string title)
    {
        if (YoutubeJemok != null)
        {
            switch (title)
            {
                case ("마라탕"):
                    YoutubeJemok.text = "아니 세상에 제로 마라탕이 있다고요? 제가 직접 먹어봤습니다";
                    SetYoutubeThumbnail("마라탕");
                    break;

                case ("책"):
                    YoutubeJemok.text = "이거 하나면 여러분도 엠벼 고수가 됩니다!";
                    SetYoutubeThumbnail("책");
                    break;

                case ("화분"):
                    YoutubeJemok.text = "이 화분 덕분에 매일 웃음이 끊이지 않아요ㅋㅋ";
                    SetYoutubeThumbnail("화분");
                    break;

                case ("게장"):
                    YoutubeJemok.text = "제 인생 게장을 찾았습니다...";
                    SetYoutubeThumbnail("게장");
                    break;

                case ("집사"):
                    YoutubeJemok.text = "잘 자요, 아가씨.";
                    SetYoutubeThumbnail("집사");
                    break;

                case ("폰"):
                    YoutubeJemok.text = "아니 누가 핸드폰을 13번이나 접어요ㅋㅋ ...어라? 내 폰이 왜 계속 접히지...?";
                    SetYoutubeThumbnail("폰");
                    break;

                case ("노트북"):
                    YoutubeJemok.text = "인공지능 비서가 탑재된 노트북이 있다? 근데 인공지능이 자비스가 아니라 울트론이라고요??!";
                    SetYoutubeThumbnail("노트북");
                    break;

                case ("노래방"):
                    YoutubeJemok.text = "내가 왜 이제서야 이걸 샀을까? (후회 중)";
                    SetYoutubeThumbnail("노래방");
                    break;

                case ("세차기"):
                    YoutubeJemok.text = "사고 나서 매일 쓰고 있는 나의 최애템";
                    SetYoutubeThumbnail("세차기");
                    break;

                case ("한남더힐"):
                    YoutubeJemok.text = "감스타 마켓에서 아파트를 공동구매?! 이건 대박이지예 ㅋㅋ";
                    SetYoutubeThumbnail("한남더힐");
                    break;

                default:
                    YoutubeJemok.text = "감스타 마켓 오픈 Vlog....<sprite index=38>";
                    break;
            }
        }
        else
        {
            Debug.LogWarning("YoutubeJemok is not assigned.");
        }
    }

    // YoutubeMoney 내용을 totalEarnings으로 변경하는 메서드
    public void UpdateYoutubeMoney()
    {
        UpdateItemPurchaseAvailability();
        if (YoutubeMoney != null)
        {
            YoutubeMoney.text = "여러분의 성원에 힘입어 벌써 <color=#FF0000>" + GameManager.Instance.FormatPrice(totalEarnings) + "</color>이 모였어요!<br><color=#FF0000>" + PerSecond + "</color>초에 <color=#FF0000>" + incomePerSecond + "</color>명이나 제 영상을 봐주고 계시다니?!<br>영광이에요~ ㅎ";
        }
        else
        {
            Debug.LogWarning("YoutubeMoney is not assigned.");
        }
    }

    // 현재까지 쌓인 금액을 수령하는 메서드
    public void AddTotalEarningsToMoney()
    {
        string formattedSum = GameManager.Instance.FormatPrice(totalEarnings);
        GameManager.Instance.SetMoney(GameManager.Instance.money + totalEarnings);
        BankManager.Instance.CreateBankPrefab($"[미튜브 코리아] {formattedSum} 입금", 3);
        totalEarnings = 0; // 쌓인 금액을 0으로 만듭니다.
        UpdateYoutubeMoney();
    }

    // 모든 아이템의 구매 가능 여부를 업데이트하는 메서드
    public void UpdateItemPurchaseAvailability()
    {
        foreach (var item in items)
        {
            item.canPurchase = GameManager.Instance.money >= item.itemCost;
            item.itemButton.interactable = item.canPurchase;
        }
    }

    // 특정 아이템을 구매하는 메서드
    public void PurchaseItem(int itemID)
    {
        ClickerItem item = items.Find(i => i.itemID == itemID);
        if (item != null)
        {
            if (item.canPurchase && GameManager.Instance.money >= item.itemCost)
            {
                BankManager.Instance.CreateBankPrefab($"[미튜브 코리아] {item.itemCost} 출금", 3);
                GameManager.Instance.SetMoney(GameManager.Instance.money - item.itemCost);
                UpdateItemPurchaseAvailability();
            }
            else
            {
                Debug.LogWarning("Cannot purchase item: " + itemID);
            }
        }
        UpdateItemPurchaseAvailability();
    }

    // incomePerSecond를 floatByeondong만큼 곱하고 itemCost를 costByeondong만큼 곱하는 메서드
    public void IncreaseIncomePerSecond(int itemID)
    {
        ClickerItem item = items.Find(i => i.itemID == itemID);
        if (item != null && item.canPurchase)
        {
            incomePerSecond = (long)(incomePerSecond * item.floatByeondong);
            item.itemCost = (long)(item.itemCost * item.costByeondong);
            PurchaseItem(itemID);
        }
    }

    // incomePerClick을 floatByeondong만큼 곱하고 itemCost를 costByeondong만큼 곱하는 메서드
    public void IncreaseIncomePerClick(int itemID)
    {
        ClickerItem item = items.Find(i => i.itemID == itemID);
        if (item != null && item.canPurchase)
        {
            incomePerClick = (long)(incomePerClick * item.floatByeondong);
            item.itemCost = (long)(item.itemCost * item.costByeondong);
            PurchaseItem(itemID);
        }
    }

    // PerSecond를 floatByeondong만큼 곱하고 itemCost를 costByeondong만큼 곱하는 메서드
    public void IncreasePerSecond(int itemID)
    {
        ClickerItem item = items.Find(i => i.itemID == itemID);
        if (item != null && item.canPurchase)
        {
            PerSecond = (int)(PerSecond * item.floatByeondong);
            item.itemCost = (long)(item.itemCost * item.costByeondong);
            PurchaseItem(itemID);
        }
    }

    // 아이템 초기화
    private void InitializeItems()
    {
        items = new List<ClickerItem>
        {
            new ClickerItem { itemCost = 10000, itemID = 0, costByeondong = 1.2f, floatByeondong = 1.5f, canPurchase = false },
            new ClickerItem { itemCost = 50000, itemID = 1, costByeondong = 1.3f, floatByeondong = 0.9f, canPurchase = false },
            new ClickerItem { itemCost = 100000, itemID = 2, costByeondong = 1.4f, floatByeondong = 1.6f, canPurchase = false },
            new ClickerItem { itemCost = 200000, itemID = 3, costByeondong = 1.5f, floatByeondong = 1.7f, canPurchase = false },
            new ClickerItem { itemCost = 500000, itemID = 4, costByeondong = 1.6f, floatByeondong = 0.7f, canPurchase = false },
            new ClickerItem { itemCost = 1000000, itemID = 5, costByeondong = 1.7f, floatByeondong = 1.8f, canPurchase = false },
            new ClickerItem { itemCost = 2000000, itemID = 6, costByeondong = 1.8f, floatByeondong = 1.9f, canPurchase = false },
            new ClickerItem { itemCost = 5000000, itemID = 7, costByeondong = 1.9f, floatByeondong = 2f, canPurchase = false },
            new ClickerItem { itemCost = 10000000, itemID = 8, costByeondong = 2f, floatByeondong = 2.1f, canPurchase = false },
        };
    }

    // ClickerItem 리스트를 저장하는 메서드
    public void SaveClickerItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            PlayerPrefs.SetString($"ClickerItem_{i}_itemCost", items[i].itemCost.ToString());
            PlayerPrefs.SetInt($"ClickerItem_{i}_itemID", items[i].itemID);
            PlayerPrefs.SetFloat($"ClickerItem_{i}_costByeondong", items[i].costByeondong);
            PlayerPrefs.SetFloat($"ClickerItem_{i}_floatByeondong", items[i].floatByeondong);
            PlayerPrefs.SetInt($"ClickerItem_{i}_canPurchase", items[i].canPurchase ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    // ClickerItem 리스트를 로드하는 메서드
    public void LoadClickerItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (PlayerPrefs.HasKey($"ClickerItem_{i}_itemCost"))
            {
                items[i].itemCost = long.Parse(PlayerPrefs.GetString($"ClickerItem_{i}_itemCost"));
                items[i].itemID = PlayerPrefs.GetInt($"ClickerItem_{i}_itemID");
                items[i].costByeondong = PlayerPrefs.GetFloat($"ClickerItem_{i}_costByeondong");
                items[i].floatByeondong = PlayerPrefs.GetFloat($"ClickerItem_{i}_floatByeondong");
                items[i].canPurchase = PlayerPrefs.GetInt($"ClickerItem_{i}_canPurchase") == 1;
            }
        }
    }

    // ClickerItem 리스트를 초기화하는 메서드
    public void ResetClickerItems()
    {
        var initialValues = new List<(long itemCost, int itemID, float costByeondong, float floatByeondong)>
    {
        (10000, 0, 1.2f, 1.5f),
        (50000, 1, 1.3f, 0.9f),
        (100000, 2, 1.4f, 1.6f),
        (200000, 3, 1.5f, 1.7f),
        (500000, 4, 1.6f, 0.7f),
        (1000000, 5, 1.7f, 1.8f),
        (2000000, 6, 1.8f, 1.9f),
        (5000000, 7, 1.9f, 2f),
        (10000000, 8, 2f, 2.1f)
    };

        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemCost = initialValues[i].itemCost;
            items[i].itemID = initialValues[i].itemID;
            items[i].costByeondong = initialValues[i].costByeondong;
            items[i].floatByeondong = initialValues[i].floatByeondong;
            items[i].canPurchase = false; // 기본값으로 설정
        }
    }

    // canPurchase가 true인 아이템의 개수를 반환하는 메서드
    public int GetPurchasableItemCount()
    {
        return items.Count(item => item.canPurchase == true);
    }
}
