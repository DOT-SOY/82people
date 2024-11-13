using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class countManager : MonoBehaviour
{
    public static countManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    public int devilDM; // �Ǹ� DM ī��Ʈ
    public int Odap; // ���� ī��Ʈ
    public int curruntEndingID; // ���� ���� ID

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

    public void Becomedevil()
    {
        devilDM++;
    }

    public void startEnding()
    {
        if(GameManager.Instance.InfiniteMod == false)
        {
            if (GameManager.Instance.money < 0) //����1 - ������ ��...
            {
                SoundManager.instance.PlayBGM(5);
                curruntEndingID = 0;

                if (ScriptManager.Instance != null)
                {
                    ScriptManager.Instance.ActivateHelper();
                    Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_1[ScriptManager.Instance.endingorId];
                    PopupManager.Instance.ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                }
            }
            else if (GameManager.Instance.money > 20000000000) // ����2 - ���¾� ��������
            {
                SoundManager.instance.PlayBGM(7);
                curruntEndingID = 1;

                if (ScriptManager.Instance != null)
                {
                    ScriptManager.Instance.ActivateHelper();
                    Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_2[ScriptManager.Instance.endingorId];
                    PopupManager.Instance.ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                }
            }
            else if (devilDM > 99) // ����3 - �׷����� ���� ����ڵ���� �ߴ���..?
            {
                SoundManager.instance.PlayBGM(4);
                curruntEndingID = 2;

                if (ScriptManager.Instance != null)
                {
                    ScriptManager.Instance.ActivateHelper();
                    Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_3[ScriptManager.Instance.endingorId];
                    PopupManager.Instance.ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                }
            }
            else if (Odap == 0 && GameManager.Instance.money > 10000000000) // ����4 - �����ϴ� ���ο�����, ��ȣ 82���Դϴ�
            {
                SoundManager.instance.PlayBGM(3);
                curruntEndingID = 3;

                if (ScriptManager.Instance != null)
                {
                    ScriptManager.Instance.ActivateHelper();
                    Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_4[ScriptManager.Instance.endingorId];
                    PopupManager.Instance.ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                }
            }
            else if (devilDM == 0 && GameManager.Instance.money > 10000000000) // ����5 - �������� ���ظ԰ڳ�
            {
                SoundManager.instance.PlayBGM(6);
                curruntEndingID = 4;

                if (ScriptManager.Instance != null)
                {
                    ScriptManager.Instance.ActivateHelper();
                    Dialogue nextDialogue = ScriptManager.Instance.endingDialogue_5[ScriptManager.Instance.endingorId];
                    PopupManager.Instance.ShowMessage(nextDialogue.dialogue, nextDialogue.imgName);
                }
            }
        }
    }
}
