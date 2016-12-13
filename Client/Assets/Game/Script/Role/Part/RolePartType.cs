using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//角色部件创建信息
public class RolePartType
{
    public System.Type partType;
    public enPartCreate createType;
    public static RolePartType[] s_parts = new RolePartType[(int)enPart.max];
    public RolePartType(System.Type partType, enPartCreate createType) { this.partType = partType; this.createType = createType; }
    static RolePartType()
    {
        //注册需要由框架创建的部件,注意这个顺序代表enPart枚举序
        s_parts[(int)enPart.prop] = new RolePartType(typeof(PropPart), enPartCreate.role);
        s_parts[(int)enPart.tran] = new RolePartType(typeof(TranPart), enPartCreate.model);
        s_parts[(int)enPart.ani] = new RolePartType(typeof(AniPart), enPartCreate.model);
        s_parts[(int)enPart.render] = new RolePartType(typeof(RenderPart), enPartCreate.model);
        s_parts[(int)enPart.state] = new RolePartType(typeof(StatePart), enPartCreate.model);
        s_parts[(int)enPart.buff] = new RolePartType(typeof(BuffPart), enPartCreate.role);
        s_parts[(int)enPart.dead] = new RolePartType(typeof(DeadPart), enPartCreate.model);
        s_parts[(int)enPart.move] = new RolePartType(typeof(MovePart), enPartCreate.model);
        s_parts[(int)enPart.combat] = new RolePartType(typeof(CombatPart), enPartCreate.model);
        s_parts[(int)enPart.ai] = new RolePartType(typeof(AIPart), enPartCreate.model);
    }
}
