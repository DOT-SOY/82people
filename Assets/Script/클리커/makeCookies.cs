using System.Collections;
using UnityEngine;

public class makeCookies : MonoBehaviour
{
    public ParticleManager particleManager; // ��ƼŬ �Ŵ��� ����

    private Coroutine earningCoroutine; // �ڷ�ƾ�� ������ ����

    private void Start()
    {
        // Start the coroutine to earn money per second
        earningCoroutine = StartCoroutine(EarnMoneyPerSecond());
    }

    private void OnEnable()
    {
        // GameObject�� Ȱ��ȭ�� ������ �ڷ�ƾ�� ����
        if (earningCoroutine == null)
        {
            earningCoroutine = StartCoroutine(EarnMoneyPerSecond());
        }
    }

    private void OnDisable()
    {
        // GameObject�� ��Ȱ��ȭ�� �� �ڷ�ƾ�� ����
        if (earningCoroutine != null)
        {
            StopCoroutine(earningCoroutine);
            earningCoroutine = null;
        }
    }

    // �ʴ� incomePerSecond��ŭ ���� ���� �޼���
    private IEnumerator EarnMoneyPerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(ClickerManager.Instance.PerSecond); // PerSecond �ֱ�� ����
            ClickerManager.Instance.totalEarnings += ClickerManager.Instance.incomePerSecond;
            Debug.Log("Earnings per second added. Current Total Earnings: " + ClickerManager.Instance.totalEarnings);

            ClickerManager.Instance.UpdateYoutubeMoney();
        }
    }

    // �޼��带 �������� �� incomePerClick��ŭ ���� ���� �޼���
    public void EarnMoneyPerClick()
    {
        ClickerManager.Instance.totalEarnings += ClickerManager.Instance.incomePerClick;
        Debug.Log("Earnings per click added. Current Total Earnings: " + ClickerManager.Instance.totalEarnings);

        ClickerManager.Instance.UpdateYoutubeMoney();

        // ��ƼŬ �Ŵ����� ���� ��ƼŬ ���
        if (particleManager != null)
        {
            particleManager.PlayParticleAtClickPosition();
        }
    }
}
