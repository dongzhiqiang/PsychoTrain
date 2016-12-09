using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class IdType
{
    int _poolId;
    bool _isInPool;
    PoolBase _pool;

    public bool IsInPool { get { return _isInPool; } set { _isInPool = value; } }
    public int Id { get { return _poolId; } set { _poolId = value; } }
    public PoolBase Pool { get { return _pool; } set { _pool = value; } }

    public void Put()
    {
        if (IsInPool)
            return;
        Pool.Put2(this);
    }

    public bool IsDestroy(int curId)
    {
        return curId != _poolId || _isInPool;
    }

    public virtual void OnInit() { }
    public virtual void OnClear() { }

}

public class PoolBase : IPool
{
    const int Check_Leak_Count = 100;
    static int s_idCounter = 0;
    int m_counter = 0;

    Stack<IdType> m_pool = new Stack<IdType>();

    public string Name { get { return this.GetType().ToString(); } }
    public int Count { get { return m_pool.Count; } }
    public int TotalCount { get { return m_counter; } }

    public PoolBase()
    {
        if (PoolMgr.instance != null)
            PoolMgr.instance.AddPool(this);
    }

    public T Get<T>() where T : IdType, new()
    {
        T it;
        if (m_pool.Count == 0)
        {
            it = new T();
            it.Pool = this;
            ++m_counter;
            if (m_counter > Check_Leak_Count)
                Debuger.LogError(string.Format("类型对象池({0})分配的{1}过多，请检查是不是没有put", typeof(T).ToString(), m_counter));
        }
        else
            it = (T)m_pool.Pop();

        it.Id = ++s_idCounter;
        it.IsInPool = false;
        it.OnInit();
        return it;
    }

    public void Put2(IdType it)
    {
        if (it.IsInPool == true)
        {
            Debuger.LogError("类型对象池,重复放入对象：" + it.GetType().ToString());
            return;
        }

        it.OnClear();
        it.IsInPool = true;
        m_pool.Push(it);
    }

    public void Clear()
    {
        m_pool.Clear();
    }
}

public class IdTypePool<T> : PoolBase
    where T : IdType, new()
{
    static IdTypePool<T> s_instance = null;
    static IdTypePool<T> Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new IdTypePool<T>();
            return s_instance;
        }
    }

    public static T Get()
    {
        return Instance.Get<T>();
    }

    public static void Put(T t)
    {
        Instance.Put2(t);
    }
}
