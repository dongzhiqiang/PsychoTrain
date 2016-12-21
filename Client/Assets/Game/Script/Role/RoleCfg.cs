using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum enBloodType
{
    none,
    small,
    big,
    building,
    npc,

}

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
    public int addBuffType = 0;     //是否是回血回蓝的宝箱

    public List<string> flags = new List<string>();//标记
    public string deadFx = "";//死亡特效，播放了死亡特效就不会播放死亡动作
    public string skillFile;//技能文件

    public string icon; //头像图标

    public float power; //战斗力初始值


    public string bornType = "";    //默认出生方式
    public string deadType = "";    //默认死亡方式
    public string aiType = "";  //默认ai
    public string titleBlood = string.Empty;//头顶血条类型
    public int headBloodNum = 10;//右上角boss头像血条数量，角色身上有GlobalConst.FLAG_SHOW_BLOOD标记的时候才会显示右上角血条
    public string colliderLayer = null;

    enBloodType _titleBloodType;
    enRolePropType _rolePropType;
    enGameLayer _colliderLayer;
    string _hitDefBloodIcon;

    public static string[] PropTypeName = new string[] { "角色属性", "怪物属性" };

    static Dictionary<string, enRolePropType> s_propTypes = new Dictionary<string, enRolePropType>();

    static string[] s_roleIds;
    static string[] s_roleNames;
    static Dictionary<string, RoleCfg> s_roleCfgs;
    static HashSet<string> s_preLoads = new HashSet<string>();
    static PropertyTable tem = new PropertyTable();
    static PropertyTable empty = new PropertyTable();

    public enBloodType TitleBloodType { get { return _titleBloodType; } }
    public enRolePropType RolePropType { get { return _rolePropType; } }
    public enGameLayer ColliderLayer { get { return _colliderLayer; } }
    public string HitDefBloodIcon { get { return _hitDefBloodIcon; } }

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
