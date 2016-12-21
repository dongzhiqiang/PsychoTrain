using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleModel : MonoBehaviour
{
    #region Fields
    public bool m_undead = false;
    public bool m_unAttack = false;
    public bool m_debugAI = false;

    public Vector3 m_showOffset = Vector3.zero;
    public Color m_showClr = Color.green;
    public bool m_debugProp = false;
    public bool m_debugBuff = false;
    public bool m_debugNotifier = false;
    public bool m_debugHate = false;
    public bool m_debugFlag = false;
    public string m_debugSkillId = "";//技能id|连击技能id|返回值类型，默认都可以填-1，那么就是全部调试
    public string m_debugState = "";//老状态|新状态，默认都可以填-1，那么就是全部调试，出生,空闲,移动,战斗,被击,死亡,动作序列,下落,换武器

    Role m_parent;
    Transform m_root;
    Transform m_model;
    CharacterController m_CC;   //角色控制器
    Transform m_title;
    Transform m_center;
    Transform m_foot;   //脚底方向

    RolePart[] m_parts = new RolePart[(int)enPart.max]; //模型上的部件

    bool m_isUpdating;
    bool m_created = false;
    bool m_isShow = true;
    float m_height = 1f;
    #endregion

    #region Properties
    public Role Parent { get { return m_parent; } }

    // 底层(一般内部使用，非部件)
    public Transform Root { get { return m_root; } }
    public Transform Model { get { return m_model; } }
    public Transform Tran { get { return m_model != null ? m_model : m_root; } }
    public Transform Title { get { return m_title; } }
    public Transform Center { get { return m_center; } }
    public Transform Foot { get { return m_foot; } }
    public CharacterController CC { get { return m_CC; } }
    public float Radius { get { return m_CC != null ? m_CC.radius : 0.5f; } }
    public float Height { get { return m_height; } }

    public TranPart TranPart { get { return m_parent.TranPart; } }
    public AniPart AniPart { get { return m_parent.AniPart; } }
    public RenderPart RenderPart { get { return m_parent.RenderPart; } }
    public StatePart StatePart { get { return m_parent.StatePart; } }

    //2 数据层(属性、状态、仇恨）
    public PropPart PropPart { get { return m_parent.PropPart; } }
    public BuffPart BuffPart { get { return m_parent.BuffPart; } }
    public HatePart HatePart { get { return m_parent.HatePart; } }

    //3 战斗层(移动、战斗、死亡等，和StatePart有对应关系，本质上是为了更好地控制StatePart)
    public DeadPart DeadPart { get { return m_parent.DeadPart; } }
    public MovePart MovePart { get { return m_parent.MovePart; } }
    public CombatPart CombatPart { get { return m_parent.CombatPart; } }

    //4 控制层(ai)


    public bool IsUpdating { get { return m_isUpdating; } }
    public bool IsCreated { get { return m_created; } }
    public bool IsShow { get { return m_isShow; } }
    #endregion

    #region Mono Frame
    void Update()
    {
        m_isUpdating = true;
        RolePart part;
        for (int i = (int)(enPart.max) - 1; i >= 0; --i)
        {
            part = m_parent.GetPart((enPart)i);
            if (part != null)
                part.OnUpdate();
        }

        if (Root.transform.position.y < -300)
        {
            Debuger.LogError("角色一直往下掉:{0}", Root.transform.position);
            DeadPart.Handle(true);
        }

        m_isUpdating = false;

        if (!RoleMgr.instance.CheckDestroy())
            RoleMgr.instance.CheckDead();

    }
    #endregion
}
