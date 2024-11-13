using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ShopManager Instance;

    // 상점 데이터를 저장하는 변수들
    public int productEA = 1; // 상품의 갯수
    private long productSum = 0; // 상품의 금액 총 합
    public long currentPrice = 0; // 현재 상품의 가격
    public string curruntItemID = "";

    //뱃지 관리용 변수
    public int BadgeInt;

    // TextMeshPro를 사용하여 상품 금액을 표시하기 위한 참조
    [SerializeField] private TextMeshProUGUI productSumText;
    [SerializeField] private TextMeshProUGUI productSumEAText;

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
    }

    // 상품의 가격을 설정합니다.
    public void SetProductPrice(string id)
    {
        // GameManager의 products 배열을 순회하여 입력한 ID와 일치하는 상품을 찾습니다.
        List<Product> products = GameManager.Instance.GetProducts();
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].ID == id)
            {
                // 일치하는 상품의 가격을 설정합니다.
                currentPrice = products[i].Price;
                curruntItemID = products[i].ID;
                BadgeInt = i;
                break;
            }
        }
        CalculateProductSum();
    }

    // 상품 갯수를 1 증가시키는 메서드
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
        CalculateProductSum(); // 이 부분에서 인자가 없도록 변경되었습니다.
    }

    // 상품 갯수를 1 감소시키는 메서드
    public void DecreaseProductEA()
    {
        if (productEA > 1)
        {
            productEA--;
            CalculateProductSum(); // 이 부분에서 인자가 없도록 변경되었습니다.
        }
    }

    // 상품 금액 총 합을 계산하고 표시 텍스트를 업데이트하는 메서드
    private void CalculateProductSum()
    {
        productSum = currentPrice * productEA;
        productSumText.text = GameManager.Instance.FormatPrice(productSum);
        productSumEAText.text = productEA.ToString();
    }

    // 상품을 구매하고 플레이어의 소지금을 업데이트하는 메서드
    public void PurchaseProduct()
    {
        long playerMoney = GameManager.Instance.GetMoney();
        string formattedSum = GameManager.Instance.FormatPrice(productSum);
        if (productSum > playerMoney)
        {
            PopupManager.Instance.ShowPopup($"[코코아뱅크] 카드결제실패 <br> {formattedSum} - 잔액부족");
            BankManager.Instance.CreateBankPrefab($"[코코아뱅크] 카드결제실패 {formattedSum} - 잔액부족", 1);
            ResetProductEA();
        }
        else
        {
            GameManager.Instance.SetMoney(playerMoney - productSum);
            InstarManager.Instance.DmEA = productEA;
            BankManager.Instance.CreateBankPrefab($"[배무] {formattedSum} 결제완료", 0);
            InstarManager.Instance.CreatePost();
            ClickerManager.Instance.SetYoutubeJemok(curruntItemID);

            //뱃지 관리
            if (GameManager.Instance.TemuBadgeBool[BadgeInt] == true)
            {
                GameManager.Instance.TemuBadgeNum--;
            }
            GameManager.Instance.TemuBadgeText.text = GameManager.Instance.TemuBadgeNum.ToString();
            GameManager.Instance.TemuBadgeBool[BadgeInt] = false;
            GameManager.Instance.TemuBadges[BadgeInt].SetActive(false);
            //뱃지 관리 끝

            ResetProductEA();
        }
    }

    /*
    //유튜브 썸네일과 제목을 업데이트하는 메서드
    public void setYoutube(string ID){
        ClickerManager.Instance.SetYoutubeThumbnail(ID);
        switch (ID)
        {
            case ("세차기"):
                ClickerManager.Instance.SetYoutubeJemok("인간을 세차시켜주는 기계가 있다?! 공구 후기 Vlog..<sprite index=36>");
                break;

            case ("폰"):
                ClickerManager.Instance.SetYoutubeJemok("대망의 아이플립 공구 후기 Vlog..<sprite index=36>");
                break;

            case ("한남더힐"):
                ClickerManager.Instance.SetYoutubeJemok("남한더힐 공구 후기 Vlog..<sprite index=36>");
                break;

            default:
                ClickerManager.Instance.SetYoutubeJemok(ID + " 공구 후기 Vlog..<sprite index=36>");
                break;
        }

    }*/


    // 상품 갯수를 초기화하는 메서드
    private void ResetProductEA()
    {
        productEA = 1;
        productSum = 0;
    }

}
