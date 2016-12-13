using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    RoleCxt m_roleCxt;
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
    public bool IsShow { get { return m_roleModel == null ? false : true; } }
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
    public TranPart TranPart { get { return GetPart(enPart.tran) as TranPart; } }
    public AniPart AniPart { get { return GetPart(enPart.ani) as AniPart; } }
    public RenderPart RenderPart { get { return GetPart(enPart.render) as RenderPart; } }
    public StatePart StatePart { get { return GetPart(enPart.state) as StatePart; } }

    //数据层（属性，状态，仇恨）
    public PropPart PropPart { get { return GetPart(enPart.prop) as PropPart; } }
    public BuffPart BuffPart { get { return GetPart(enPart.buff) as BuffPart; } }
    public HatePart HatePart { get { return GetPart(enPart.hate) as HatePart; } }

    //战斗层(移动，战斗，死亡，状态)
    public DeadPart DeadPart { get { return GetPart(enPart.dead) as DeadPart; } }
    public MovePart MovePart { get { return GetPart(enPart.move) as MovePart; } }
    public CombatPart CombatPart { get { return GetPart(enPart.combat) as CombatPart; } }

    //控制层


    //上层逻辑


    #endregion


    #region 人物属性相关，这里提供比较容易获取的接口
    public int GetInt(enProp prop) { return PropPart.GetInt(prop); }

    public void SetInt(enProp prop, int v) { PropPart.SetInt(prop, v); }

    public void AddInt(enProp prop, int v) { PropPart.AddInt(prop, v); }

    public long GetLong(enProp prop) { return PropPart.GetLong(prop); }

    public void SetLong(enProp prop, long v) { PropPart.SetLong(prop, v); }

    public void AddLong(enProp prop, long v) { PropPart.AddLong(prop, v); }

    public float GetFloat(enProp prop) { return PropPart.GetFloat(prop); }

    public float GetPercent(enProp prop, enProp propMax)
    {
        float v = (float)PropPart.GetInt(prop);
        float vMax = PropPart.GetFloat(propMax);
        return Mathf.Clamp01(v / vMax);
    }

    public void SetFloat(enProp prop, float v) { PropPart.SetFloat(prop, v); }

    public void AddFloat(enProp prop, float v) { PropPart.AddFloat(prop, v); }

    public string GetString(enProp prop) { return PropPart.GetString(prop); }

    public void SetString(enProp prop, string v) { PropPart.SetString(prop, v); }

    public void SetFlag(string flag, int n = 1, bool levelTemp = true) { PropPart.SetFlag(flag, n, levelTemp); }

    public void AddFlag(string flag, int n = 1, bool levelTemp = true) { PropPart.AddFlag(flag, n, levelTemp); }

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

    #region Constructors
    public Role()
    {
        for (int i = (int)(enPart.max) - 1; i >= 0; --i)
        {
            var partType = RolePartType.s_parts[(int)i];

            if (partType != null && partType.createType == enPartCreate.role)
            {
                RolePart part = (RolePart)Activator.CreateInstance(partType.partType);
                part.OnCreate(null);
                SetPart(part);
            }
        }
    }

    #endregion

    #region Private Methods
    void OnLoad(GameObject go)
    {
        //已经被RoleMgr销毁
        if (m_state == Role.enState.none)
        {
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).Put(m_roleModel.gameObject);//模型放回对象池
            OnDestroy();
            return;
        }

        //创建失败
        if (go == null)
        {
            RoleMgr.instance.DestroyRole(this);
            return;
        }
        
        //出生点出生方向
        Transform t = go.transform;
        t.position = PosUtil.CaleByTerrains(m_roleCxt.pos);
        t.eulerAngles = m_roleCxt.euler;

        //模型和模型所属部件初始化
        m_roleModel = go.AddComponentIfNoExist<RoleModel>();
        if (!m_roleModel.Init(this))
        {
            RoleMgr.instance.DestroyRole(this);
            return;
        }

        //加载模型结束
        m_isLoading = false;

        //加到角色管理器
        RoleMgr.instance.AliveRole(this);


        //后置初始化,部件间可相互调用，注意顺序
        foreach (RolePart part in m_parts)
        {
            if (part != null)
                part.OnPostInit();
        }

        //结束回调
        if (m_roleCxt.onCreate != null)
            m_roleCxt.onCreate(this);

        //广播出生
        Fire(MSG_ROLE.BORN, this);
    }

    //模型被销毁
    public void OnDestroy()
    {
        m_isLoading = false;
        m_isDestroying = true;
        //模型放回对象池
        if (m_roleModel != null)
        {
            m_roleModel.OnRoleDestroy();
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).Put(m_roleModel.gameObject);//模型放回对象池
            m_roleModel = null;
        }

        //模型的销毁回调
        RolePartType partType;
        RolePart part;
        for (int i = (int)(enPart.max) - 1; i >= 0; --i)//这里要先遍历上层部件，因为上层部件会对底层部件做修改
        {
            part = m_parts[(int)i];
            if (part == null) continue;
            partType = RolePartType.s_parts[i];

            //onDestroy回调
            if (partType == null || partType.createType != enPartCreate.model)
                part.OnDestroy();

            //属于模型的部件要清空
            if (partType != null && partType.createType == enPartCreate.model)
                m_parts[(int)i] = null;
        }

        Fire(MSG_ROLE.DESTROY_MODEL, this);

        m_isDestroying = false;
        m_roleCxt.OnClear();
        IdTypePool<RoleCxt>.Put(m_roleCxt);
    }

    public override void OnClear()
    {
        RolePartType partType;
        RolePart part;
        for (int i = (int)(enPart.max) - 1; i >= 0; --i) //这里要先遍历上层部件，因为上层部件会对底层部件做修改
        {
            part = m_parts[(int)i];
            if (part == null) continue;

            //onClear回调
            part.OnClear();

            //不是属于role的部件要清空，不然下次从对象池拿出来的时候会有一些不应该存在的部件
            partType = RolePartType.s_parts[i];
            if (partType == null || partType.createType != enPartCreate.role)
                m_parts[(int)i] = null;
        }

        //销毁notifier
        if (m_notifier != null)
        {
            Fire(MSG_ROLE.DESTROY, this);
            //检查下剩下什么
            //string log = string.Format("{0}清空，剩余的监听\n{1}", m_cfg.id, m_notifier.LogObservers());
            //Debuger.Log(log);
            m_notifier.Remove();
            m_notifier = null;
        }
    }

    #endregion

    //初始化
    public bool Init(RoleCfg cfg)
    {
        this.m_cfg = cfg;
        Parent = null;
        m_isLoading = false;
        m_isDestroying = false;
        //数据层和上层部件的初始化
        RolePartType partType;
        for (int i = 0; i < (int)enPart.max; ++i)
        {
            partType = RolePartType.s_parts[i];
            if (m_parts[i] != null && partType != null && partType.createType != enPartCreate.model)
                if (!m_parts[i].Init(this))
                    return false;
        }

        //创建消息广播器
        if (m_notifier != null)
        {
            Debuger.LogError("创建的时候发现老的消息广播器还留着");
            m_notifier.Remove();
            m_notifier = null;
        }

        m_notifier = EventMgr.Get();
        m_notifier.SetParent(this);
        m_notifier.onRemoveOb = OnRemoveOb;

        Fire(MSG_ROLE.INIT, this);
        return true;
    }

    //初始化网络信息
    public void InitNet()
    {
        m_isNetRole = true;//初始化过一次网络数据的话就认为是网络角色，销毁后role类不会放回对象池
        for (int i = 0; i < (int)enPart.max; ++i)
        {
            if (m_parts[i] != null)
                m_parts[i].OnNetInit();
        }
    }

    public void Refresh()
    {
        if (m_state != enState.alive)
        {
            Debuger.Log("角色不是alive不能刷新");
            return;
        }

        //这里先临时这样写，以后用换模型的接口替代
        {
            //出生上下文
            RoleCxt cxt = IdTypePool<RoleCxt>.Get();
            cxt.CopyFrom(m_roleCxt);
            cxt.pos = transform.position;
            cxt.euler = transform.eulerAngles;

            //如果相机的跟随对象是自己，那么重新加载模型后要设置回去
            bool needCameraFollow = CameraMgr.instance.GetFollow() == this.transform;

            m_ingoreFire = true;
            this.OnDestroy();
            m_state = enState.init;
            PropPart.FreshBaseProp();
            this.Load(cxt);
            m_state = enState.alive;
            m_ingoreFire = false;

            //由于有预加载了，所以这里模型肯定是加载完的了，直接设置跟随对象
            if (needCameraFollow)
                CameraMgr.instance.SetFollow(this.transform);
        }
    }

    //加载模型
    public void Load(RoleCxt cxt)
    {
        if (m_state != enState.init)
        {
            Debuger.LogError("逻辑错误，只有在初始化态下可以加载模型，当前状态:{0}", m_state);
            return;
        }

        m_isLoading = true;
        m_isDestroying = false;
        m_roleCxt = cxt;

        //创建模型
        GameObject go = GameObjectPool.GetPool(GameObjectPool.enPool.Role).GetImmediately(m_cfg.mod);
        OnLoad(go);
    }

    //注册部件，注意注册的顺序决定了部件的OnInit的顺序，而枚举顺序(倒序)决定了部件的OnPostInit
    public void SetPart(RolePart part)
    {
        if (part != null && m_parts[(int)part.Type] != null)
        {
            Debuger.LogError("重复设置了部件，或者忘记销毁 {0}", part.Type);
        }

        if (part == null)
        {
            Debuger.LogError("设置进来空的部件");
        }
        m_parts[(int)part.Type] = part;
    }

    public RolePart GetPart(enPart type)
    {
        if (IsInPool)
        {
            Debuger.LogError("逻辑错误，获取了已经被销毁的角色的部件:{0}", type);
            return null;
        }
        return m_parts[(int)type];
    }

    void OnRemoveOb(EventObserver observer)
    {
        m_unBindModelObs.Remove(observer.Id);
    }

    public void RemoveModelObjs()
    {
        if (m_notifier != null)
        {
            List<EventObserver> obs = new List<EventObserver>(m_notifier.observersById.Values);
            foreach(var ob in obs)
            {
                if (!m_unBindModelObs.Contains(ob.Id))
                    EventMgr.Remove(ob);
            }
        }
    }

    //用于检查角色是不是已经不在alive状态
    public bool IsUnAlive(int poolId)
    {
        if (this.IsDestroy(poolId))
            return true;

        if (this.State != enState.alive)
            return true;

        return false;
    }

    public void PreLoad()
    {
        RoleCfg.PreLoad(this.Cfg.id);
        
        for (int i = 0; i < (int)enPart.max; ++i)
        {
            if (m_parts[i] != null)
                m_parts[i].OnPreLoad();
        }
    }

    //显示隐藏角色，注意要先用IsShow判断下，以免重复设置
    public void Show(bool show)
    {
        if (m_roleModel == null)
        {
            Debuger.LogError("模型还没有创建出来就设置显示状态:{0}", show);
            return;
        }
        m_roleModel.Show(show);
    }

    public float DistanceSq(Role other)
    {
        if (this.State != enState.alive || this.transform == null)
        {
            Debuger.LogError("自己不在alive状态，不能计算距离");
        }

        if (other == null || other.State != enState.alive || other.transform == null)
        {
            Debuger.LogError("角色不在alive状态，不能计算距离");
            return float.MaxValue;
        }

        Vector3 a = this.transform.position;
        Vector3 b = other.transform.position;

        float dx = b.x - a.x;
        float dz = b.z - a.z;
        return dx * dx + dz * dz;
    }

    public float Distance(Role other)
    {
        return Mathf.Sqrt(DistanceSq(other));
    }
}
