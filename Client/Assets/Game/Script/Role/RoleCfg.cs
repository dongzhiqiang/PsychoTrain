using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum enRoleType
{
    min,
    hero,
    monster,
    boss,
    box,
    trap,
}

public enum enRolePropType
{
    role,
    monster,
}

public class RoleCfg
{

    public string id = "";
    public string name = "";
    public string mod = "";

    public enRoleType roleType = enRoleType.monster;

    public string icon; //头像图标

    public float power; //战斗力初始值

    public static string[] PropTypeName = new string[] { "角色属性", "怪物属性" };

    static Dictionary<string, enRolePropType> s_propTypes = new Dictionary<string, enRolePropType>();

    static string[] s_roleIds;
    static string[] s_roleNames;
    static Dictionary<string, RoleCfg> s_roleCfgs;
    static HashSet<string> s_preLoads = new HashSet<string>();
    static PropertyTable tem = new PropertyTable();
    static PropertyTable empty = new PropertyTable();

    public static string[] RoleIds
    {
        get
        {
            if (s_roleIds == null)
            {
                CheckInit();
                s_roleIds = new string[s_roleCfgs.Count];
                int i = 0;
                foreach (RoleCfg c in s_roleCfgs.Values)
                    s_roleIds[i++] = c.id;
                Array.Sort(s_roleIds);
            }
            return s_roleIds;
        }
    }

    public static string[] RoleNames
    {
        get
        {
            if (s_roleNames == null)
            {
                s_roleNames = new string[s_roleCfgs.Count];
                int i = 0;
                foreach (RoleCfg c in s_roleCfgs.Values)
                    s_roleNames[i++] = c.id;
                Array.Sort(s_roleNames);
            }
            return s_roleNames;
        }
    }

    public static void CheckInit()
    {
        if (s_roleCfgs == null)
            Init();
    }

    public static void Init()
    {
        s_roleCfgs = Csv.CsvUtil.Load<string, RoleCfg>("role", "id");
        s_roleIds = null;
        s_roleNames = null;
        s_preLoads.Clear();
        empty.IsRead = true;

        //建立角色属性类型的索引
        for (int i = 0; i < PropTypeName.Length; ++i)
            s_propTypes[PropTypeName[i]] = (enRolePropType)i;
    }

    //预加载
    public static void PreLoad(string roleId)
    {
        CheckInit();
        if (string.IsNullOrEmpty(roleId))
            return;

        if (s_preLoads.Contains(roleId))
            return;

        if (s_preLoads.Count == 0)
        {

        }

        s_preLoads.Add(roleId);
        RoleCfg cfg = Get(roleId);
        if (cfg == null)
            return;

        //预加载模型
        if (!string.IsNullOrEmpty(cfg.mod))
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).PreLoad(cfg.mod);
    }

    public static RoleCfg Get(string roleId)
    {
        CheckInit();
        RoleCfg cfg = s_roleCfgs.Get(roleId);
        if (cfg == null)
            Debuger.LogError("角色id不存在，请检查role表:{0}", roleId);
        return cfg;
    }

    public static Dictionary<string, RoleCfg> GetAll()
    {
        return s_roleCfgs;
    }

}
