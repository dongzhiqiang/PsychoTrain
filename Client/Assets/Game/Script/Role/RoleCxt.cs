using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoleCxt : IdType
{
    public string guid;
    public string name = "";
    public string roleId;
    public int level;
    public enCamp camp = enCamp.camp1;
    public Vector3 pos;
    public Vector3 euler;
    public Action<Role> onCreate = null;
    public string bornAniId = "";
    public string deadAniId = "";
    public string aiBehavior = "";
    public void Init(string guid, string name, string roleId, int level, enCamp camp, Vector3 pos, Vector3 euler, string bornAniId = "", string deadAniId = "", string groundDeadAniId = "", string aiBehavior = "", Action<Role> onCreate = null)
    {
        this.guid = guid;
        this.name = name;
        this.roleId = roleId;
        this.level = level;
        this.camp = camp;
        this.pos = pos;
        this.euler = euler;
        this.onCreate = onCreate;
        this.bornAniId = bornAniId;
        this.deadAniId = deadAniId;
        this.aiBehavior = aiBehavior;

    }

    public void CopyFrom(RoleCxt cxt)
    {
        OnClear();
        this.guid = cxt.guid;
        this.name = cxt.name;
        this.roleId = cxt.roleId;
        this.level = cxt.level;
        this.camp = cxt.camp;
        this.onCreate = cxt.onCreate;
        this.bornAniId = cxt.bornAniId;
        this.deadAniId = cxt.deadAniId;
        this.aiBehavior = cxt.aiBehavior;
    }

    public override void OnClear()
    {
        guid = Util.GenerateGUID();
        name = "";
        roleId = "";
        level = 1;
        camp = enCamp.camp1;
        onCreate = null;
        bornAniId = "";
        deadAniId = "";
        aiBehavior = "";
    }

}
