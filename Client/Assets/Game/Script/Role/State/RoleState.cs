using UnityEngine;
using System.Collections;

/*
 * *********************************************************
 * 名称：角色状态
 * 描述：基类
 * *********************************************************
 */

public abstract class RoleState
{
    #region Fields
    protected StatePart m_statePart;
    protected enRoleState m_forceLeveType;//当前状态如果要切换到m_forceLeveType，那么不用判断canLeave,直接切换
    #endregion

    #region Properties
    public abstract enRoleState Type { get; }

    //是不是当前状态
    public bool IsCur { get { return m_statePart.CurStateType == Type; } }
    public RoleStateBorn StateBorn { get { return m_statePart.StateBorn; } }
    public RoleStateFree StateFree { get { return m_statePart.StateFree; } }
    public RoleStateMove StateMove { get { return m_statePart.StateMove; } }
    public RoleStateCombat StateCombat { get { return m_statePart.StateCombat; } }


    //1 模型层(一般是内部调用，非部件一定要慎用)
    public Role Parent { get { return m_statePart.Parent; } }
    public RoleModel RoleModel { get { return m_statePart.RoleModel; } }
    public TranPart TranPart { get { return m_statePart.TranPart; } }
    public AniPart AniPart { get { return m_statePart.AniPart; } }
    public RenderPart RenderPart { get { return m_statePart.RenderPart; } }
    public StatePart StatePart { get { return m_statePart; } }

    //2 数据层(属性、状态、仇恨）
    public PropPart PropPart { get { return m_statePart.PropPart; } }
    public BuffPart BuffPart { get { return m_statePart.BuffPart; } }
    public HatePart HatePart { get { return m_statePart.HatePart; } }

    //3 战斗层(移动、战斗、死亡等，和StatePart有对应关系，本质上是为了更好地控制StatePart)
    public DeadPart DeadPart { get { return m_statePart.DeadPart; } }
    public MovePart MovePart { get { return m_statePart.MovePart; } }
    public CombatPart CombatPart { get { return m_statePart.CombatPart; } }


    #endregion

    #region Frame
    public RoleState(StatePart statePart, enRoleState forceLeveType)
    {
        this.m_statePart = statePart;
        this.m_forceLeveType = forceLeveType;
    }

    public virtual bool CanEnter() { return true; }

    //下一个状态能不能强制使这个状态离开
    public virtual bool IsForceLeave(RoleState nextState)
    {
        return (m_forceLeveType & nextState.Type) != 0;//有这个标志位则禁止进入
    }

    public abstract void Enter(object param);


    //重新传递参数给当前状态,比如走动中换方向，使用技能时强制使用第二个技能
    public abstract void Do(object param);


    //判断当前状态能不能离开
    public abstract bool CanLeave(RoleState nextState);


    public abstract void Leave();


    public abstract void Update();

    //被对象池回收的时候
    public virtual void OnDestroy() { }
    #endregion

    public bool GotoState(object param = null, bool force = false, bool putIdTypeParam = false)
    {
        return m_statePart.GotoState(Type, param, force, putIdTypeParam);
    }
}
