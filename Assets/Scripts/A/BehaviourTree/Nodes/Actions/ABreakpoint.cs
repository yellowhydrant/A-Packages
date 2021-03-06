using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using A.BehaviourTree;

namespace A.BehaviourTree.Nodes.Standard
{
    public class ABreakpoint : AActionNode
    {
        protected override void OnStart()
        {
            Debug.Log("Triggering Breakpoint");
            Debug.Break();
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }
    }
}
