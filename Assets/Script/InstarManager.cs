using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static instaDmMassage;

public class InstarManager : MonoBehaviour
{
    public static InstarManager Instance; // 싱글톤 인스턴스

    public int DmEA = 0;
    public int DmCount = 0;

    public GameObject Timeline; // 게임 오브젝트를 지정하는 변수
    public GameObject InstaPost; // Inspector에서 prefab을 지정할 수 있는 변수
    public GameObject dmListScroll; // Inspector에서 지정할 수 있는 게임오브젝트 변수
    public GameObject DMListPrefab; // Inspector에서 지정할 수 있는 프리팹 변수
    public GameObject dmScroll; // Inspector에서 지정할 수 있는 게임오브젝트 변수
    public GameObject DMPrefab; // Inspector에서 지정할 수 있는 프리팹 변수

    public GameObject targetInactiveObject; // Inspector에서 지정할 Inactive될 오브젝트

    // Inspector에서 관리할 수 있는 게임오브젝트 리스트
    public List<GameObject> gameObjectsToInActive;
    public List<GameObject> gameObjectsToActive;

    // 다른 스크립트에서도 접근할 수 있는 변수
    public string whatIsMassageID;

    private List<GameObject> posts = new List<GameObject>(); // 생성된 prefab을 관리하는 리스트
    private List<GameObject> dmPrefabs = new List<GameObject>(); // 생성된 DMPrefab을 관리하는 리스트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // targetInactiveObject가 비활성화되었는지 확인
        if (targetInactiveObject != null && !targetInactiveObject.activeInHierarchy)
        {
            // 생성된 모든 DMPrefab을 제거
            foreach (GameObject dmPrefab in dmPrefabs)
            {
                Destroy(dmPrefab);
            }
            dmPrefabs.Clear();
        }
    }

    // 버튼에서 호출할 수 있는 매개변수가 없는 메서드
    public void CreatePost()
    {
        // prefab을 생성하여 Timeline의 하위로 설정
        GameObject newPost = Instantiate(InstaPost, Timeline.transform);

        // prefab InstaPost 내부에 있는 컴포넌트를 가져옴
        Transform contentTransform = newPost.transform.Find("게시글 내용");
        Transform imageTransform = newPost.transform.Find("이미지"); // 이미지 Transform 추가

        // TextMeshProUGUI 컴포넌트를 가져옴
        TextMeshProUGUI textComponent = contentTransform.GetComponent<TextMeshProUGUI>();

        // 생성된 prefab을 리스트에 추가
        posts.Add(newPost);

        // 생성된 prefab의 갯수가 10개가 넘어가면 가장 이전에 생성된 prefab을 제거
        if (posts.Count > 10)
        {
            Destroy(posts[0]);
            posts.RemoveAt(0);
        }

        List<Product> products = GameManager.Instance.GetProducts();
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].ID == ShopManager.Instance.curruntItemID)
            {
                // 해당 ID를 가진 Product의 Instacontent와 InstaImage를 설정
                Product targetProduct = products[i];
                textComponent.text = targetProduct.Instacontent;

                if (imageTransform != null)
                {
                    Image imageComponent = imageTransform.GetComponent<Image>();
                    if (imageComponent != null)
                    {
                        imageComponent.sprite = targetProduct.InstaImage;
                    }
                }
                break;
            }
        }

        // CreatePost 마지막에 dmGet 메서드 실행
        dmGet();
    }

    // dmGet 메서드 생성
    public void dmGet()
    {
        // ShopManager 싱글톤 인스턴스에서 현재 아이템 ID를 가져옴
        string curruntItemID = ShopManager.Instance.curruntItemID;

        // instaDmMassage 싱글톤 인스턴스의 DM 리스트에서 현재 아이템 ID와 일치하는 DMItem들을 필터링하여 리스트로 만듦
        List<instaDmMassage.DMItem> dmItems = instaDmMassage.instance.DM
            .Where(dm => dm.ItemID == curruntItemID)
            .ToList();

        for (int i = 0; i < DmEA; i++)
        {
            // DMListPrefab 인스턴스 생성
            GameObject newDMList = Instantiate(DMListPrefab, dmListScroll.transform);

            // 랜덤한 DMItem 선택
            instaDmMassage.DMItem randomDMItem = dmItems[Random.Range(0, dmItems.Count)];

            // DMListPrefab 내부의 TextMeshPro 업데이트
            TextMeshProUGUI dmListText = newDMList.GetComponentInChildren<TextMeshProUGUI>();
            if (dmListText != null)
            {
                //8자까지만 출력
                string truncatedMessage = randomDMItem.Massage.Length > 8 ? randomDMItem.Massage.Substring(0, 8) : randomDMItem.Massage;
                dmListText.text = truncatedMessage + "...";

                // DMListPrefab 버튼에 클릭 이벤트 추가
                Button dmListButton = newDMList.GetComponentInChildren<Button>();
                if (dmListButton != null)
                {
                    dmListButton.onClick.AddListener(() => OnDMListButtonClick(randomDMItem.Massage));
                }
            }

            DmCount++;
        }

        GameManager.Instance.InstaBadgeSetting();
    }

    // TextMeshPro의 내용을 기반으로 DMItem을 찾는 메서드
    private instaDmMassage.DMItem FindDMItemByMessage(string message)
    {
        return instaDmMassage.instance.DM.FirstOrDefault(dm => dm.Massage == message);
    }

    // DMListPrefab의 버튼 클릭 시 호출될 메서드
    private void OnDMListButtonClick(string message)
    {
        // DMPrefab 인스턴스 생성
        GameObject newDM = Instantiate(DMPrefab, dmScroll.transform);

        // 생성된 DMPrefab을 리스트에 추가
        dmPrefabs.Add(newDM);

        // DMPrefab 내부의 TextMeshPro 업데이트
        TextMeshProUGUI dmText = newDM.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText != null)
        {
            // 클릭한 DMListPrefab의 텍스트 내용으로 업데이트
            dmText.text = message;

            // whatIsMassageID 업데이트
            instaDmMassage.DMItem targetDMItem = FindDMItemByMessage(message);
            if (targetDMItem != null)
            {
                whatIsMassageID = targetDMItem.MassageID;
            }

            // 레이아웃 강제 재구성
            StartCoroutine(ForceRebuildLayout(newDM));
        }

        UpdateWhatIsMassageID(message);

        Debug.Log("현재 메세지 ID: " + whatIsMassageID);
        DmCount--;
    }

    public void ActiveAndInActive()
    {
        // 리스트에 있는 모든 게임오브젝트를 비활성화
        foreach (GameObject obj in gameObjectsToInActive)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // 리스트에 있는 모든 게임오브젝트를 활성화
        foreach (GameObject obj in gameObjectsToActive)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    // whatIsMassageID를 업데이트하는 메서드
    public void UpdateWhatIsMassageID(string clickedMessage)
    {
        instaDmMassage.DMItem targetDMItem = FindDMItemByMessage(clickedMessage);
        if (targetDMItem != null)
        {
            whatIsMassageID = targetDMItem.MassageID;
        }
    }

    // 프리팹의 설정을 변경하는 메서드
    private void UpdatePrefabSettings(GameObject prefab, bool isGoodOrBadReply)
    {
        HorizontalLayoutGroup layoutGroup = prefab.GetComponent<HorizontalLayoutGroup>();
        Transform profileTransform = prefab.transform.Find("프사");
        TextMeshProUGUI textMeshPro = prefab.GetComponentInChildren<TextMeshProUGUI>();
        Image balloonImage = prefab.transform.Find("말풍선").GetComponent<Image>();

        if (layoutGroup != null)
        {
            layoutGroup.childAlignment = isGoodOrBadReply ? TextAnchor.LowerRight : TextAnchor.LowerLeft;
            layoutGroup.reverseArrangement = isGoodOrBadReply;
        }

        if (profileTransform != null)
        {
            profileTransform.gameObject.SetActive(!isGoodOrBadReply);
        }

        if (textMeshPro != null)
        {
            textMeshPro.color = isGoodOrBadReply ? new Color32(255, 255, 255, 255) : new Color32(62, 58, 57, 255);
        }

        if (balloonImage != null)
        {
            balloonImage.color = isGoodOrBadReply ? new Color32(43, 174, 102, 255) : new Color32(255, 255, 255, 255);
        }
    }

    public void GoodReply()
    {
        // DMPrefab 인스턴스 생성
        GameObject newDM = Instantiate(DMPrefab, dmScroll.transform);

        // 생성된 DMPrefab을 리스트에 추가
        dmPrefabs.Add(newDM);

        // DMPrefab 내부의 TextMeshPro 업데이트
        TextMeshProUGUI dmText = newDM.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText != null)
        {
            // 현재 whatIsMassageID를 기반으로 DMItem을 찾고 commentGood로 업데이트
            instaDmMassage.DMItem targetDMItem = instaDmMassage.instance.DM.FirstOrDefault(dm => dm.MassageID == whatIsMassageID);
            if (targetDMItem != null)
            {
                dmText.text = targetDMItem.CommentGood.Substring(1); // 첫 글자 제외한 나머지 문자열

                long playerMoney = GameManager.Instance.GetMoney();
                long price = GetProductPriceByMassageID(targetDMItem.MassageID);
                long priceDouble = (long)(price * 2); // 가격의 1.5배 계산
                long pricehalf = (long)(price * 0.5); // 가격의 1.5배 계산
                Debug.Log($"Price: {price}, 1.5x Price: {priceDouble}"); // 디버깅 메시지 추가
                string formattedSum = GameManager.Instance.FormatPrice(priceDouble);
                string formattedSum_2 = GameManager.Instance.FormatPrice(pricehalf);

                // 추가된 부분: commentGood의 첫 글자가 'ㅇ'인지 확인하고 정답 계산
                if (!string.IsNullOrEmpty(targetDMItem.CommentGood) && targetDMItem.CommentGood[0] == 'ㅇ')
                {
                    GameManager.Instance.SetMoney(playerMoney + priceDouble);
                    BankManager.Instance.CreateBankPrefab($"[판매성공] {formattedSum} 입금", 2);
                }

                // 추가된 부분: commentGood의 첫 글자가 'ㄴ'인지 확인하고 정답 계산
                if (!string.IsNullOrEmpty(targetDMItem.CommentGood) && targetDMItem.CommentGood[0] == 'ㄴ')
                {
                    countManager.Instance.Odap++;
                    GameManager.Instance.SetMoney(playerMoney - pricehalf);
                    BankManager.Instance.CreateBankPrefab($"[판매실패] 재고 처리 비용 - {formattedSum_2} 출금", 1);
                }

                // 프리팹 설정 업데이트
                UpdatePrefabSettings(newDM, true);
            }

            // 레이아웃 강제 재구성
            StartCoroutine(ForceRebuildLayout(newDM));
        }

        // DMPrefab 인스턴스 생성
        GameObject newDM_r = Instantiate(DMPrefab, dmScroll.transform);

        // 생성된 DMPrefab을 리스트에 추가
        dmPrefabs.Add(newDM_r);

        // DMPrefab 내부의 TextMeshPro 업데이트
        TextMeshProUGUI dmText_r = newDM_r.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText_r != null)
        {
            // 현재 whatIsMassageID를 기반으로 DMItem을 찾고 commentGood_r로 업데이트
            instaDmMassage.DMItem targetDMItem_r = instaDmMassage.instance.DM.FirstOrDefault(dm => dm.MassageID == whatIsMassageID);
            if (targetDMItem_r != null)
            {
                dmText_r.text = targetDMItem_r.CommentGood_r.Substring(1); // 첫 글자 제외한 나머지 문자열

                // 프리팹 설정 업데이트
                UpdatePrefabSettings(newDM_r, false);
            }

            // 레이아웃 강제 재구성
            StartCoroutine(ForceRebuildLayout(newDM_r));
        }
    }


    // BadReply 메서드
    public void BadReply()
    {
        // DMPrefab 인스턴스 생성
        GameObject newDM = Instantiate(DMPrefab, dmScroll.transform);

        // 생성된 DMPrefab을 리스트에 추가
        dmPrefabs.Add(newDM);

        // DMPrefab 내부의 TextMeshPro 업데이트
        TextMeshProUGUI dmText = newDM.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText != null)
        {
            // 현재 whatIsMassageID를 기반으로 DMItem을 찾고 commentBad로 업데이트
            instaDmMassage.DMItem targetDMItem = instaDmMassage.instance.DM.FirstOrDefault(dm => dm.MassageID == whatIsMassageID);
            if (targetDMItem != null)
            {
                dmText.text = targetDMItem.CommentBad.Substring(1); // 첫 글자 제외한 나머지 문자열
                long playerMoney = GameManager.Instance.GetMoney();
                long price = GetProductPriceByMassageID(targetDMItem.MassageID);
                long priceDouble = (long)(price * 2); // 가격의 1.5배 계산
                long pricehalf = (long)(price * 0.5); // 가격의 1.5배 계산
                string formattedSum = GameManager.Instance.FormatPrice(priceDouble);
                string formattedSum_2 = GameManager.Instance.FormatPrice(pricehalf);


                // 추가된 부분: commentBad의 첫 글자가 'ㅇ'인지 확인하고 정답 계산
                if (!string.IsNullOrEmpty(targetDMItem.CommentBad) && targetDMItem.CommentBad[0] == 'ㅇ')
                {
                    GameManager.Instance.SetMoney(playerMoney + priceDouble);
                    BankManager.Instance.CreateBankPrefab($"[판매성공] {formattedSum} 입금", 2);
                }

                // 추가된 부분: commentBad의 첫 글자가 'ㄴ'인지 확인하고 정답 계산
                if (!string.IsNullOrEmpty(targetDMItem.CommentBad) && targetDMItem.CommentBad[0] == 'ㄴ')
                {
                    countManager.Instance.Odap++;
                    GameManager.Instance.SetMoney(playerMoney - pricehalf);
                    BankManager.Instance.CreateBankPrefab($"[판매실패] 재고 처리 비용 - {formattedSum_2} 출금", 1);
                }

                // 프리팹 설정 업데이트
                UpdatePrefabSettings(newDM, true);
            }

            // 레이아웃 강제 재구성
            StartCoroutine(ForceRebuildLayout(newDM));
        }

        // DMPrefab 인스턴스 생성
        GameObject newDM_r = Instantiate(DMPrefab, dmScroll.transform);

        // 생성된 DMPrefab을 리스트에 추가
        dmPrefabs.Add(newDM_r);

        // DMPrefab 내부의 TextMeshPro 업데이트
        TextMeshProUGUI dmText_r = newDM_r.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText_r != null)
        {
            // 현재 whatIsMassageID를 기반으로 DMItem을 찾고 CommentBad_r로 업데이트
            instaDmMassage.DMItem targetDMItem_r = instaDmMassage.instance.DM.FirstOrDefault(dm => dm.MassageID == whatIsMassageID);
            if (targetDMItem_r != null)
            {
                dmText_r.text = targetDMItem_r.CommentBad_r.Substring(1); // 첫 글자 제외한 나머지 문자열

                // 프리팹 설정 업데이트
                UpdatePrefabSettings(newDM_r, false);
            }

            // 레이아웃 강제 재구성
            StartCoroutine(ForceRebuildLayout(newDM_r));
        }
    }

    // 레이아웃을 강제로 재구성하는 코루틴
    private IEnumerator ForceRebuildLayout(GameObject target)
    {
        // 한 프레임 대기
        yield return new WaitForEndOfFrame();

        // 레이아웃 강제 재구성
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }


    // MassageID를 기반으로 Product의 Price를 찾는 메서드
    private long GetProductPriceByMassageID(string massageID)
    {
        List<Product> products = GameManager.Instance.GetProducts();
        string ItemID = instaDmMassage.instance.GetItemIDByMassageID(massageID);
        Product targetProduct = products.FirstOrDefault(product => product.ID == ItemID);
        return targetProduct.Price;
    }
}
