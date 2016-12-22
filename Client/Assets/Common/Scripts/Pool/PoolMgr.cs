using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPool
{

    string Name { get; }
    int Count { get; }
    int TotalCount { get; }
    void Clear();

}

public class PoolMgr : SingletonMonoBehaviour<PoolMgr>
{
    const float COLLECT_TIME = 3 * 60;
    float m_lastGCCollect;

    public string m_debugIdTypePool = "";

    SortedDictionary<string, IPool> m_pools = new SortedDictionary<string, IPool>();

    public SortedDictionary<string, IPool> Pools { get { return m_pools; } }

    public void AddPool(IPool pool)
    {
        if (m_pools.ContainsKey(pool.Name))
        {
            Debuger.LogError("重复注册对象池:{0}", pool.Name);
            return;
        }
        m_pools.Add(pool.Name, pool);
    }

    public void Clear()
    {
        GameObjectPool.GetPool(GameObjectPool.enPool.Fx).Clear();
        GameObjectPool.GetPool(GameObjectPool.enPool.Role).Clear();
        GameObjectPool.GetPool(GameObjectPool.enPool.Other).Clear();

        GCCollect();
    }

    public void GCCollect(bool immidiaty = true)
    {
        if (!immidiaty && Time.realtimeSinceStartup - m_lastGCCollect < COLLECT_TIME)
            return;

        System.GC.Collect();
        m_lastGCCollect = Time.realtimeSinceStartup;
    }
}
