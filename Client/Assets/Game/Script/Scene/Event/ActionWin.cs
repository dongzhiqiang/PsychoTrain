using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionWin : SceneAction
{

    public ActionCfg_Win mActionCfg;
    public override void Init(ActionCfg actionCfg)
    {
        base.Init(actionCfg);
        mActionCfg = actionCfg as ActionCfg_Win;
    }

    public override void OnAction()
    {
        if (SceneMgr.SceneDebug)
            Debug.Log("设置胜利");

        //boss可能有死亡效果 没播完 因为胜利暂停后会立即停止销毁
        Room.instance.StartCoroutine(CoSetWin());
    }

    IEnumerator CoSetWin()
    {
        yield return new WaitForSeconds(0.5f);
        EventMgr.FireAll(MSG.MSG_SCENE, MSG_SCENE.WIN, true);

        //先关闭所有界面
        UIMgr.instance.CloseAll();

        LevelMgr.instance.SetWin();
    }
}

