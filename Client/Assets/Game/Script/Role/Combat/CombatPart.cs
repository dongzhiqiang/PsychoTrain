using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPart : RolePart
{
    #region Properties
    public override enPart Type { get { return enPart.combat; } }

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
