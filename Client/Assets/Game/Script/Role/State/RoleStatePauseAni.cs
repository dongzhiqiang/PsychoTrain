using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStatePauseAni : RoleState
{
    #region Fields
    #endregion
    #region Properties
    public override enRoleState Type { get { return enRoleState.pauseAni; } }
    #endregion

    #region Frame
    public RoleStatePauseAni(StatePart rsm, enRoleState enterType)
    : base(rsm, enterType)
    {

    }

    public override void Enter(object param)
    {

    }

    //重新传递参数给当前状态,比如走动中换方向，使用技能时强制使用第二个技能
    public override void Do(object param)
    {
    }

    //判断当前状态能不能离开
    public override bool CanLeave(RoleState nextState)
    {
        return true;
    }

    public override void Leave()
    {

    }

    public override void Update()
    {

    }
    #endregion
}
