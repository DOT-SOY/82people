using System.Collections.Generic;
using UnityEngine;

public class InActiveInsta : MonoBehaviour
{
    // Inspector���� ������ �� �ִ� ���ӿ�����Ʈ ����Ʈ
    public List<GameObject> gameObjectsToDeactivate;

    // ����Ʈ�� ���ӿ�����Ʈ�� ��Ȱ��ȭ��Ű�� �޼���
    public void DeactivateGameObjects()
    {
        InstarManager.Instance.ActiveAndInActive();
    }
}
