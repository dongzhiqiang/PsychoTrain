using UnityEngine;
using System.Collections;
using LitJson;
using System;

public class TriggerFactory
{
    public static SceneTrigger GetTrigger(SceneCfg.CheckSaveCfg saveCfg)
    {
        SceneTrigger trigger = null;
        SceneCfg.EventType eveType = (SceneCfg.EventType)saveCfg.eveType;
        switch (eveType)
        {
            case SceneCfg.EventType.None:
                trigger = new TriggerNone(); break;
            case SceneCfg.EventType.StartLevel:
                trigger = new TriggerStartLevel(); break;
            case SceneCfg.EventType.EnterTime:
                trigger = new TriggerEnterTime(); break;
            case SceneCfg.EventType.NpcIDDead:
                trigger = new TriggerNpcDead(); break;
            case SceneCfg.EventType.Area:
                trigger = new TriggerArea(); break;
            case SceneCfg.EventType.Win:
                trigger = new TriggerWin(); break;
            case SceneCfg.EventType.Lose:
                trigger = new TriggerLose(); break;
            case SceneCfg.EventType.RoleDead:
                trigger = new TriggerRoleDead(); break;
            case SceneCfg.EventType.RoleEnter:
                trigger = new TriggerRoleEnter(); break;
            case SceneCfg.EventType.RefreshDead:
                trigger = new TriggerRefreshDead(); break;
            case SceneCfg.EventType.RoleBlood:
                trigger = new TriggerRoleBlood(); break;
            case SceneCfg.EventType.RoleNum:
                trigger = new TriggerRoleNum(); break;
            case SceneCfg.EventType.GroupDeadNum:
                trigger = new TriggerGroupDeadNum(); break;
            case SceneCfg.EventType.FinishEvent:
                trigger = new TriggerFinishEvent(); break;
        }
        if (trigger != null)
            trigger.Init(EventCfgFactory.instance.GetEventCfg(eveType, saveCfg.content));

        return trigger;
    }

}
