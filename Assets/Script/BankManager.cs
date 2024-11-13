using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BankManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static BankManager Instance;

    // �̺�Ʈ�Ŵ��� ����
    public EventManager eventManager; // Inspector���� �Ҵ�

    // Inspector���� ������ �� �ִ� TextMeshPro ����
    public TextMeshProUGUI curruntMoney;

    // Inspector���� ������ �� �ִ� Image ����Ʈ ����
    public List<Sprite> bankImage;

    // Inspector���� ������ �� �ִ� ������ ����
    public GameObject bankPrefab;

    // Inspector���� ������ �� �ִ� ���� ������Ʈ ����
    public GameObject bankTarget;

    // ������ �������� �����ϴ� ����Ʈ
    private List<GameObject> createdPrefabs = new List<GameObject>();

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // curruntMoney�� ������ ������Ʈ�ϴ� �޼���
    public void UpdateCurruntMoney()
    {
        // GameManager.Instance.GetMoney()�� ȣ���Ͽ� ��ȯ�� ���� GameManager.Instance.FormatPrice()�� ����
        long money = GameManager.Instance.GetMoney();
        string formattedMoney = GameManager.Instance.FormatPrice(money);

        // curruntMoney�� �ؽ�Ʈ�� ���˵� ������ ����
        if (curruntMoney != null)
        {
            curruntMoney.text = formattedMoney;
        }
    }

    // ���� -> ���� �޼���
    public void temuToBank(string message)
    {
        CreateBankPrefab(message, 0);
    }

    // īī�� -> ���� �޼���
    public void cacaoToBank(string message)
    {
        CreateBankPrefab(message, 1);
    }

    // ��� -> ���� �޼���
    public void humanToBank(string message)
    {
        CreateBankPrefab(message, 2);
    }

    // ��Ʃ�� -> ���� �޼���
    public void youtubeToBank(string message)
    {
        CreateBankPrefab(message, 3);
    }

    // �̺�Ʈ -> ���� �޼���
    public void EventToBank(string message)
    {
        CreateBankPrefab(message, 4);
    }

    // ����� ������ ���� �޼���
    public void CreateBankPrefab(string message, int imageIndex)
    {
        if (bankPrefab != null && bankTarget != null && bankImage != null && bankImage.Count > imageIndex)
        {
            // bankTarget ������ prefab ����
            GameObject newBankPrefab = Instantiate(bankPrefab, bankTarget.transform);

            // ������ prefab ���� �̹��� ������Ʈ ����
            Image imageComponent = newBankPrefab.GetComponentInChildren<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = bankImage[imageIndex];
            }

            // ������ prefab ���� TextMeshPro ������Ʈ ����
            TextMeshProUGUI textComponent = newBankPrefab.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = message;
            }

            // ������ �������� ����Ʈ�� �߰�
            createdPrefabs.Add(newBankPrefab);

            // �������� ������ 30���� ������ ���� ������ ������ ����
            if (createdPrefabs.Count > 30)
            {
                Destroy(createdPrefabs[0]);
                createdPrefabs.RemoveAt(0);
            }
        }

        GameManager.Instance.BankBadgeInt++;

        
        // �Ʒ��� ���� �̺�Ʈ ��ī���� �Լ� ȣ��
        eventManager.RandomEncounter();
        
    }
}
