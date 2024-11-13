using System.Collections;
using UnityEngine;

public class makeCookies : MonoBehaviour
{
    public ParticleManager particleManager; // 파티클 매니저 변수

    private Coroutine earningCoroutine; // 코루틴을 관리할 변수

    private void Start()
    {
        // Start the coroutine to earn money per second
        earningCoroutine = StartCoroutine(EarnMoneyPerSecond());
    }

    private void OnEnable()
    {
        // GameObject가 활성화될 때마다 코루틴을 시작
        if (earningCoroutine == null)
        {
            earningCoroutine = StartCoroutine(EarnMoneyPerSecond());
        }
    }

    private void OnDisable()
    {
        // GameObject가 비활성화될 때 코루틴을 멈춤
        if (earningCoroutine != null)
        {
            StopCoroutine(earningCoroutine);
            earningCoroutine = null;
        }
    }

    // 초당 incomePerSecond만큼 돈을 버는 메서드
    private IEnumerator EarnMoneyPerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(ClickerManager.Instance.PerSecond); // PerSecond 주기로 실행
            ClickerManager.Instance.totalEarnings += ClickerManager.Instance.incomePerSecond;
            Debug.Log("Earnings per second added. Current Total Earnings: " + ClickerManager.Instance.totalEarnings);

            ClickerManager.Instance.UpdateYoutubeMoney();
        }
    }

    // 메서드를 실행했을 때 incomePerClick만큼 돈을 버는 메서드
    public void EarnMoneyPerClick()
    {
        ClickerManager.Instance.totalEarnings += ClickerManager.Instance.incomePerClick;
        Debug.Log("Earnings per click added. Current Total Earnings: " + ClickerManager.Instance.totalEarnings);

        ClickerManager.Instance.UpdateYoutubeMoney();

        // 파티클 매니저를 통해 파티클 재생
        if (particleManager != null)
        {
            particleManager.PlayParticleAtClickPosition();
        }
    }
}
