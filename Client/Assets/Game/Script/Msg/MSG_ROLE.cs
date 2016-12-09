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
}
