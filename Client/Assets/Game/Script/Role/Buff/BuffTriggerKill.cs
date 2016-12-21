﻿#region Header
/**
 * 名称：主动杀死触发
 * 描述：
状态列表,作用对象,释放者
当角色杀死别人的时候，给角色加状态
 **/
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
public class BuffTriggerKillCfg : BuffExCfg
{
    public List<int> buffIds = new List<int>();
    public enBuffTargetType targetType = enBuffTargetType.self;
    public enBuffTargetType sourceType = enBuffTargetType.self;

    public override bool Init(string[] pp)
    {
        if (pp.Length < 1)
            return false;

        //状态列表
        int i = 0;
        if (int.TryParse(pp[0], out i))
            buffIds.Add(i);
        else
        {
            if (!StringUtil.TryParse(pp[0].Split('|'), ref buffIds))
                return false;
        }

        //作用对象
        if (pp.Length > 1 && int.TryParse(pp[1], out i))
            targetType = (enBuffTargetType)i;

        //释放者
        if (pp.Length > 2 && int.TryParse(pp[2], out i))
            sourceType = (enBuffTargetType)i;
        return true;
    }
    public override void PreLoad()
    {
        for (int i = 0; i < buffIds.Count; ++i)
        {
            BuffCfg.ProLoad(buffIds[i]);
        }
    }
}

public class BuffTriggerKill : Buff
{
    public BuffTriggerKillCfg ExCfg { get { return (BuffTriggerKillCfg)m_cfg.exCfg; } }

    int m_observer;


    //初始化，状态创建的时候调用，一般用来解析下参数
    public override void OnBuffInit()
    {


    }

    //处理，可能会调用多次
    public override void OnBuffHandle()
    {
        if (m_count > 1)
        {
            LogError("不需要执行多次");
            return;
        }
        //监听技能事件
        m_observer = m_parent.Add(MSG_ROLE.KILL, OnEvent);
    }

    //结束
    public override void OnBuffStop(bool isClear)
    {
        if (m_observer != EventMgr.Invalid_Id) { EventMgr.Remove(m_observer); m_observer = EventMgr.Invalid_Id; }

    }


    void OnEvent(object p, object p2, object p3, EventObserver ob)
    {
        Role target = (Role)p;

        int poolId = this.Id;
        int parentId = m_parent.Id;
        int targetId = target.Id;

        //作用对象
        Role buffTarget = this.GetRole(ExCfg.targetType, target);
        if (buffTarget == null)
            return;
        BuffPart buffPart = buffTarget.BuffPart;

        //释放者
        Role buffSource = this.GetRole(ExCfg.sourceType, target);
        if (buffSource == null)
            return;

        for (int i = 0; i < ExCfg.buffIds.Count; ++i)
        {
            buffPart.AddBuff(ExCfg.buffIds[i], buffSource);
            if (IsUnneedHandle(poolId, m_parent, parentId, target, targetId))
                return;
        }
    }


}

