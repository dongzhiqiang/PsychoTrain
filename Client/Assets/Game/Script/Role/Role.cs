using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role : IdType
{
    public enum enState
    {
        none,   //在对象池管理中，不再角色管理器中
        init,   //在初始化中
        alive,  //正常
        ignore, //无视状态，不能攻击别人，也不能被别人攻击，中立npc
        dead,   //死亡，游戏对象可见
    }

    #region Fields

    public static RolePartType[] s_partCreateInfos = new RolePartType[(int)enPart.max];

    enState m_state = enState.none; //状态只有RoleMgr改变

    //网络角色销毁时只会删除模型，Role对象不会放回对象池
    bool m_isNetRole = false;
    //是否正在加载模型中
    bool m_isLoading = false;
    //是否正在销毁中
    bool m_isDestroying = false;

    bool m_canMove = true;

    RoleCfg m_cfg;
    RolePart[] m_parts = new RolePart[(int)enPart.max];

    //模型层
    RoleModel m_roleModel;

    //用于向外界广播消息
    EventNotifier m_notifier = null;
    //不希望广播id
    bool m_ingoreFire = false;
    int m_parentId = -1;
    Role m_parent;

    HashSet<int> m_unBindModelObs = new HashSet<int>();
    #endregion

    #region Properties
    public bool IsHero { get { return RoleMgr.instance.Hero == this; } }
    public bool IsLoading { get { return m_isLoading; } }
    public bool IsDestroying { get { return m_isDestroying; } }
    public RoleCfg Cfg { get { return m_cfg; } }
    public bool CanMove { get { return m_canMove; } set { m_canMove = value; } }
    public EventNotifier Notifier { get { return m_notifier; } }
    public enState State
    {
        get { return m_state; }
        set { m_state = value; }
    }
    public bool IsMonster
    {
        get
        {
            if (Cfg.roleType == enRoleType.boss || Cfg.roleType == enRoleType.monster)
                return true;
            else
                return false;
        }
    }
    public Role Parent
    {
        get
        {
            return m_parentId == -1 || m_parent.IsDestroy(m_parentId) ? null : m_parent;
        }
        set
        {
            m_parent = value;
            if (m_parent == null)
                m_parentId = -1;
            else
                m_parentId = m_parent.Id;
        }

    }


    //模型层(一般是内部调用，非部件一定要慎用)
    public RoleModel RoleModel { get { return m_roleModel; } }
    //注意不能利用这个接口设置位置和方向，用TranPart的接口SetPos()、SetDir()
    public Transform transform { get { return m_roleModel != null ? m_roleModel.Root : null; } }

    #endregion

    
    #region 人物属性相关，这里提供比较容易获取的接口
    public int GetInt(enProp prop) { return PropPart.GetInt(prop); }

    public void SetInt(enProp prop, int v) { PropPart.SetInt(prop, v); }

    public void AddInt(enProp prop, int v) { PropPart.AddInt(prop, v); }

    public long GetLong(enProp prop) { return PropPart.GetLong(prop); }

    public void SetLong(enProp prop, long v) { PropPart.SetLong(prop, v); }

    public void AddLong(enProp prop, long v) { PropPart.AddLong(prop, v); }

    public float GetFloat(enProp prop) { return PropPart.GetFloat(prop); }

    public float GetPercent(enProp prop,enProp propMax)
    {
        float v = (float)PropPart.GetInt(prop);
        float vMax = PropPart.GetFloat(propMax);
        return Mathf.Clamp01(v/vMax);
    }

    public void SetFloat(enProp prop, float v) { PropPart.SetFloat(prop, v); }

    public void AddFloat(enProp prop, float v) { PropPart.AddFloat(prop, v); }

    public string GetString(enProp prop) { return PropPart.GetString(prop); }

    public void SetString(enProp prop, string v) { PropPart.SetString(prop, v); }

    public void SetFlag(string flag, int n = 1, bool levelTemp = true) { PropPart.SetFlag(flag, n, levelTemp); }

    public void AddFlag(string flag, int n =1,bool levelTemp = true) { PropPart.AddFlag(flag, n, levelTemp); }

    public int GetFlag(string flag) { return PropPart.GetFlag(flag); }

    public bool HasFlag(string flag) { return PropPart.HasFlag(flag); }

    public enCamp GetCamp() { return (enCamp)PropPart.GetInt(enProp.camp); }

    #endregion


    #region 消息广播相关，这里提供比较易用的接口
    //监听,onFire返回false表示否决(之后的监听者不执行)
    //code见MSG_ROLE类
    public int Add(int code, EventObserver.OnFire onFire, bool bindModel = true)
    {
        if (m_notifier == null) { Debuger.LogError("还没有创建完就监听消息"); return EventMgr.Invalid_Id; }
        int obId = EventMgr.Add(m_notifier, MSG.MSG_ROLE, code, onFire);
        if (!bindModel)
            m_unBindModelObs.Add(obId);
        return obId;
    }
    public int Add(int code, EventObserver.OnFire1 onFire, bool bindModel = true)
    {
        if (m_notifier == null) { Debuger.LogError("还没有创建完就监听消息"); return EventMgr.Invalid_Id; }
        int obId = EventMgr.Add(m_notifier, MSG.MSG_ROLE, code, onFire);
        if (!bindModel)
            m_unBindModelObs.Add(obId);
        return obId;
    }
    public int Add(int code, EventObserver.OnFire2 onFire, bool bindModel = true)
    {
        if (m_notifier == null) { Debuger.LogError("还没有创建完就监听消息"); return EventMgr.Invalid_Id; }
        int obId = EventMgr.Add(m_notifier, MSG.MSG_ROLE, code, onFire);
        if (!bindModel)
            m_unBindModelObs.Add(obId);
        return obId;
    }
    public int Add(int code, EventObserver.OnFire3 onFire, bool bindModel = true)
    {
        if (m_notifier == null) { Debuger.LogError("还没有创建完就监听消息"); return EventMgr.Invalid_Id; }
        int obId = EventMgr.Add(m_notifier, MSG.MSG_ROLE, code, onFire);
        if (!bindModel)
            m_unBindModelObs.Add(obId);
        return obId;
    }
    public int Add(int code, EventObserver.OnFireOb onFire, bool bindModel = true)
    {
        if (m_notifier == null) { Debuger.LogError("还没有创建完就监听消息"); return EventMgr.Invalid_Id; }
        int obId = EventMgr.Add(m_notifier, MSG.MSG_ROLE, code, onFire);
        if (!bindModel)
            m_unBindModelObs.Add(obId);
        return obId;
    }

    public int AddVote(int code, EventObserver.OnVote onVote, bool bindModel = true)
    {
        if (m_notifier == null) { Debuger.LogError("还没有创建完就监听消息"); return EventMgr.Invalid_Id; }
        int obId = EventMgr.AddVote(m_notifier, MSG.MSG_ROLE, code, onVote);
        if (!bindModel)
            m_unBindModelObs.Add(obId);
        return obId;
    }

    //监听人物属性变化
    public int AddPropChange(enProp prop, EventObserver.OnFire onFire) { return Add(MSG_ROLE.PROP_CHANGE + (int)prop, onFire); }

    public int AddPropChange(enProp prop, EventObserver.OnFireOb onFire) { return Add(MSG_ROLE.PROP_CHANGE + (int)prop, onFire); }

    public bool Fire(int code, object param = null, object param2 = null, object param3 = null)
    {
        if (m_ingoreFire) return true;
        if (m_notifier == null)
        {
            Debuger.LogError("还没有创建完就监听消息");
            return true;
        }
        return EventMgr.Fire(m_notifier, MSG.MSG_ROLE, code, param, param2, param3);
    }
    #endregion
}
