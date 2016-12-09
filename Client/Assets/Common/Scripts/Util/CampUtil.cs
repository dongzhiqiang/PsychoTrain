using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum enCamp
{
    camp1,//主角
    camp2,//副本中的敌人
    camp3,
    camp4,
    camp5,
    camp6,
    camp7,
    camp8,
    neutral,//中立
    max
}

public class CampUtil
{
    public static bool Match(enCamp a, enCamp b)
    {
        return a == b;
    }

}
