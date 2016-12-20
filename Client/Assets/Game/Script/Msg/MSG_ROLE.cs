using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSG_ROLE
{
    //创建， 模型还没创建 处于init状态
    public const int INIT = 1;

    //出生， 模型创建完 处于alive状态
    public const int BORN = 2;

    //死亡
    public const int DEAD = 3;

    //完全销毁
    public const int DESTROY = 4;

    //使用技能
    public const int SKILL = 5;

    //攻击
    public const int HIT = 6;

    //被击
    public const int BEHIT = 7;

    //本人主角创建完成
    public const int HERO_CREATED = 8;

    //模型被销毁
    public const int DESTROY_MODEL = 9;

    //属性变化
    public const int FRESH_BASE_PROP = 10;

    //标记有变化
    public const int FLAG_CHANGE = 11;

    //切换武器
    public const int WEAPON_CHANGE = 12;

    //武器显示状态改变，比如换武器或者技能需要隐藏武器
    public const int WEAPON_RENDER_CHANGE = 13;

    //杀死了一个敌人
    public const int KILL = 14;









    //属性变化，当要监听某个属性改变的时候应该监听Prop_CHANGE+该属性的索引
    public const int PROP_CHANGE = 1000;

    //活动属性变化，当要监听某个属性改变的时候应该监听ACT_PROP_CHANGE+该属性的索引
    public const int ACT_PROP_CHANGE = 2000;
}
