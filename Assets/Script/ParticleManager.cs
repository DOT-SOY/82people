using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem particlePrefab; // ��ƼŬ �ý��� ������
    public int poolSize = 10; // Ǯ ũ��
    private List<ParticleSystem> particlePool; // ��ƼŬ Ǯ
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // ��ƼŬ Ǯ �ʱ�ȭ
        particlePool = new List<ParticleSystem>();
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem particle = Instantiate(particlePrefab, transform);
            particle.Stop();
            particlePool.Add(particle);
        }
    }

    // Ŭ���� ��ġ���� ��ƼŬ�� ����ϴ� �޼���
    public void PlayParticleAtClickPosition()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ����
        {
            Vector3 clickPosition = Input.mousePosition;
            clickPosition.z = 10f; // ī�޶󿡼��� �Ÿ� ���� (������ ���� �ʿ�)

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(clickPosition);
            PlayParticleAtPosition(worldPosition);
        }
    }

    // Ư�� ��ġ���� ��ƼŬ�� ����ϴ� �޼���
    private void PlayParticleAtPosition(Vector3 position)
    {
        ParticleSystem particle = GetAvailableParticle();
        if (particle != null)
        {
            particle.transform.position = position;
            particle.Play();
        }
    }

    // ��� ������ ��ƼŬ �ý����� ��ȯ�ϴ� �޼���
    private ParticleSystem GetAvailableParticle()
    {
        foreach (ParticleSystem particle in particlePool)
        {
            if (!particle.isPlaying)
            {
                return particle;
            }
        }

        // Ǯ�� ��� ������ ��ƼŬ �ý����� ���� ���, ���ο� ��ƼŬ �ý����� �����Ͽ� �߰�
        ParticleSystem newParticle = Instantiate(particlePrefab, transform);
        particlePool.Add(newParticle);
        return newParticle;
    }
}
