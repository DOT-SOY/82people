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
    public Sprite InstaImage; // ���ο� InstaImage �ʵ� �߰�
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // �̱��� �ν��Ͻ�

    public long money; // ������ ���� ����
    public bool didTutorialComplete = false; // Ʃ�丮�� �Ϸ� ���� ����
    public bool InfiniteMod = false; // ���� ��� ���� �߰�

    public event Action<long> MoneyIncreased; // ������ ���� �̺�Ʈ
    public event Action<long> MoneyDecreased; // ������ ���� �̺�Ʈ

    //���� ����
    public List<GameObject> TemuBadges; //�׹� �űԻ�ǰ�� new ����
    public bool[] TemuBadgeBool = { true, true, true, true, true, true, true, true, true, true}; // �׹� ������ ���� ����
    public int TemuBadgeNum = 10;
    public TextMeshProUGUI TemuBadgeText; // Ȩȭ�� �׹� ������ ����
    private const string TemuBadgeKey = "TBadgeNum"; // ���� ���� ���� Ű
    private const string TemuBadgeBoolKey = "TBadgeBool"; // �� ���� ���� ���� Ű �����Ƚ�

    public GameObject YoutubeBadge;
    public TextMeshProUGUI YoutubeBadgeText; // Ȩȭ�� ��Ʃ�� ������ ����

    public GameObject InstaBadge;
    public TextMeshProUGUI InstaBadgeText;

    public GameObject BankBadge;
    public TextMeshProUGUI BankBadgeText;
    public int BankBadgeInt;
    //���� ���� ��

    [SerializeField]
    private List<Product> products = new List<Product>(10); // ��ǰ ���

    private const string devilDMKey = "devilDM"; // ���� DM Ű
    private const string OdapKey = "Odap"; // ���� DM Ű
    private const string MoneyKey = "Money"; // ������ ���� Ű
    private const string TutorialKey = "DidTutorialComplete"; // Ʃ�丮�� �Ϸ� ���� ���� Ű
    private const string howfarTutor = "tutorId"; // Ʃ�丮�� ���൵ ���� Ű
    private const string TotalEarningsKey = "TotalEarnings"; // ClickerManager totalEarnings ���� Ű
    private const string IncomePerSecondKey = "IncomePerSecond"; // ClickerManager incomePerSecond ���� Ű
    private const string IncomePerClickKey = "IncomePerClick"; // ClickerManager incomePerClick ���� Ű
    private const string PerSecondKey = "PerSecond"; // ClickerManager PerSecond ���� Ű
    private const string InfiniteModKey = "InfiniteMod"; // InfiniteMod ���� Ű

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
        // �̱��� ���� ����: �ν��Ͻ��� ���ٸ� ���� �ν��Ͻ��� �����ϰ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ε�� �� ��ü�� �ı����� ����

            // ClickerManager �ν��Ͻ��� �ʱ�ȭ�� ������ ��ٸ�
            StartCoroutine(InitializeAfterClickerManager());

            Debug.Log("������: " + money);
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ߺ� ������ ���� �ı�
        }
    }

    public IEnumerator InitializeAfterClickerManager()
    {
        // ClickerManager �ν��Ͻ��� �ʱ�ȭ�� ������ ��ٸ�
        while (ClickerManager.Instance == null)
        {
            yield return null;
        }

        LoadData(); // ����� ������ �ε�

        // �ʱ� ������ ����
        if (ScriptManager.Instance != null && ScriptManager.Instance.tutorId == 0)
        {
            money = 20000; // �ʱ� ������ ����
            // �Ʒ��� Ŭ��Ŀ �ʱ� ����
            ClickerManager.Instance.PerSecond = 60;
            ClickerManager.Instance.incomePerSecond = 1000;
            ClickerManager.Instance.incomePerClick = 10;
            ClickerManager.Instance.totalEarnings = 0;
            ClickerManager.Instance.ResetClickerItems(); // �ʱ� ������ ������ ����
        }

        //���� ����
        TemuBadgeText.text = TemuBadgeNum.ToString();
    }

    private void OnApplicationQuit()
    {
        SaveData(); // ������ ����
    }

    // ��ǰ ����� ��ȯ�ϴ� �޼���
    public List<Product> GetProducts()
    {
        return products;
    }

    // ������ ���� �޼���
    public void SaveData()
    {
        PlayerPrefs.SetString(MoneyKey, money.ToString()); // ������ ����
        if (countManager.Instance != null)
        {
            PlayerPrefs.SetString(devilDMKey, countManager.Instance.devilDM.ToString()); // ���۵� Ƚ�� ����
            PlayerPrefs.SetString(OdapKey, countManager.Instance.Odap.ToString()); // ����� Ƚ�� ����
        }
        PlayerPrefs.SetInt(PerSecondKey, ClickerManager.Instance.PerSecond); // �ۼ����� ����
        PlayerPrefs.SetInt(TutorialKey, didTutorialComplete ? 1 : 0); // Ʃ�丮�� �Ϸ� ���� ����
        if (ScriptManager.Instance != null)
        {
            PlayerPrefs.SetInt(howfarTutor, ScriptManager.Instance.tutorId);
        }
        PlayerPrefs.SetInt(InfiniteModKey, InfiniteMod ? 1 : 0); // InfiniteMod ����

        // ClickerManager ������ ����
        PlayerPrefs.SetString(TotalEarningsKey, ClickerManager.Instance.totalEarnings.ToString());
        PlayerPrefs.SetString(IncomePerSecondKey, ClickerManager.Instance.incomePerSecond.ToString());
        PlayerPrefs.SetString(IncomePerClickKey, ClickerManager.Instance.incomePerClick.ToString());
        ClickerManager.Instance.SaveClickerItems(); // ClickerItem ����Ʈ ����

        //���� ����
        PlayerPrefs.SetInt(TemuBadgeKey, TemuBadgeNum);

        // �׹� ���� ���� ����
        PlayerPrefs.SetInt(TemuBadgeKey, TemuBadgeNum);
        for (int i = 0; i < TemuBadgeBool.Length; i++)
        {
            PlayerPrefs.SetInt(TemuBadgeBoolKey + i, TemuBadgeBool[i] ? 1 : 0); // ������ ���� ���� ����
        }

        PlayerPrefs.Save(); // PlayerPrefs ����
    }

    // ������ �ε� �޼���
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(MoneyKey))
        {
            money = long.Parse(PlayerPrefs.GetString(MoneyKey)); // ������ �ε�
        }

        ClickerManager.Instance.PerSecond = PlayerPrefs.GetInt(PerSecondKey, 60); // �ۼ����� �ε�
        if (countManager.Instance != null)
        {
            countManager.Instance.devilDM = PlayerPrefs.GetInt(devilDMKey, 0); // ���۴��� Ƚ�� �ε�
            countManager.Instance.Odap = PlayerPrefs.GetInt(OdapKey, 0); // ���� Ƚ�� �ε�
        }
        didTutorialComplete = PlayerPrefs.GetInt(TutorialKey, 0) == 1; // Ʃ�丮�� �Ϸ� ���� �ε�

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

        InfiniteMod = PlayerPrefs.GetInt(InfiniteModKey, 0) == 1; // InfiniteMod �ε�

        // ClickerManager ������ �ε�
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

        ClickerManager.Instance.LoadClickerItems(); // ClickerItem ����Ʈ �ε�

        //���� ����
        TemuBadgeNum = PlayerPrefs.GetInt(TemuBadgeKey);
        // �׹� ���� ���� �ε�
        TemuBadgeNum = PlayerPrefs.GetInt(TemuBadgeKey, 10); // �⺻���� 10
        for (int i = 0; i < TemuBadgeBool.Length; i++)
        {
            if (PlayerPrefs.HasKey(TemuBadgeBoolKey + i))
            {
                TemuBadgeBool[i] = PlayerPrefs.GetInt(TemuBadgeBoolKey + i) == 1;
            }
        }

        // TemuBadgeBool �迭�� ���� TemuBadges ������Ʈ ���� ������Ʈ
        for (int i = 0; i < TemuBadges.Count; i++)
        {
            TemuBadges[i].SetActive(TemuBadgeBool[i]); // ���� Ȱ��ȭ/��Ȱ��ȭ ����
        }

        TemuBadgeText.text = TemuBadgeNum.ToString(); // ���� ���� ������Ʈ
    }

    // ���� �����͸� �ʱ�ȭ�ϴ� �޼���
    public void ResetGameData()
    {
        PlayerPrefs.DeleteAll(); // ��� ������ ����

        money = 20000; // �ʱ� ������ ����
        didTutorialComplete = false; // Ʃ�丮�� �Ϸ� ���� �ʱ�ȭ
        //EventManager.Instance.encounterCount = 0;

        if (ScriptManager.Instance != null)
        {
            ScriptManager.Instance.tutorId = 0;
        }
        InfiniteMod = false; // InfiniteMod �ʱ�ȭ
        if (ScriptManager.Instance != null)
        {
            ScriptManager.Instance.endingChogihwa(); // ���� ���൵ �ʱ�ȭ
        }

        // ClickerManager �ʱ�ȭ
        ClickerManager.Instance.PerSecond = 60;
        ClickerManager.Instance.totalEarnings = 0;
        ClickerManager.Instance.incomePerSecond = 1000;
        ClickerManager.Instance.incomePerClick = 10;
        ClickerManager.Instance.ResetClickerItems(); // �ʱ� ������ ������ ����

        //���� ����
        for(int i = 0; i < 9; i++)
        {
            TemuBadgeBool[i] = true;
            TemuBadges[i].SetActive(true);
        }
        TemuBadgeNum = 10;
        TemuBadgeText.text = TemuBadgeNum.ToString(); // ���� ���� ������Ʈ
        YoutubeBadgeSetting();
        InstaBadgeSetting();
        BankBadgeSetting();

        SaveData(); // �ʱ� ������ ����
    }

    // ���� ���� �޼���
    public void QuitGame()
    {
        SaveData(); // ������ ���� �� ���� ����

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� ���� ����
#else
        Application.Quit(); // ����� ���� ����
#endif
    }

    // �������� �����ϴ� �޼���
    public void SetMoney(long amount)
    {
        if (money != amount)
        {
            long diff = amount - money;
            money = amount;

            // �������� ������ ��� �̺�Ʈ ȣ��
            if (diff > 0)
            {
                MoneyIncreased?.Invoke(diff); // ������ ���� �̺�Ʈ ȣ��
            }
            // �������� ������ ��� �̺�Ʈ ȣ��
            else if (diff < 0)
            {
                MoneyDecreased?.Invoke(Math.Abs(diff)); // ������ ���� �̺�Ʈ ȣ��
            }
        }
    }

    // ��Ʃ�� ���� ���� �޼���
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

    //�ν�Ÿ ���� ���� �޼���
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

    //ī�� ���� ���� �޼���
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

    // ���� �������� ��ȯ�ϴ� �޼���
    public long GetMoney()
    {
        return money;
    }

    // Ư�� �ε����� ��ǰ�� ��ȯ�ϴ� �޼���
    public Product GetProduct(int index)
    {
        if (index >= 0 && index < products.Count)
        {
            return products[index];
        }
        return new Product(); // ��ȿ���� ���� �ε����� ��� �� ��ǰ ��ȯ
    }

    // ��ǰ ������ �����ϴ� �޼���
    public void SetProductPrice(int index, long price)
    {
        if (index >= 0 && index < products.Count)
        {
            Product product = products[index];
            product.Price = price;
            products[index] = product;
        }
    }

    // ��ǰ ID�� �����ϴ� �޼���
    public void SetProductID(int index, string id)
    {
        if (index >= 0 && index < products.Count)
        {
            Product product = products[index];
            product.ID = id;
            products[index] = product;
        }
    }

    // ��ǰ �ν�Ÿ �Խñ� ������ �����ϴ� �޼���
    public void SetProductPost(int index, string post)
    {
        if (index >= 0 && index < products.Count)
        {
            Product product = products[index];
            product.Instacontent = post;
            products[index] = product;
        }
    }

    // ������ �������ϴ� �޼���
    public string FormatPrice(long price)
    {
        if (price == 0)
        {
            return "0��";
        }

        if (price < 10000)
        {
            return price + "��";
        }

        string[] units = { "", "��", "��", "��", "��" };
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

        return result.Trim() + "��";
    }
}
