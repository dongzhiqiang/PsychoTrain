/*
 * *********************************************************
 * ���ƣ����Ͷ����
 * ������ע����Ҫ����list��dict�����ͣ��Զ�������ͽ��黹�Ǽ̳���IdTypePool������ȫЩ
 */

using System;
using System.Collections.Generic;

public abstract class TypePool : IPool
{
    //����
    public string Name { get { return this.GetType().ToString(); } }

    //�ڳ��еĶ���
    public abstract int Count { get; }

    //������
    public abstract int TotalCount { get; }

    public TypePool()
    {
        if(PoolMgr.instance!=null)
            PoolMgr.instance.AddPool(this);
    }
    public abstract void Clear();
}

public class TypePool<T> : TypePool
    where T :  new()
{
    static TypePool<T> s_instance = null;
    static TypePool<T> Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new TypePool<T>();
            return s_instance;
        }
    }


    // Object pool to avoid allocations.
    const int Check_Leak_Count = 2000;//й¶��⣬������������ͱ���
    Stack<T> m_ListPool = new Stack<T>();
    int m_idCounter =0;

    //�ڳ��еĶ���
    public override int Count { get { return m_ListPool.Count; } }
    //������
    public override int TotalCount { get { return m_idCounter; } }
    T Get2()
    {
        T element;
        if (m_ListPool.Count == 0)
        {
            element = new T();
            ++m_idCounter;
            if (m_idCounter > Check_Leak_Count)//���й¶
                Debuger.LogError(string.Format("list�����({0})�����{1}���࣬�����ǲ���û��put", typeof(T).ToString(), m_idCounter));
        }
        else
            element = m_ListPool.Pop();
        return element;
    }

    //ע��Ż���֮ǰҪ�Լ������
    void Put2(T element)
    {
        m_ListPool.Push(element);
    }
    public override void Clear()
    {
        m_ListPool.Clear();
    }

    public static T Get()
    {
        return Instance.Get2();
    }

    public static void Put(T element)
    {
        Instance.Put2(element);
    }

}