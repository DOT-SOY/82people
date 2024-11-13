using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem particlePrefab; // 파티클 시스템 프리팹
    public int poolSize = 10; // 풀 크기
    private List<ParticleSystem> particlePool; // 파티클 풀
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // 파티클 풀 초기화
        particlePool = new List<ParticleSystem>();
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem particle = Instantiate(particlePrefab, transform);
            particle.Stop();
            particlePool.Add(particle);
        }
    }

    // 클릭한 위치에서 파티클을 재생하는 메서드
    public void PlayParticleAtClickPosition()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 감지
        {
            Vector3 clickPosition = Input.mousePosition;
            clickPosition.z = 10f; // 카메라에서의 거리 설정 (적절히 조정 필요)

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(clickPosition);
            PlayParticleAtPosition(worldPosition);
        }
    }

    // 특정 위치에서 파티클을 재생하는 메서드
    private void PlayParticleAtPosition(Vector3 position)
    {
        ParticleSystem particle = GetAvailableParticle();
        if (particle != null)
        {
            particle.transform.position = position;
            particle.Play();
        }
    }

    // 사용 가능한 파티클 시스템을 반환하는 메서드
    private ParticleSystem GetAvailableParticle()
    {
        foreach (ParticleSystem particle in particlePool)
        {
            if (!particle.isPlaying)
            {
                return particle;
            }
        }

        // 풀에 사용 가능한 파티클 시스템이 없을 경우, 새로운 파티클 시스템을 생성하여 추가
        ParticleSystem newParticle = Instantiate(particlePrefab, transform);
        particlePool.Add(newParticle);
        return newParticle;
    }
}
