using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static instaDmMassage;

public class InstarManager : MonoBehaviour
{
    public static InstarManager Instance; // �̱��� �ν��Ͻ�

    public int DmEA = 0;
    public int DmCount = 0;

    public GameObject Timeline; // ���� ������Ʈ�� �����ϴ� ����
    public GameObject InstaPost; // Inspector���� prefab�� ������ �� �ִ� ����
    public GameObject dmListScroll; // Inspector���� ������ �� �ִ� ���ӿ�����Ʈ ����
    public GameObject DMListPrefab; // Inspector���� ������ �� �ִ� ������ ����
    public GameObject dmScroll; // Inspector���� ������ �� �ִ� ���ӿ�����Ʈ ����
    public GameObject DMPrefab; // Inspector���� ������ �� �ִ� ������ ����

    public GameObject targetInactiveObject; // Inspector���� ������ Inactive�� ������Ʈ

    // Inspector���� ������ �� �ִ� ���ӿ�����Ʈ ����Ʈ
    public List<GameObject> gameObjectsToInActive;
    public List<GameObject> gameObjectsToActive;

    // �ٸ� ��ũ��Ʈ������ ������ �� �ִ� ����
    public string whatIsMassageID;

    private List<GameObject> posts = new List<GameObject>(); // ������ prefab�� �����ϴ� ����Ʈ
    private List<GameObject> dmPrefabs = new List<GameObject>(); // ������ DMPrefab�� �����ϴ� ����Ʈ

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
        // targetInactiveObject�� ��Ȱ��ȭ�Ǿ����� Ȯ��
        if (targetInactiveObject != null && !targetInactiveObject.activeInHierarchy)
        {
            // ������ ��� DMPrefab�� ����
            foreach (GameObject dmPrefab in dmPrefabs)
            {
                Destroy(dmPrefab);
            }
            dmPrefabs.Clear();
        }
    }

    // ��ư���� ȣ���� �� �ִ� �Ű������� ���� �޼���
    public void CreatePost()
    {
        // prefab�� �����Ͽ� Timeline�� ������ ����
        GameObject newPost = Instantiate(InstaPost, Timeline.transform);

        // prefab InstaPost ���ο� �ִ� ������Ʈ�� ������
        Transform contentTransform = newPost.transform.Find("�Խñ� ����");
        Transform imageTransform = newPost.transform.Find("�̹���"); // �̹��� Transform �߰�

        // TextMeshProUGUI ������Ʈ�� ������
        TextMeshProUGUI textComponent = contentTransform.GetComponent<TextMeshProUGUI>();

        // ������ prefab�� ����Ʈ�� �߰�
        posts.Add(newPost);

        // ������ prefab�� ������ 10���� �Ѿ�� ���� ������ ������ prefab�� ����
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
                // �ش� ID�� ���� Product�� Instacontent�� InstaImage�� ����
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

        // CreatePost �������� dmGet �޼��� ����
        dmGet();
    }

    // dmGet �޼��� ����
    public void dmGet()
    {
        // ShopManager �̱��� �ν��Ͻ����� ���� ������ ID�� ������
        string curruntItemID = ShopManager.Instance.curruntItemID;

        // instaDmMassage �̱��� �ν��Ͻ��� DM ����Ʈ���� ���� ������ ID�� ��ġ�ϴ� DMItem���� ���͸��Ͽ� ����Ʈ�� ����
        List<instaDmMassage.DMItem> dmItems = instaDmMassage.instance.DM
            .Where(dm => dm.ItemID == curruntItemID)
            .ToList();

        for (int i = 0; i < DmEA; i++)
        {
            // DMListPrefab �ν��Ͻ� ����
            GameObject newDMList = Instantiate(DMListPrefab, dmListScroll.transform);

            // ������ DMItem ����
            instaDmMassage.DMItem randomDMItem = dmItems[Random.Range(0, dmItems.Count)];

            // DMListPrefab ������ TextMeshPro ������Ʈ
            TextMeshProUGUI dmListText = newDMList.GetComponentInChildren<TextMeshProUGUI>();
            if (dmListText != null)
            {
                //8�ڱ����� ���
                string truncatedMessage = randomDMItem.Massage.Length > 8 ? randomDMItem.Massage.Substring(0, 8) : randomDMItem.Massage;
                dmListText.text = truncatedMessage + "...";

                // DMListPrefab ��ư�� Ŭ�� �̺�Ʈ �߰�
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

    // TextMeshPro�� ������ ������� DMItem�� ã�� �޼���
    private instaDmMassage.DMItem FindDMItemByMessage(string message)
    {
        return instaDmMassage.instance.DM.FirstOrDefault(dm => dm.Massage == message);
    }

    // DMListPrefab�� ��ư Ŭ�� �� ȣ��� �޼���
    private void OnDMListButtonClick(string message)
    {
        // DMPrefab �ν��Ͻ� ����
        GameObject newDM = Instantiate(DMPrefab, dmScroll.transform);

        // ������ DMPrefab�� ����Ʈ�� �߰�
        dmPrefabs.Add(newDM);

        // DMPrefab ������ TextMeshPro ������Ʈ
        TextMeshProUGUI dmText = newDM.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText != null)
        {
            // Ŭ���� DMListPrefab�� �ؽ�Ʈ �������� ������Ʈ
            dmText.text = message;

            // whatIsMassageID ������Ʈ
            instaDmMassage.DMItem targetDMItem = FindDMItemByMessage(message);
            if (targetDMItem != null)
            {
                whatIsMassageID = targetDMItem.MassageID;
            }

            // ���̾ƿ� ���� �籸��
            StartCoroutine(ForceRebuildLayout(newDM));
        }

        UpdateWhatIsMassageID(message);

        Debug.Log("���� �޼��� ID: " + whatIsMassageID);
        DmCount--;
    }

    public void ActiveAndInActive()
    {
        // ����Ʈ�� �ִ� ��� ���ӿ�����Ʈ�� ��Ȱ��ȭ
        foreach (GameObject obj in gameObjectsToInActive)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // ����Ʈ�� �ִ� ��� ���ӿ�����Ʈ�� Ȱ��ȭ
        foreach (GameObject obj in gameObjectsToActive)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    // whatIsMassageID�� ������Ʈ�ϴ� �޼���
    public void UpdateWhatIsMassageID(string clickedMessage)
    {
        instaDmMassage.DMItem targetDMItem = FindDMItemByMessage(clickedMessage);
        if (targetDMItem != null)
        {
            whatIsMassageID = targetDMItem.MassageID;
        }
    }

    // �������� ������ �����ϴ� �޼���
    private void UpdatePrefabSettings(GameObject prefab, bool isGoodOrBadReply)
    {
        HorizontalLayoutGroup layoutGroup = prefab.GetComponent<HorizontalLayoutGroup>();
        Transform profileTransform = prefab.transform.Find("����");
        TextMeshProUGUI textMeshPro = prefab.GetComponentInChildren<TextMeshProUGUI>();
        Image balloonImage = prefab.transform.Find("��ǳ��").GetComponent<Image>();

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
        // DMPrefab �ν��Ͻ� ����
        GameObject newDM = Instantiate(DMPrefab, dmScroll.transform);

        // ������ DMPrefab�� ����Ʈ�� �߰�
        dmPrefabs.Add(newDM);

        // DMPrefab ������ TextMeshPro ������Ʈ
        TextMeshProUGUI dmText = newDM.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText != null)
        {
            // ���� whatIsMassageID�� ������� DMItem�� ã�� commentGood�� ������Ʈ
            instaDmMassage.DMItem targetDMItem = instaDmMassage.instance.DM.FirstOrDefault(dm => dm.MassageID == whatIsMassageID);
            if (targetDMItem != null)
            {
                dmText.text = targetDMItem.CommentGood.Substring(1); // ù ���� ������ ������ ���ڿ�

                long playerMoney = GameManager.Instance.GetMoney();
                long price = GetProductPriceByMassageID(targetDMItem.MassageID);
                long priceDouble = (long)(price * 2); // ������ 1.5�� ���
                long pricehalf = (long)(price * 0.5); // ������ 1.5�� ���
                Debug.Log($"Price: {price}, 1.5x Price: {priceDouble}"); // ����� �޽��� �߰�
                string formattedSum = GameManager.Instance.FormatPrice(priceDouble);
                string formattedSum_2 = GameManager.Instance.FormatPrice(pricehalf);

                // �߰��� �κ�: commentGood�� ù ���ڰ� '��'���� Ȯ���ϰ� ���� ���
                if (!string.IsNullOrEmpty(targetDMItem.CommentGood) && targetDMItem.CommentGood[0] == '��')
                {
                    GameManager.Instance.SetMoney(playerMoney + priceDouble);
                    BankManager.Instance.CreateBankPrefab($"[�Ǹż���] {formattedSum} �Ա�", 2);
                }

                // �߰��� �κ�: commentGood�� ù ���ڰ� '��'���� Ȯ���ϰ� ���� ���
                if (!string.IsNullOrEmpty(targetDMItem.CommentGood) && targetDMItem.CommentGood[0] == '��')
                {
                    countManager.Instance.Odap++;
                    GameManager.Instance.SetMoney(playerMoney - pricehalf);
                    BankManager.Instance.CreateBankPrefab($"[�ǸŽ���] ��� ó�� ��� - {formattedSum_2} ���", 1);
                }

                // ������ ���� ������Ʈ
                UpdatePrefabSettings(newDM, true);
            }

            // ���̾ƿ� ���� �籸��
            StartCoroutine(ForceRebuildLayout(newDM));
        }

        // DMPrefab �ν��Ͻ� ����
        GameObject newDM_r = Instantiate(DMPrefab, dmScroll.transform);

        // ������ DMPrefab�� ����Ʈ�� �߰�
        dmPrefabs.Add(newDM_r);

        // DMPrefab ������ TextMeshPro ������Ʈ
        TextMeshProUGUI dmText_r = newDM_r.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText_r != null)
        {
            // ���� whatIsMassageID�� ������� DMItem�� ã�� commentGood_r�� ������Ʈ
            instaDmMassage.DMItem targetDMItem_r = instaDmMassage.instance.DM.FirstOrDefault(dm => dm.MassageID == whatIsMassageID);
            if (targetDMItem_r != null)
            {
                dmText_r.text = targetDMItem_r.CommentGood_r.Substring(1); // ù ���� ������ ������ ���ڿ�

                // ������ ���� ������Ʈ
                UpdatePrefabSettings(newDM_r, false);
            }

            // ���̾ƿ� ���� �籸��
            StartCoroutine(ForceRebuildLayout(newDM_r));
        }
    }


    // BadReply �޼���
    public void BadReply()
    {
        // DMPrefab �ν��Ͻ� ����
        GameObject newDM = Instantiate(DMPrefab, dmScroll.transform);

        // ������ DMPrefab�� ����Ʈ�� �߰�
        dmPrefabs.Add(newDM);

        // DMPrefab ������ TextMeshPro ������Ʈ
        TextMeshProUGUI dmText = newDM.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText != null)
        {
            // ���� whatIsMassageID�� ������� DMItem�� ã�� commentBad�� ������Ʈ
            instaDmMassage.DMItem targetDMItem = instaDmMassage.instance.DM.FirstOrDefault(dm => dm.MassageID == whatIsMassageID);
            if (targetDMItem != null)
            {
                dmText.text = targetDMItem.CommentBad.Substring(1); // ù ���� ������ ������ ���ڿ�
                long playerMoney = GameManager.Instance.GetMoney();
                long price = GetProductPriceByMassageID(targetDMItem.MassageID);
                long priceDouble = (long)(price * 2); // ������ 1.5�� ���
                long pricehalf = (long)(price * 0.5); // ������ 1.5�� ���
                string formattedSum = GameManager.Instance.FormatPrice(priceDouble);
                string formattedSum_2 = GameManager.Instance.FormatPrice(pricehalf);


                // �߰��� �κ�: commentBad�� ù ���ڰ� '��'���� Ȯ���ϰ� ���� ���
                if (!string.IsNullOrEmpty(targetDMItem.CommentBad) && targetDMItem.CommentBad[0] == '��')
                {
                    GameManager.Instance.SetMoney(playerMoney + priceDouble);
                    BankManager.Instance.CreateBankPrefab($"[�Ǹż���] {formattedSum} �Ա�", 2);
                }

                // �߰��� �κ�: commentBad�� ù ���ڰ� '��'���� Ȯ���ϰ� ���� ���
                if (!string.IsNullOrEmpty(targetDMItem.CommentBad) && targetDMItem.CommentBad[0] == '��')
                {
                    countManager.Instance.Odap++;
                    GameManager.Instance.SetMoney(playerMoney - pricehalf);
                    BankManager.Instance.CreateBankPrefab($"[�ǸŽ���] ��� ó�� ��� - {formattedSum_2} ���", 1);
                }

                // ������ ���� ������Ʈ
                UpdatePrefabSettings(newDM, true);
            }

            // ���̾ƿ� ���� �籸��
            StartCoroutine(ForceRebuildLayout(newDM));
        }

        // DMPrefab �ν��Ͻ� ����
        GameObject newDM_r = Instantiate(DMPrefab, dmScroll.transform);

        // ������ DMPrefab�� ����Ʈ�� �߰�
        dmPrefabs.Add(newDM_r);

        // DMPrefab ������ TextMeshPro ������Ʈ
        TextMeshProUGUI dmText_r = newDM_r.GetComponentInChildren<TextMeshProUGUI>();
        if (dmText_r != null)
        {
            // ���� whatIsMassageID�� ������� DMItem�� ã�� CommentBad_r�� ������Ʈ
            instaDmMassage.DMItem targetDMItem_r = instaDmMassage.instance.DM.FirstOrDefault(dm => dm.MassageID == whatIsMassageID);
            if (targetDMItem_r != null)
            {
                dmText_r.text = targetDMItem_r.CommentBad_r.Substring(1); // ù ���� ������ ������ ���ڿ�

                // ������ ���� ������Ʈ
                UpdatePrefabSettings(newDM_r, false);
            }

            // ���̾ƿ� ���� �籸��
            StartCoroutine(ForceRebuildLayout(newDM_r));
        }
    }

    // ���̾ƿ��� ������ �籸���ϴ� �ڷ�ƾ
    private IEnumerator ForceRebuildLayout(GameObject target)
    {
        // �� ������ ���
        yield return new WaitForEndOfFrame();

        // ���̾ƿ� ���� �籸��
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }


    // MassageID�� ������� Product�� Price�� ã�� �޼���
    private long GetProductPriceByMassageID(string massageID)
    {
        List<Product> products = GameManager.Instance.GetProducts();
        string ItemID = instaDmMassage.instance.GetItemIDByMassageID(massageID);
        Product targetProduct = products.FirstOrDefault(product => product.ID == ItemID);
        return targetProduct.Price;
    }
}
