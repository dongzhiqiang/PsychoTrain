using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Csv;

public class AsyncOpInitCfg
{
    bool _isDone = false;
    float _progress = 0;
    List<Action> _ops;
    public bool isDone { get { return _isDone; } }

    public float progress { get { return _progress; } }

    public AsyncOpInitCfg(List<Action> ops)
    {
        _ops = ops;

        Main.instance.StartCoroutine(CoInit());
    }

    IEnumerator CoInit()
    {
        yield return 2;
        int totalCount = _ops.Count;
        long lastTick = System.DateTime.Now.Ticks;
        for (int i = 0; i < _ops.Count; ++i)
        {
            _ops[i]();
            _progress = i / (float)_ops.Count;

            long curTick = System.DateTime.Now.Ticks;
            //耗时超过50ms下一帧
            if (curTick - lastTick > System.TimeSpan.TicksPerMillisecond * 50)
            {
                //Debuger.LogError("配置表加载到:{0}",i);
                lastTick = curTick;
                yield return 0;
            }

        }
        //结束清空操作
        CsvUtil.Clear();
        CsvReader.Clear();
        PoolMgr.instance.GCCollect();//垃圾回收下，解析表可能会有大量的垃圾
        _progress = 1f;
        _isDone = true;
    }
}

public class CfgMgr : Singleton<CfgMgr>
{

    public AsyncOpInitCfg Init()
    {
        List<Action> ops = new List<Action>();
        //配置表初始化
        ops.Add(ConfigValue.Init);
        ops.Add(LanguageCfg.Init);
        ops.Add(SoundCfg.Init);
        ops.Add(SkillLvRateCfg.Init);
        ops.Add(SkillLvValueCfg.Init);
        ops.Add(HitPropCfg.Init);
        ops.Add(RoleCfg.Init);
        ops.Add(RoleFxCfg.Init);
        ops.Add(RoleLvExpCfg.Init);
        ops.Add(WeaponCfg.Init);
        ops.Add(PropTypeCfg.Init);//注意要在涉及属性的表初始化之前解析
        ops.Add(PropBasicCfg.Init);
        ops.Add(PropValueCfg.Init);
        ops.Add(PropRateCfg.Init);
        ops.Add(PropDistributeCfg.Init);
        ops.Add(RoleLvPropCfg.Init);
        ops.Add(MonsterLvPropCfg.Init);
        ops.Add(RoleTypePropCfg.Init);
        ops.Add(BuffType.Init);
        ops.Add(BuffCfg.Init);
        ops.Add(QTECfg.Init);
        ops.Add(RoomCfg.Init);
        ops.Add(BornCfg.Init);
        ops.Add(SystemSkillCfg.Init);
        ops.Add(LoadingTipsCfg.Init);
        ops.Add(AniSoundCfg.Init);
        ops.Add(FxSoundCfg.Init);

        return new AsyncOpInitCfg(ops);
    }



}
