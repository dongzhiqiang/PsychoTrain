using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranPart : RolePart
{
    //模仿重力的贴近地面时的速度
    public static Vector3 Default_Gravity_Speed = new Vector3(0, -20.0f, 0);
    //重力加速度
    public static Vector3 Default_Gravity = new Vector3(0, -9.8F, 0);
    //模仿重力 全局的
    public static Vector3 s_gravity_speed = Default_Gravity_Speed;

    #region Fields
    Transform m_root;
    Transform m_tranModel;
    List<TranPartCxt> m_cxts = new List<TranPartCxt>();//这里sample要保证一定顺序，以后如果删除操作有效率问题，考虑用System.Collections.Specialized.OrderedDictionary\
    List<TranPartCxt> m_removes = new List<TranPartCxt>();
    float m_lastSampleTime = 0;
    #endregion

    #region Properties
    public override enPart Type { get { return enPart.tran; } }
    public bool IsGrounded { get { return m_tranModel.localPosition.y <= 0.01f; } }
    public Vector3 Pos
    {//这里可以认为是模型的位置，用于伤害计算
        get
        {
            ////如果在浮空或者击翻状态下要调整下位置
            //var behit = RSM.StateBehit;
            //if (behit.IsCur && (behit.CurStateType == enBehit.befloat || behit.CurStateType == enBehit.beFly))
            //{
            //    return m_tranModel.position + new Vector3(0, RoleModel.Height * 0.4f, 0);
            //}
            //else
                return m_tranModel.position;
        }
    }

    #endregion

    #region Frame

    //初始化，不保证模型已经创建，每次角色从对象池取出来都会调用(可以理解为Awake)
    public override bool OnInit()
    {
        m_root = RoleModel.Root;
        m_tranModel = RoleModel.Model;
        if (m_cxts.Count != 0)
        {
            Debuger.LogError("逻辑错误");
            m_cxts.Clear();
        }
        return true;
    }

    //后置初始化，模型已经创建，每个模块都初始化过一次，每次角色从对象池取出来都会调用(可以理解为Start())
    public override void OnPostInit()
    {
        m_lastSampleTime = TimeMgr.instance.logicTime;
        ResetHight();
    }

    public override void OnDestroy()
    {
        foreach(TranPartCxt c in m_cxts)
        {
            IdTypePool<TranPartCxt>.Put(c);
        }
        m_cxts.Clear();
        m_tranModel.localEulerAngles = Vector3.zero;//模型方向可能在渐变，这个时候要复位下
    }

    public override void OnUpdate()
    {
        if (Parent.State != Role.enState.alive) return;
        if (!TimeMgr.instance.IsStop && TimeMgr.instance.IsPause)
        {
            if (m_cxts.Count > 0)
            {
                foreach (TranPartCxt c in m_cxts)
                {
                    IdTypePool<TranPartCxt>.Put(c);
                }
                m_cxts.Clear();
            }
            return;
        }
        Sample();
    }

    #endregion

    #region Private Methods
    void Sample()
    {

    }
    #endregion

    //直接设置方向，注意移动中也有可能会改变方向，这个行为可能被覆盖
    //一般是和AddCxt一起用，快速调整方向
    public void SetDir(Vector3 dir)
    {
        dir.y = 0;
        if (dir == Vector3.zero)
            return;
        m_root.forward = dir;
    }

    public void ResetHight()
    {
        if (!RoleModel.IsShow)
            return;

        //if (m_root.GetComponent<SimpleRole>().m_needResetPos)
            m_tranModel.localPosition = Vector3.zero;
    }

    public void SetModelPosAndRot(Vector3 pos, Quaternion rot)
    {
        if (!RoleModel.IsShow)
            return;
        if (RSM.IsNoCollider)
            return;
    }
}
