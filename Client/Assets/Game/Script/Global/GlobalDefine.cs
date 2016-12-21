using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//系统图标枚举,注意只能在最后加
public enum enSystem
{
    min,
    none,
    other,
    task,//任务
    weapon,//武器
    weapon1, //武器-2
    weapon2, //武器-3
    weapon3, //武器-4
    setting,//系统设置
    max,
}

public enum enPart
{
    prop,
    tran,
    ani,
    render,
    state,
    buff,
    hate,
    dead,
    move,
    combat,
    ai,
    item,
    max,
}

//角色部件由谁创建
public enum enPartCreate
{
    role,   //角色类创建
    model,  //模型类创建
    none,   //外部添加的部件，role类不会自动创建，注意要在角色的init之前添加
}


public class GlobalConst
{
    public const string Flag_Locing = "RoleMgrLocking";

}