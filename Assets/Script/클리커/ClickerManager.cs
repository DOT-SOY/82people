using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class ClickerItem
{
    public long itemCost; // �������� �ݾ�
    public int itemID; // �������� ID
    public Button itemButton; // �������� ��ư ����
    public bool canPurchase; // ���� ���� ���� ����
    public float costByeondong; // ��� ���� ����
    public float floatByeondong; // �ٸ� ����
}

public class ClickerManager : MonoBehaviour
{
    public static ClickerManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    public int PerSecond; // �� �ʴ� ���� ������?
    public long incomePerSecond; // �ʴ� ���� �ݾ�
    public long incomePerClick; // Ŭ���� ���� �ݾ�
    public long totalEarnings; // ���ݱ��� �� �ݾ�

    public List<ClickerItem> items; // �������� �ݾ�, ID �� ���� Ƚ���� ������ ����Ʈ

    // �߰��� ����
    public List<Sprite> Youtube; // Inspector���� ������ �� �ִ� �̹��� ����Ʈ
    public Image YoutubeThumbnail; // Inspector���� ������ �� �ִ� �̹���
    public TextMeshProUGUI YoutubeJemok; // Inspector���� ������ �� �ִ� TextMeshPro ����
    public TextMeshProUGUI YoutubeMoney; // Inspector���� ������ �� �ִ� TextMeshPro ����

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
        UpdateYoutubeMoney();
    }

    // �޼��忡 string�� �����ϸ� string�� ������ �̸��� ���� Youtube�� ��ҷ� YoutubeThumbnail�� �̹����� �����ϴ� �޼���
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

    // �޼��忡 string�� �����ϸ� YoutubeJemok�� ������ string���� �����ϴ� �޼���
    public void SetYoutubeJemok(string title)
    {
        if (YoutubeJemok != null)
        {
            switch (title)
            {
                case ("������"):
                    YoutubeJemok.text = "�ƴ� ���� ���� �������� �ִٰ��? ���� ���� �Ծ�ý��ϴ�";
                    SetYoutubeThumbnail("������");
                    break;

                case ("å"):
                    YoutubeJemok.text = "�̰� �ϳ��� �����е� ���� ����� �˴ϴ�!";
                    SetYoutubeThumbnail("å");
                    break;

                case ("ȭ��"):
                    YoutubeJemok.text = "�� ȭ�� ���п� ���� ������ ������ �ʾƿ䤻��";
                    SetYoutubeThumbnail("ȭ��");
                    break;

                case ("����"):
                    YoutubeJemok.text = "�� �λ� ������ ã�ҽ��ϴ�...";
                    SetYoutubeThumbnail("����");
                    break;

                case ("����"):
                    YoutubeJemok.text = "�� �ڿ�, �ư���.";
                    SetYoutubeThumbnail("����");
                    break;

                case ("��"):
                    YoutubeJemok.text = "�ƴ� ���� �ڵ����� 13���̳� ����䤻�� ...���? �� ���� �� ��� ������...?";
                    SetYoutubeThumbnail("��");
                    break;

                case ("��Ʈ��"):
                    YoutubeJemok.text = "�ΰ����� �񼭰� ž��� ��Ʈ���� �ִ�? �ٵ� �ΰ������� �ں񽺰� �ƴ϶� ��Ʈ���̶���??!";
                    SetYoutubeThumbnail("��Ʈ��");
                    break;

                case ("�뷡��"):
                    YoutubeJemok.text = "���� �� �������� �̰� ������? (��ȸ ��)";
                    SetYoutubeThumbnail("�뷡��");
                    break;

                case ("������"):
                    YoutubeJemok.text = "��� ���� ���� ���� �ִ� ���� �־���";
                    SetYoutubeThumbnail("������");
                    break;

                case ("�ѳ�����"):
                    YoutubeJemok.text = "����Ÿ ���Ͽ��� ����Ʈ�� ��������?! �̰� ��������� ����";
                    SetYoutubeThumbnail("�ѳ�����");
                    break;

                default:
                    YoutubeJemok.text = "����Ÿ ���� ���� Vlog....<sprite index=38>";
                    break;
            }
        }
        else
        {
            Debug.LogWarning("YoutubeJemok is not assigned.");
        }
    }

    // YoutubeMoney ������ totalEarnings���� �����ϴ� �޼���
    public void UpdateYoutubeMoney()
    {
        UpdateItemPurchaseAvailability();
        if (YoutubeMoney != null)
        {
            YoutubeMoney.text = "�������� ������ ���Ծ� ���� <color=#FF0000>" + GameManager.Instance.FormatPrice(totalEarnings) + "</color>�� �𿴾��!<br><color=#FF0000>" + PerSecond + "</color>�ʿ� <color=#FF0000>" + incomePerSecond + "</color>���̳� �� ������ ���ְ� ��ôٴ�?!<br>�����̿���~ ��";
        }
        else
        {
            Debug.LogWarning("YoutubeMoney is not assigned.");
        }
    }

    // ������� ���� �ݾ��� �����ϴ� �޼���
    public void AddTotalEarningsToMoney()
    {
        string formattedSum = GameManager.Instance.FormatPrice(totalEarnings);
        GameManager.Instance.SetMoney(GameManager.Instance.money + totalEarnings);
        BankManager.Instance.CreateBankPrefab($"[��Ʃ�� �ڸ���] {formattedSum} �Ա�", 3);
        totalEarnings = 0; // ���� �ݾ��� 0���� ����ϴ�.
        UpdateYoutubeMoney();
    }

    // ��� �������� ���� ���� ���θ� ������Ʈ�ϴ� �޼���
    public void UpdateItemPurchaseAvailability()
    {
        foreach (var item in items)
        {
            item.canPurchase = GameManager.Instance.money >= item.itemCost;
            item.itemButton.interactable = item.canPurchase;
        }
    }

    // Ư�� �������� �����ϴ� �޼���
    public void PurchaseItem(int itemID)
    {
        ClickerItem item = items.Find(i => i.itemID == itemID);
        if (item != null)
        {
            if (item.canPurchase && GameManager.Instance.money >= item.itemCost)
            {
                BankManager.Instance.CreateBankPrefab($"[��Ʃ�� �ڸ���] {item.itemCost} ���", 3);
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

    // incomePerSecond�� floatByeondong��ŭ ���ϰ� itemCost�� costByeondong��ŭ ���ϴ� �޼���
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

    // incomePerClick�� floatByeondong��ŭ ���ϰ� itemCost�� costByeondong��ŭ ���ϴ� �޼���
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

    // PerSecond�� floatByeondong��ŭ ���ϰ� itemCost�� costByeondong��ŭ ���ϴ� �޼���
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

    // ������ �ʱ�ȭ
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

    // ClickerItem ����Ʈ�� �����ϴ� �޼���
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

    // ClickerItem ����Ʈ�� �ε��ϴ� �޼���
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

    // ClickerItem ����Ʈ�� �ʱ�ȭ�ϴ� �޼���
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
            items[i].canPurchase = false; // �⺻������ ����
        }
    }

    // canPurchase�� true�� �������� ������ ��ȯ�ϴ� �޼���
    public int GetPurchasableItemCount()
    {
        return items.Count(item => item.canPurchase == true);
    }
}
