using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleMgr : SingletonMonoBehaviour<RoleMgr>
{
    public class CloseComparer : IComparer<float>
    {
        public int Compare(float a, float b)
        {
            //这样保证了键值重复的情况下不会报错，如果有等于0的情况，那么就SortedList就不能add相同的键值
            return a <= b ? -1 : 1;
        }

        #region Fields

        Dictionary<int, Role> m_initRoles = new Dictionary<int, Role>();
        Dictionary<int, Role> m_roles = new Dictionary<int, Role>();
        Dictionary<int, Role> m_deadRoles = new Dictionary<int, Role>();

        //正在update中的角色要update完才能删除，记下来
        Role m_updatingDestroy;
        //正在update中的角色要update完才能死亡，记下来
        Role m_updatingDead;

        Role m_hero;

        #endregion

        #region Properties
        public Role Hero { get { return m_hero; } }
        //取得用角色管理器创建出来的所有角色
        public ICollection<Role> Roles { get { return m_roles.Values; } }

        #endregion

        public void CreateHero(enCamp camp = enCamp.camp1)
        {
            RoleCxt cxt = IdTypePool<RoleCxt>.Get();
            cxt.OnClear();
            cxt.roleId = "hero";
            cxt.pos = Vector3.zero;
            cxt.euler = Vector3.zero;
            cxt.camp = camp;
            cxt.aiBehavior = "";

            EventMgr.FireAll(MSG.MSG_ROLE, MSG_ROLE.HERO_CREATED);
        }

        public Role CreateRole(RoleCxt cxt, bool dontLoadModel = false)
        {
            if (cxt == null)
                return null;

            RoleCfg cfg = RoleCfg.Get(cxt.roleId);
            if (cfg == null)
            {
                Debuger.LogError("找不到角色id:{0}", cxt.Id);
                return null;
            }

            Role role = IdTypePool<Role>.Get();
            role.State = Role.enState.init;
            m_initRoles[role.Id] = role;
            if (!role.Init(Cfg))
            {

            }
        }
    }
}
