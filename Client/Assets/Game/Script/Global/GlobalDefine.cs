using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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