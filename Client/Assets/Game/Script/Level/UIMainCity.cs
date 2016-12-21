#region Header
/**
 * 名称：UIMainCity类模板
 * 描述：
 **/
#endregion
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;



public class UIMainCity : UIPanel
{
    #region SerializeFields
    public UIMainCityItem[] m_items;
    public UIArtFont m_power;
    public Image m_exp;
    public StateHandle m_toggle;
    public GameObject m_toggleTip;
    public StateGroup roleInfoGroup;
    #endregion

    #region Fields
    static Dictionary<enSystem,Action> s_click = new Dictionary<enSystem,Action>();
    static Action s_onOpen;
    GameObject[] roleGo = new GameObject[3];  
    bool m_isTopBefore = false;
    Role m_role;
    int m_observer;
    int m_observer2;
    int m_observer3;
    int m_observer5;
    int m_observer6;
    Dictionary<enSystem, UIMainCityItem> m_itemsBySys = new Dictionary<enSystem, UIMainCityItem>();
    #endregion

    #region Properties
    #endregion

    #region Static Methods
    public static void AddClick(enSystem sys,Action a)
    {
        if(s_click.ContainsKey(sys))
        {
            Debuger.LogError("主城界面{0}系统按钮被重复监听，只能有一个监听者",sys);
            return;
        }
        s_click[sys]=a;
    }
    public static void AddOpen(Action a)
    {
        if(s_onOpen== null)
            s_onOpen = a;
        else
            s_onOpen+=a;
    }
    #endregion

    #region Frame
    //初始化时调用
    public override void OnInitPanel()
    {
        //建立索引，并设置点击回调     
        foreach(UIMainCityItem item in m_items){
            if(m_itemsBySys.ContainsKey(item.sys))
            {
                Debuger.LogError("主城界面有重复的系统图标，是不是复制黏贴新图标后没有修改系统枚举？{0}",item.sys);
                continue;
            }

            m_itemsBySys[item.sys] = item;

            item.btn.AddClickEx(OnClickItem);
            if (item.tip)
            {
                item.tip.SetActive(false);
            }
        }
        
    }


    //显示,保证在初始化之后
    public override void OnOpenPanel(object param)
    {
        m_isTopBefore = PanelBase.IsTop;

        this.GetComponent<RectTransform>().sizeDelta =Vector2.zero;
        
        if (s_onOpen!=null)
            s_onOpen();//打开主城界面的时候广播消息给外部
    }
  
    //关闭，保证在初始化之后
    public override void OnClosePanel()
    {
        //要缩回右边栏
        m_toggle.SetState(0);

        //界面关掉的时候要取消监听
        if (m_observer != EventMgr.Invalid_Id) { EventMgr.Remove(m_observer); m_observer = EventMgr.Invalid_Id; }
        if (m_observer2 != EventMgr.Invalid_Id) { EventMgr.Remove(m_observer2); m_observer2 = EventMgr.Invalid_Id; }
        if (m_observer3 != EventMgr.Invalid_Id) { EventMgr.Remove(m_observer3); m_observer3 = EventMgr.Invalid_Id; }
        if (m_observer5 != EventMgr.Invalid_Id) { EventMgr.Remove(m_observer5); m_observer5 = EventMgr.Invalid_Id; }

        CancelInvoke();
        m_role = null;
    }

    //更新，保证在初始化之后
    public override void OnUpdatePanel()
    {

        if (PanelBase.IsTop != m_isTopBefore)
        {
            m_isTopBefore = PanelBase.IsTop;
            if (m_isTopBefore)
            {
                EventMgr.FireAll(MSG.MSG_SYSTEM, MSG_SYSTEM.MAINCITY_UI_TOP);
                PanelBase.PlayAni("ui_ani_maincity_top", false);
            }
            else
            {
                EventMgr.FireAll(MSG.MSG_SYSTEM, MSG_SYSTEM.MAINCITY_UI_UNTOP);
                PanelBase.PlayAni("ui_ani_maincity_untop", false);
            }
        }
    }
    #endregion

    #region Private Methods
    void OnClickItem(StateHandle s)
    {
        UIMainCityItem item =s.Get<UIMainCityItem>();
        if(item == null)
        {
            Debuger.LogError("找不到UIMainCityItem");
            return;
        }

        Action a =s_click.Get(item.sys);
        if (a == null)
        {
            UIMessage.Show("该功能未实现，敬请期待!");
            return;
        }

        a();
    }

    #endregion

    public UIMainCityItem GetItem(enSystem sys)
    {
        return m_itemsBySys.Get(sys);
    }
}
