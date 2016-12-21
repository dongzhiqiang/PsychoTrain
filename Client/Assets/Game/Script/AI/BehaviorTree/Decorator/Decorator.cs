#region Header
/**
 * 名称：Decorator
 * 描述：
 **/
#endregion
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Simple.BehaviorTree
{
    public class DecoratorCfg : ParentNodeCfg
    {
        //允许拥有的子节点数量
        public override int MaxChildren { get { return 1; } }
    }
    public class Decorator : ParentNode
    {




    }
}