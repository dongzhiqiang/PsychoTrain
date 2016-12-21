﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Simple.BehaviorTree
{
    public class SelectorCfg : ParentNodeCfg
    {
        
    }

    public class Selector : Composite
    {
        
        enNodeState m_lastNodeState = enNodeState.failure;

        SelectorCfg CfgEx { get { return (SelectorCfg)m_cfg; } }

        //入栈。行为树遍历过程中，遍历到一个节点就会入栈它。可以用做是当前次遍历的OnInit
        protected override void OnPush() {
            
            m_lastNodeState = enNodeState.failure;
        }

        //获取下个要执行的子节点,没有返回-1
        public override int OnGetNextChildIdx(int counter)
        {
            if (m_lastNodeState != enNodeState.failure)
                return -1;
            

            return base.OnGetNextChildIdx(counter);
        }

        //字节点执行完,告知父节点
        public override void OnChildPop(int childIdx, enNodeState childState) {
            if(childState== enNodeState.inactive || childState == enNodeState.running)
            {
                LogError("Selector OnChildPop只能接收成功或者失败:{0},子节点:{1}", childState, childIdx);
                return;
            }

            m_lastNodeState = childState;
        }
        

        //执行。遍历到这个节点的时候就会在OnPush()后执行，如果返回running的话就会一直执行，直到返回success或者fail，然后OnPop()
        protected override enNodeState OnExecute(enExecute executeType) { return m_lastNodeState; }
        
    }
}