using System.Collections.Generic;
using UnityEngine;

public class InActiveInsta : MonoBehaviour
{
    // Inspector에서 관리할 수 있는 게임오브젝트 리스트
    public List<GameObject> gameObjectsToDeactivate;

    // 리스트의 게임오브젝트를 비활성화시키는 메서드
    public void DeactivateGameObjects()
    {
        InstarManager.Instance.ActiveAndInActive();
    }
}
