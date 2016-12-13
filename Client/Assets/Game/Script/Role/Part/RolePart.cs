using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RolePart
{
    #region Fields
    protected Role m_parent;
    #endregion

    #region Properties
    public Role Parent { get { return m_parent; } }
    public abstract enPart Type { get; }

    //模型层(一般内部使用 非部件)
    public RoleModel RoleModel { get { return m_parent.RoleModel; } }
    public Transform transform { get { return m_parent.transform; } }
    public TranPart TranPart { get { return m_parent.TranPart; } }


    #endregion

    #region Frame
    public bool Init(Role parent)
    {
        m_parent = parent;
        return OnInit();
    }

    //属于角色的部件在角色第一次创建的时候调用，属于模型的部件在模型第一次创建的时候调用
    public virtual void OnCreate(RoleModel model) { }

    //初始化，不保证模型已经创建，每次角色从对象池取出来都会调用(可以理解为Awake)
    public abstract bool OnInit();

    //网络数据初始化,注意只有网络角色会被调用
    public virtual void OnNetInit() { }

    //模型创建的时候被调用,可以安全调用其他部件，在所有部件OnModelInit才进行OnPostInit
    public abstract void OnPostInit();

    //模型销毁时调用
    public virtual void OnDestroy() { }

    //角色销毁时调用
    public virtual void OnClear() { }

    //每帧更新
    public virtual void OnUpdate() { }

    //计算战斗力
    public virtual void OnFreshBaseProp(PropertyTable values, PropertyTable rates) { }

    //网络属性变化
    public virtual void OnSyncProps(List<int> props) { }

    //预加载 次接口只有网络角色会调用 关卡怪物不在这加载
    public virtual void OnPreLoad() { }

    #endregion
}
