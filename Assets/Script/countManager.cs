using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class countManager : MonoBehaviour
{
    public static countManager Instance { get; private set; } // 싱글톤 인스턴스

    public int devilDM; // 악마 DM 카운트
    public int Odap; // 오답 카운트
    public int curruntEndingID; // 현재 엔딩 ID

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

    public void Becomedevil()
    {
        devilDM++;
    }

    public void startEnding()
    {
        if(GameManager.Instance.InfiniteMod == false)
        {
            if (GameManager.Instance.money < 0) //엔딩1 - 거지가 되...
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
            else if (GameManager.Instance.money > 20000000000) // 엔딩2 - 나는야 팔이피플
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
            else if (devilDM > 99) // 엔딩3 - 그러고보니 내가 사업자등록을 했던가..?
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
            else if (Odap == 0 && GameManager.Instance.money > 10000000000) // 엔딩4 - 존경하는 국민여러분, 기호 82번입니다
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
            else if (devilDM == 0 && GameManager.Instance.money > 10000000000) // 엔딩5 - 더러워서 못해먹겠네
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
