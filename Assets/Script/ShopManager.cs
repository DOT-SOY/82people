using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static ShopManager Instance;

    // ���� �����͸� �����ϴ� ������
    public int productEA = 1; // ��ǰ�� ����
    private long productSum = 0; // ��ǰ�� �ݾ� �� ��
    public long currentPrice = 0; // ���� ��ǰ�� ����
    public string curruntItemID = "";

    //���� ������ ����
    public int BadgeInt;

    // TextMeshPro�� ����Ͽ� ��ǰ �ݾ��� ǥ���ϱ� ���� ����
    [SerializeField] private TextMeshProUGUI productSumText;
    [SerializeField] private TextMeshProUGUI productSumEAText;

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
    }

    // ��ǰ�� ������ �����մϴ�.
    public void SetProductPrice(string id)
    {
        // GameManager�� products �迭�� ��ȸ�Ͽ� �Է��� ID�� ��ġ�ϴ� ��ǰ�� ã���ϴ�.
        List<Product> products = GameManager.Instance.GetProducts();
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].ID == id)
            {
                // ��ġ�ϴ� ��ǰ�� ������ �����մϴ�.
                currentPrice = products[i].Price;
                curruntItemID = products[i].ID;
                BadgeInt = i;
                break;
            }
        }
        CalculateProductSum();
    }

    // ��ǰ ������ 1 ������Ű�� �޼���
    public void IncreaseProductEA()
    {
        if (productEA >= 20)
        {
            productEA = 20;
        }
        else
        {
            productEA++;
        }
        CalculateProductSum(); // �� �κп��� ���ڰ� ������ ����Ǿ����ϴ�.
    }

    // ��ǰ ������ 1 ���ҽ�Ű�� �޼���
    public void DecreaseProductEA()
    {
        if (productEA > 1)
        {
            productEA--;
            CalculateProductSum(); // �� �κп��� ���ڰ� ������ ����Ǿ����ϴ�.
        }
    }

    // ��ǰ �ݾ� �� ���� ����ϰ� ǥ�� �ؽ�Ʈ�� ������Ʈ�ϴ� �޼���
    private void CalculateProductSum()
    {
        productSum = currentPrice * productEA;
        productSumText.text = GameManager.Instance.FormatPrice(productSum);
        productSumEAText.text = productEA.ToString();
    }

    // ��ǰ�� �����ϰ� �÷��̾��� �������� ������Ʈ�ϴ� �޼���
    public void PurchaseProduct()
    {
        long playerMoney = GameManager.Instance.GetMoney();
        string formattedSum = GameManager.Instance.FormatPrice(productSum);
        if (productSum > playerMoney)
        {
            PopupManager.Instance.ShowPopup($"[���ھƹ�ũ] ī��������� <br> {formattedSum} - �ܾ׺���");
            BankManager.Instance.CreateBankPrefab($"[���ھƹ�ũ] ī��������� {formattedSum} - �ܾ׺���", 1);
            ResetProductEA();
        }
        else
        {
            GameManager.Instance.SetMoney(playerMoney - productSum);
            InstarManager.Instance.DmEA = productEA;
            BankManager.Instance.CreateBankPrefab($"[�蹫] {formattedSum} �����Ϸ�", 0);
            InstarManager.Instance.CreatePost();
            ClickerManager.Instance.SetYoutubeJemok(curruntItemID);

            //���� ����
            if (GameManager.Instance.TemuBadgeBool[BadgeInt] == true)
            {
                GameManager.Instance.TemuBadgeNum--;
            }
            GameManager.Instance.TemuBadgeText.text = GameManager.Instance.TemuBadgeNum.ToString();
            GameManager.Instance.TemuBadgeBool[BadgeInt] = false;
            GameManager.Instance.TemuBadges[BadgeInt].SetActive(false);
            //���� ���� ��

            ResetProductEA();
        }
    }

    /*
    //��Ʃ�� ����ϰ� ������ ������Ʈ�ϴ� �޼���
    public void setYoutube(string ID){
        ClickerManager.Instance.SetYoutubeThumbnail(ID);
        switch (ID)
        {
            case ("������"):
                ClickerManager.Instance.SetYoutubeJemok("�ΰ��� ���������ִ� ��谡 �ִ�?! ���� �ı� Vlog..<sprite index=36>");
                break;

            case ("��"):
                ClickerManager.Instance.SetYoutubeJemok("����� �����ø� ���� �ı� Vlog..<sprite index=36>");
                break;

            case ("�ѳ�����"):
                ClickerManager.Instance.SetYoutubeJemok("���Ѵ��� ���� �ı� Vlog..<sprite index=36>");
                break;

            default:
                ClickerManager.Instance.SetYoutubeJemok(ID + " ���� �ı� Vlog..<sprite index=36>");
                break;
        }

    }*/


    // ��ǰ ������ �ʱ�ȭ�ϴ� �޼���
    private void ResetProductEA()
    {
        productEA = 1;
        productSum = 0;
    }

}
