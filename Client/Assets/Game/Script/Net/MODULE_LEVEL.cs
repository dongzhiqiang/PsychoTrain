using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MODULE_LEVEL
{
    /** 进入关卡 */
    public const int CMD_ENTER = 1;
    /** 结束关卡 */
    public const int CMD_END = 2;
    /** 扫荡 */
    public const int CMD_SWEEP = 3;
    /** 评星奖励 */
    public const int CMD_STAR = 4;
}

public class RESULT_CODE_LEVEL : RESULT_CODE
{
    public const int LEVEL_NO_NUM = 1;          //今日次数已用完
    public const int LEVEL_NO_STAMINA = 2;      //今日体力已用完
    public const int LEVEL_NO_RECORD = 3;       //挑战结束的关卡服务端并没有记录
    public const int LEVEL_CANNOT_USE_AI = 4;   //不可挂机
    public const int LEVEL_NOT_EXSTIS = 5;      //关卡不存在
    public const int MUST_PASS_FIRST = 6;       //必须先通过此关
    public const int SWEEP_COND_NOT_MATCH = 7;  //不符合扫荡条件
    public const int LEVEL_STAR_CANT_GET = 8;   //不符合领取星级奖励条件
    public const int LEVEL_CANNT_AUTOBATTLE = 9;//不允许使用自动战斗
    public const int LEVEL_MUST_AUTOBATTLE = 10;  //只能使用自动战斗
}
