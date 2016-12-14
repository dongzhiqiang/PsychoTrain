#region Header
/**
 * 名称：角色行为状态机部件
 * 描述：
 * 1.行为状态维护，角色在任意时刻有且只有一个行为
 * 2.角色是不是处于某些特殊状态(和当前行为没有必然联系，但是可能会影响到行为间的切换，所以也放到这里了)的维护，比如空中状态、气力状态、眩晕
 * 
 * 
 * 关于没有模型角色的特殊之处，没有模型的角色的SimpleRole上勾选着"无模型角色",没有模型角色由于不能播放动作所以不能进入任何行为(待机、移动、攻击、被击等等),它处于空行为中
 * 关于没有碰撞的角色，即没有CharacterController组件，移动需要依靠CharacterController，所以没有碰撞的角色不可移动或者被移动，但是可以设置位置
 * 关于不受伤害，免疫伤害事件+免疫伤害反弹+免疫增减血就可以实现，见状态表里不受伤害事件的实现
 * 关于不可移动，对于没有模型的角色是不能进入移动行为的(一直都是empty状态),但是TranPart还是会计算其他移动
 *               对于没有碰撞的角色是不能位移(除了直接设置位置)，TranPart不会计算（移动事件、移动行为、被击行为都内部做了处理）
 *               对于普通角色，要实现类似于束缚效果的话可以给角色添加一个不可位移的状态类型
 * 关于不可受击(霸体),对于没有模型的角色是不会进入受击行为的(一直都是empty状态)
 *               对于普通角色，可以给这个角色添加免疫受击行为的状态(被击、浮空、击飞、抓取)
 * 关于无视攻击的角色如何配，对陷阱类型的角色，不会被自动朝向和ai监测到，再给角色加个免疫所有事件和不受伤害的状态
 *               对于普通类型的角色，暂时没有手段可以不被自动朝向和ai检测到
 **/
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enRoleState
{
    empty = 0x0000, //空
    born = 0x0001,  //出生
    free = 0x0002,  //空闲
    move = 0x0004,  //移动
    combat = 0x0008,//战斗
    beHit = 0x0010, //被击(支持击飞、击倒、击飞、二次浮空、被抓取)
    dead = 0x0020,  //死亡
    ani = 0x0040,   //动作序列
    fall = 0x0080,  //下落
    round = 0x0100, //包围
    pauseAni = 0x0200, //定身
}

public class StatePart : RolePart
{
    #region Fields
    Dictionary<enRoleState, RoleState> m_states = new Dictionary<enRoleState, RoleState>();
    Dictionary<string, RoleState> m_statesByName = new Dictionary<string, RoleState>();
    RoleState m_curState;
    RoleStateEmpty m_stateEmpty;
    RoleStateBorn m_stateBorn;
    RoleStateFree m_stateFree;
    RoleStateMove m_stateMove;
    RoleStateCombat m_stateCombat;
    RoleStateBehit m_stateBihit;
    RoleStateDead m_stateDead;
    RoleStateAni m_stateAni;
    RoleStateFall m_stateFall;
    RoleStateRound m_stateRound;
    RoleStateSwitchWeapon m_stateSwitchWeapon;
    RoleStatePauseAni m_statePauseAni;

    bool m_isChanging = false;//防止GotoState多次调用
    enRoleState m_dalayGotoState = enRoleState.empty;//防止GotoState多次调用
    object m_dalayGotoParam;//防止GotoState多次调用
    bool m_delayForce = false;


    bool m_air;
    CameraHandle m_airCameraHandle;

    int m_limitMoveCounter = 0;
    int m_silentCounter = 0;

    public static Dictionary<string, enRoleState> s_stateTypes = new Dictionary<string, enRoleState>();
    #endregion

    #region Properties
    public override enPart Type { get { return enPart.state; } }

    #endregion

    #region 特殊状态维护
    //是不是在空中
    public bool IsAir
    {
        get { return m_air; }
        set
        {
            if (m_air == true && value == true)
            {
                Debuger.LogError("重复进入空中状态");
                return;
            }
            m_air = value;

            if (m_air)
            {
                //切换到空中镜头
                if (m_airCameraHandle != null)
                {
                    m_airCameraHandle.Release();
                    m_airCameraHandle = null;
                    Debuger.LogError("逻辑错误，空中镜头没有释放");
                }
                m_airCameraHandle = CameraMgr.instance.StillLook(this.RoleModel.Model, m_stateFall.CameraMoveTime, m_stateFall.CameraOverTime);
            }
            else
            {
                //释放空中镜头
                if (m_airCameraHandle != null)
                {
                    m_airCameraHandle.Release();
                    m_airCameraHandle = null;
                }
                m_stateFall.ResetHang();
            }
        }
    }


    //是不是处于动画序列播放状态中(可能状态机不处于这个状态)
    public bool IsAnis { get { return m_stateAni.IsPlaying; } }


    //是不是没有模型
    public bool IsModelEmpty { get { return RoleModel.SimpleRole.m_isEmpty; } }

    //是不是没有碰撞,不能进入移动状态，获取移动上下文失败
    public bool IsNoCollider { get { return RoleModel.CC == null; } }

    //是不是不可移动，移动上下文不起作用
    public bool IsLimitMove
    {
        get { return m_limitMoveCounter > 0; }
    }

    //是否处于沉默状态
    public bool IsSilent
    {
        get { return m_silentCounter > 0; }
    }
    #endregion

    

    #region Frame    
    //属于角色的部件在角色第一次创建的时候调用，属于模型的部件在模型第一次创建的时候调用
    public override void OnCreate(RoleModel model)
    {
    }

    //初始化，不保证模型已经创建，每次角色从对象池取出来都会调用(可以理解为Awake)
    public override bool OnInit()
    {
        return true;
    }

    //后置初始化，模型已经创建，每个模块都初始化过一次，每次角色从对象池取出来都会调用(可以理解为Start())
    public override void OnPostInit()
    {
    }

    //每帧更新
    public override void OnUpdate()
    {
    }


    public override void OnDestroy()
    {
    }
    #endregion
}
