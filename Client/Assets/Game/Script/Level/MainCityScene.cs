using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//主城场景
public class MainCityScene : LevelBase
{

    AniFxMgr heroAni;
    AniFxMgr shinv1Ani;
    AniFxMgr shinv2Ani;

    GameObject heroGo;
    GameObject pet1Go;
    GameObject pet2Go;




    static string Ani_zhuCheng_DaiJi_01 = "zhucheng_daiji01";
    static string Ani_zhuCheng_DaiJi_02 = "zhucheng_daiji02";
    static string Ani_zhuCheng_DaiJiGuoDu = "zhucheng_daijiguodu";
    static string Ani_zhuCheng_DaiJi = "zhuchengdaiji";

    static bool isFirst = true;

    //能不能加非战斗状态
    public override bool CanAddUnaliveBuff { get { return true; } }

    public override IEnumerator OnLoad()
    {
        Role hero = RoleMgr.instance.Hero;
        //加载主角模型
        GameObjectPool.GetPool(GameObjectPool.enPool.Role).Get(RoleMgr.instance.Hero.Cfg.mod, null, OnLoadHeroMod, false);

        yield return 0;
    }

    //场景切换完成
    public override void OnLoadFinish()
    {
        //打开主城界面        
        UIMgr.instance.Open<UIMainCity>();

        //相机切到主角身上
        GameObject heroPos = GameObject.Find("heroPos");
        CameraTriggerMgr caTriggerMgr = CameraTriggerMgr.instance;
        CameraTriggerGroup caTriggerGroup = caTriggerMgr.CurGroup;
        CameraMgr.instance.SetFollow(heroPos.transform);
        if (CameraTriggerMgr.instance != null)
            CameraTriggerMgr.instance.CurGroup.SetGroupActive(true);

        if (isFirst)
        {
            isFirst = false;
            CameraMgr.instance.Add(CameraTriggerMgr.instance.CurGroup.Triggers[0].m_info);
        }
        else
        {
            CameraMgr.instance.Set(CameraTriggerMgr.instance.CurGroup.Triggers[0].m_info);
        }



        //回城弹窗
        CheckOpen();


    }
    //创建全局敌人的时候，返回全局敌人的阵营，如果不希望创建可以返回enCamp.max
    public override enCamp OnCreateGlobalEnemy() { return enCamp.max; }


    public override void OnExit()
    {
        heroGo.transform.FindChild("model/weapon_mesh_01").gameObject.SetActive(true);
        heroGo.transform.FindChild("model/weapon_mesh").gameObject.SetActive(true);
        heroGo.transform.localScale = Vector3.one;
        if (heroGo != null)
        {
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).Put(heroGo);
            heroGo = null;
        }
        if (pet1Go != null)
        {
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).Put(pet1Go);
            pet1Go = null;
        }
        if (pet2Go != null)
        {
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).Put(pet2Go);
            pet2Go = null;
        }

    }

    void OnLoadHeroMod(GameObject modelObj, object obj)
    {
        heroGo = modelObj;
        GameObject heroPos = GameObject.Find("heroPos");
        modelObj.transform.position = heroPos.transform.position;
        modelObj.transform.rotation = heroPos.transform.rotation;
        modelObj.transform.localScale = heroPos.transform.localScale;

        modelObj.transform.FindChild("model/weapon_mesh_01").gameObject.SetActive(false);
        modelObj.transform.FindChild("model/weapon_mesh").gameObject.SetActive(false);


        heroAni = modelObj.transform.Find("model").GetComponent<AniFxMgr>();
        Room.instance.StartCoroutine(CoPlayHeroAni());
    }


    void refreshPetMod()
    {
        Role hero = RoleMgr.instance.Hero;
        if (pet1Go != null)
        {
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).Put(pet1Go);
            pet1Go = null;
        }
        if (pet2Go != null)
        {
            GameObjectPool.GetPool(GameObjectPool.enPool.Role).Put(pet2Go);
            pet2Go = null;
        }
    }

    IEnumerator CoPlayHeroAni()
    {
        if (heroAni == null) yield return 0;

        do
        {
            //播放待机动作1
            heroAni.Play(Ani_zhuCheng_DaiJi_01, WrapMode.Loop, 0, 1);
            yield return new WaitForSeconds(Random.Range(4, 7));

            //播放过度动作
            heroAni.Play(Ani_zhuCheng_DaiJiGuoDu, WrapMode.ClampForever, 0, 1);
            while (heroAni.CurSt != null && heroAni.CurSt.normalizedTime < 1f)
                yield return 0;

            //播放待机动作2
            heroAni.Play(Ani_zhuCheng_DaiJi_02, WrapMode.Loop);
            yield return new WaitForSeconds(Random.Range(4, 7));

            //播放过度动作
            heroAni.Play(Ani_zhuCheng_DaiJiGuoDu, WrapMode.PingPong, 0, 1);
            heroAni.CurSt.normalizedTime = 1;

            while (heroAni.CurSt != null && heroAni.CurSt.normalizedTime < 2)
            {
                yield return 0;
            }

        } while (true);
    }

    public override void OnUpdate()
    {
    }


    //检查一些回城弹窗
    void CheckOpen()
    {
        //TimeMgr.instance.AddTimer(1, CheckOpen2);
        CheckOpen2();
    }

    void CheckOpen2()
    {
    }
}
