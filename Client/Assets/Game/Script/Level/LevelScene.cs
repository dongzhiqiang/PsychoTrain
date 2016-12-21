using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//关卡场景
public class LevelScene : LevelBase
{

    public static int m_oldLevel = 0;
    public static int m_oldExp = 0;

    static int lastSecond = 20;

    public float startTime;

    List<string> showWaveGroupIdList;

    string curWaveGroupFlag = "";
    //int curWaveNum;
    int maxWaveNum;
    int prevGroupWaveNum = 0;
    protected bool bShowLimitTime = false;

    //UILevelAreaWave uiAreaWave;

    #region Frame
    //场景切换完成
    public override void OnLoadFinish()
    {
        //打开摇杆和魂值、金币、物品栏
        UILevel uiLevel = UIMgr.instance.Open<UILevel>();

        //打开通关条件
        //uiLevel.Open<UILevelAreaCondition>();
        uiLevel.Open<UILevelAreaGizmos>();

        if (Room.instance.roomCfg.time > 0)     //大于0 倒计时
        {
            var area = uiLevel.Open<UILevelAreaTime>();
            area.SetTime(Room.instance.roomCfg.time);
        }
        else if (Room.instance.roomCfg.time == 0)   //等于0 正计时
        {
            uiLevel.Open<UILevelAreaTime>();
        }

        //SceneCfg.SceneData sceneData = SceneMgr.instance.SceneData;
        //curWaveGroupFlag = sceneData.mShowWaveGroupId;
        //if (!string.IsNullOrEmpty(curWaveGroupFlag))
        //{
        //    showWaveGroupIdList = sceneData.mGroupIdList;
        //}

        //maxWaveNum = 0;
        //if (showWaveGroupIdList != null)
        //{
        //    for (int i = 0; i < sceneData.mRefGroupList.Count; i++)
        //    {
        //        if (showWaveGroupIdList.Contains(sceneData.mRefGroupList[i].groupFlag))
        //        {
        //            maxWaveNum += sceneData.mRefGroupList[i].refreshNum;
        //        }
        //    }
        //}

        //uiAreaWave = UIMgr.instance.Get<UILevel>().Get<UILevelAreaWave>();

        startTime = TimeMgr.instance.logicTime;
        bShowLimitTime = false;
    }

    public override void OnEnterAgain()
    {

        //打开摇杆和魂值、金币、物品栏
        UILevel uiLevel = UIMgr.instance.Open<UILevel>();

        //打开通关条件
        //uiLevel.Open<UILevelAreaCondition>();
        uiLevel.Open<UILevelAreaGizmos>();

        if (Room.instance.roomCfg.time > 0)     //大于0 倒计时
        {
            var area = uiLevel.Open<UILevelAreaTime>();
            area.SetTime(Room.instance.roomCfg.time);
        }
        else if (Room.instance.roomCfg.time == 0)   //等于0 正计时
        {
            uiLevel.Open<UILevelAreaTime>();
        }
    }

    public override void SendResult(bool isWin)
    {
        Debug.Log(string.Format("通关时间 : {0}", TimeMgr.instance.logicTime - startTime));
        //单机直接回城
        if (Main.instance.isSingle)
        {
            LevelMgr.instance.GotoMaincity();
            return;
        }
        bShowLimitTime = false;
    }

    //角色死亡 isNow:是否立即销毁
    public override void OnRoleDead(Role role, bool isNow)
    {
        if (role.IsHero)
            return;
    }

    public override void OnLeave()
    {
        Dictionary<int, Role> newDict = new Dictionary<int, Role>();
        Role hero = RoleMgr.instance.Hero;
        if (hero != null && hero.State == Role.enState.alive)
        {
            newDict.Add(hero.Id, hero);
        }
        mRoleDic.Clear();
        mRoleDic = newDict;
    }

    public override void OnUpdate()
    {
        if (State == LevelState.End)
            return;

        base.OnUpdate();
        RoomCfg cfg = Room.instance.roomCfg;
        if (cfg == null)
            return;

        if (cfg.limitTime <= 0)
            return;

        if (!bShowLimitTime && cfg.limitTime - lastSecond <= TimeMgr.instance.logicTime - startTime)
        {
            UILevel uiLevel = UIMgr.instance.Get<UILevel>();
            if (uiLevel.IsOpen)
            {
                var area = uiLevel.Open<UILevelAreaTime>();
                area.SetTime(lastSecond);
            }

            bShowLimitTime = true;
        }

        if (cfg.limitTime <= TimeMgr.instance.logicTime - startTime)
        {
            LevelMgr.instance.SetLose();
        }
    }

    public override void OnRoleEnter(Role role)
    {
        //if (role == null || showWaveGroupIdList == null)
        //    return;

        //for(int i = 0; i < showWaveGroupIdList.Count; i++)
        //{
        //    if (role.GetFlag(showWaveGroupIdList[i]) > 0)
        //    {
        //        //开始刷新出需要记录波数的刷新组时 打开显示波数的界面
        //        if (showWaveGroupIdList[i] == SceneMgr.instance.SceneData.mShowWaveGroupId)
        //        {
        //            if (!uiAreaWave .IsOpen)
        //                uiAreaWave.OpenArea(null);
        //        }
        //        //下一个刷新组刷怪了
        //        if (curWaveGroupFlag != showWaveGroupIdList[i])
        //        {
        //            curWaveGroupFlag = showWaveGroupIdList[i];
        //            prevGroupWaveNum = GetCurWaveNum();         //记录下之前所有刷新组刷的波数
        //        }
        //        RefreshBase refGroup = SceneMgr.instance.GetRefreshNpcByFlag(showWaveGroupIdList[i]);
        //        curWaveNum = refGroup.waveNum;

        //        SetWaveNum();

        //        return;
        //    }
        //}
    }

    public override void OnHeroEnter(Role hero)
    {
        if (hero != null)
        {
            m_oldLevel = hero.GetInt(enProp.level);
            m_oldExp = hero.GetInt(enProp.exp);

            hero.AIPart.RePlay();
        }
    }

    #endregion

    void OnFlyEnd()
    {

    }

    public void OnWin()
    {
        Room.instance.StartCoroutine(CoWin());
    }

    IEnumerator CoWin()
    {
        yield return new WaitForSeconds(1);

        //隐藏宠物
        Role hero = RoleMgr.instance.Hero;
        if (hero != null)
        {
            hero.RoleModel.Foot.gameObject.SetActive(false);
        }

        //打开界面
        //yield return Room.instance.StartCoroutine(UIMgr.instance.Get<UILevelEnd>().onLevelEnd(vo));
    }

    public void OnLose()
    {
        Room.instance.StartCoroutine(CoLose());
    }

    IEnumerator CoLose()
    {
        yield return 0;
    }
}
