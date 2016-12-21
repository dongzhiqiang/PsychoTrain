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

    public string propType = null;//角色属性计算方式，有角色、怪物、宠物
    public string propValue = null;//属性初始值，见属性表的固定属性表
    public string propDistribute = null;//属性分配比例，见属性表的属性分配比例
    
    public enRoleType roleType = enRoleType.monster;
    public int addBuffType = 0;     //是否是回血回蓝的宝箱

    public float behitRate = 1f;//僵直系数(越大僵直时间越长)
    public List<string> behitFxs = new List<string>();//被击特效id
    public List<int> bornBuffs = new List<int>();//出生状态
    public List<string> flags = new List<string>();//标记
    public string deadFx = "";//死亡特效，播放了死亡特效就不会播放死亡动作
    public string skillFile;//技能文件
    public string atkUpSkill = string.Empty;//普通攻击
    public List<string> skills = new List<string>();//技能

    public string icon; //头像图标

    public float power; //战斗力初始值

    public int behateIfBegin = 0;//创建的时候所有敌人对自己的仇恨
    public int hateIfHit = 0;//攻击别人增加自己对此人的仇恨
    public int hateIfBeHit = 0;//被别人攻击增加自己对此人的仇恨
    public int hateIfChange = 0;//仇恨切换时再多增加自己对此人的仇恨

    public string bornType = "";    //默认出生方式
    public string deadType = "";    //默认死亡方式
    public string aiType = "";  //默认ai
    public string titleBlood = string.Empty;//头顶血条类型
    public int headBloodNum = 10;//右上角boss头像血条数量，角色身上有GlobalConst.FLAG_SHOW_BLOOD标记的时候才会显示右上角血条
    public string colliderLayer = null;

    enBloodType _titleBloodType;
    enRolePropType _rolePropType;
    string _hitDefBloodIcon;
    List<enHitDef> _hitDefs;
    enGameLayer _colliderLayer;

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
            //加载一些常用特效
            GameObjectPool.GetPool(GameObjectPool.enPool.Fx).PreLoad("fx_humen_prj_root");

            //加载所有武器，一般只有主角有
            WeaponCfg.PreLoad();
        }

        s_preLoads.Add(roleId);
        RoleCfg cfg = Get(roleId);
        if (cfg == null)
            return;

        //预加载模型
        if (!string.IsNullOrEmpty(cfg.mod))
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).PreLoad(cfg.mod);

        //预加载被击特效、死亡特效
        for (int i = 0; i < cfg.behitFxs.Count; ++i)
            RoleFxCfg.ProLoad(cfg.behitFxs[i]);
        if (!string.IsNullOrEmpty(cfg.deadFx))
            RoleFxCfg.ProLoad(cfg.deadFx);

        //预加载出生状态
        for (int i = 0; i < cfg.bornBuffs.Count; ++i)
            BuffCfg.ProLoad(cfg.bornBuffs[i]);

        //预加载默认出生死亡特效
        BornCfg.PreLoad(cfg.bornType, cfg.deadType);

        //预加载技能
        RoleSkillCfg.PreLoad(cfg.skillFile);

        //预加载动作音效
        AniSoundCfg.PreLoad(cfg.mod);

        //预加载ai
        Simple.BehaviorTree.BehaviorTreeMgrCfg.PreLoad(cfg.aiType);
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

    public static string GetHeadIcon(string roleId, bool returnDef = true)
    {
        RoleCfg cfg = string.IsNullOrEmpty(roleId) ? null : Get(roleId);
        return (cfg != null && !string.IsNullOrEmpty(cfg.icon)) ? cfg.icon : (returnDef ? ConfigValue.defRoleHead : null);
    }

    public string GetSkillId(enSkillType skillType, bool isAir)
    {
        if (isAir)
            return null;

        switch (skillType)
        {
            case enSkillType.atkUp: return atkUpSkill;
            case enSkillType.skill1: return (skills.Count <= 0 ? null : skills[0]);
            case enSkillType.skill2: return (skills.Count <= 1 ? null : skills[1]);
            case enSkillType.skill3: return (skills.Count <= 2 ? null : skills[2]);
            case enSkillType.block: return null;
            default: Debuger.LogError("未知的类型{0}", skillType); return null;
        }
    }

    //打击属性
    public enHitDef GetHitDef(HitPropCfg cfg)
    {

        return _hitDefs[cfg.id - 1];
    }



    public void GetBaseProp(PropertyTable target, int lv = 1, int advLv = 1, int star = 1)
    {
#if PROP_DEBUG
        string log= "";
#endif
        if (RolePropType == enRolePropType.monster)//怪物成长属性=初始值+属性等级系数（怪物）*怪物属性分配比例*属性值系数（monster）
        {
            //属性等级系数（角色）*角色属性分配比例*属性值系数（role）
            PropertyTable propsDistribute = string.IsNullOrEmpty(this.propDistribute) ? empty : PropDistributeCfg.Get(this.propDistribute).props;
            PropertyTable.Mul(RoleTypePropCfg.mstTypeProp, propsDistribute, target);
            PropertyTable.Mul(MonsterLvPropCfg.Get(lv).props, target, target);
#if PROP_DEBUG
            log+=string.Format("属性等级系数（角色）*角色属性分配比例*属性值系数（role）={0}\n", target.GetFloat(enProp.hpMax));
#endif

            //加初始值
            PropertyTable propsValue = string.IsNullOrEmpty(this.propValue) ? empty : PropValueCfg.Get(this.propValue).props;
            PropertyTable.Add(target, propsValue, target);
#if PROP_DEBUG
            log += string.Format("+初始值={0}\n",target.GetFloat(enProp.hpMax));
#endif
        }
        else if (this.RolePropType == enRolePropType.role) //主角成长属性=初始值+属性等级系数（角色）*角色属性分配比例*属性值系数（role）
        {
            //属性等级系数（角色）*角色属性分配比例*属性值系数（role）
            PropertyTable propsDistribute = string.IsNullOrEmpty(this.propDistribute) ? empty : PropDistributeCfg.Get(this.propDistribute).props;
            PropertyTable.Mul(RoleTypePropCfg.roleTypeProp, propsDistribute, target);
#if PROP_DEBUG
            log += string.Format("属性等级系数（角色）*角色属性分配比例*属性值系数（role）={0}\n" , target.GetFloat(enProp.hpMax));
#endif
            PropertyTable.Mul(RoleLvPropCfg.Get(lv).rate, target, target);
#if PROP_DEBUG
            log += string.Format("*属性等级系数（角色）={0}\n", target.GetFloat(enProp.hpMax));
#endif

            //加初始值
            PropertyTable propsValue = string.IsNullOrEmpty(this.propValue) ? empty : PropValueCfg.Get(this.propValue).props;
            PropertyTable.Add(target, propsValue, target);
#if PROP_DEBUG
            log += string.Format("+初始值={0}\n",target.GetFloat(enProp.hpMax));
#endif

            //战斗力：（角色战斗力初值+角色属性等级系数）*战斗力系数
            target.SetInt(enProp.power, (int)((power + RoleLvPropCfg.Get(lv).rate) * PropBasicCfg.instance.powerRate));
#if PROP_DEBUG
            log += string.Format("战斗力={0}\n", target.GetFloat(enProp.power));
#endif
        }
        else//宠物成长属性 =宠物属性初值*(1+宠物进阶属性增量(初值)+宠物升星属性增量(初值)）+属性等级系数（角色）*宠物属性点数*宠物属性分配比例*属性值系数（role）)*(1+宠物进阶属性增量(等级)+宠物升星属性增量(等级)）
        {
            //PetAdvLvPropRateCfg advCfg = PetAdvLvPropRateCfg.Get(advLv);
            //PetStarPropRateCfg starCfg = PetStarPropRateCfg.Get(star);

            ////属性等级系数（角色）*宠物属性点数*(1+宠物进阶属性增量(等级)+宠物升星属性增量(等级)）
            //PropertyTable propsDistribute = string.IsNullOrEmpty(this.propDistribute) ? empty : PropDistributeCfg.Get(this.propDistribute).props;
            //float lvRate = RoleLvPropCfg.Get(lv).rate * PropBasicCfg.instance.petPoint * (1f + advCfg.lvRate + starCfg.lvRate);
            //PropertyTable.Mul(RoleTypePropCfg.roleTypeProp, propsDistribute, target);
            //PropertyTable.Mul(lvRate, target, target);
            //#if PROP_DEBUG
            //log += string.Format("属性等级系数（角色）*宠物属性点数*(1+宠物进阶属性增量(等级)+宠物升星属性增量(等级)）={0}\n", target.GetFloat(enProp.hpMax));
            //#endif

            ////加初始值
            //float baseRate = 1f + advCfg.baseRate + starCfg.baseRate;
            //#if PROP_DEBUG
            //log += string.Format("(1+宠物进阶属性增量(等级)+宠物升星属性增量(等级))={0} \n", baseRate);
            //#endif
            //PropertyTable propsValue = string.IsNullOrEmpty(this.propValue) ? empty : PropValueCfg.Get(this.propValue).props;
            //PropertyTable.Mul(baseRate, propsValue, tem);
            //PropertyTable.Add(tem, target, target);
            //#if PROP_DEBUG
            //log += string.Format("+初始值={0} \n", target.GetFloat(enProp.hpMax));
            //#endif

            ////战斗力：（角色战斗力初值*(1+宠物进阶属性增量（初值）+宠物升星属性增量（初值））+宠物属性点数*角色属性等级系数*（1+宠物进阶属性增量（等级）+宠物升星属性增量（等级）））*战斗力系数
            //float petPower = power * (1f + advCfg.baseRate + starCfg.baseRate);
            //petPower += PropBasicCfg.instance.petPoint*RoleLvPropCfg.Get(lv).rate*(1f + advCfg.lvRate + starCfg.lvRate);
            //petPower *= PropBasicCfg.instance.powerRate;
            //target.SetInt(enProp.power, (int)petPower);
#if PROP_DEBUG
            log += string.Format("战斗力={0}\n", target.GetFloat(enProp.power));
#endif
        }
#if PROP_DEBUG
        Debuger.Log(log);
#endif

    }
    
    public static void GetBasePropByCfg(string roleId, PropertyTable target, int lv = 1, int advLv = 1, int star = 1)
    {
        RoleCfg cfg = Get(roleId);
        if (cfg == null)
            return;

        cfg.GetBaseProp(target, lv, advLv, star);
    }
}
