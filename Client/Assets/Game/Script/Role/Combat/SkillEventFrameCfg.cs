using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


public enum enSkillEventTargetType
{
    selfAlway,      //释放者，总是执行到，除非角色不可战斗了
    self,           //释放者，受碰撞检测
    enemy,          //敌人阵营
    same,           //友方，同一阵营但是不是自己
    neutral,        //中立阵营
    selfSame,       //友方和自己
    target,         //当前技能的目标，使用技能的时候可能会指定目标，否则就会自动选择目标
    exceptSelf,       //除了自己
    max
}

public enum enSkillEventFrameType
{
    once,           //单帧
    multi,          //多帧
    //buff,           //缓冲后帧，也就是每一帧都判断，如果达到触发条件了，那么结束的时候执行
    max
}

//对象排序类型
public enum enTargetOrderType
{
    none,//默认，一般是按照创建顺序
    distance,//距离
}


public class TargetRangeCfg
{
    public RangeCfg range = new RangeCfg();//碰撞检测的范围
    public enSkillEventTargetType targetType = enSkillEventTargetType.selfAlway;//对象类型

    public void CopyFrom(TargetRangeCfg cfg)
    {
        if (cfg == null) return;

        //复制其他
        range.CopyFrom(cfg.range);

        //复制值类型的属性
        Util.Copy(cfg, this, BindingFlags.Public | BindingFlags.Instance);

    }
}
public class SkillEventFrameCfg
{

}
