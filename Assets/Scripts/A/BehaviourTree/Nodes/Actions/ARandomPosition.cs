using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using A.BehaviourTree;

namespace A.BehaviourTree.Nodes.Standard
{
    public class ARandomPosition : AActionNode
    {
        public Vector2 min = Vector2.one * -10;
        public Vector2 max = Vector2.one * 10;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            blackboard.moveToPosition.x = Random.Range(min.x, max.x);
            blackboard.moveToPosition.z = Random.Range(min.y, max.y);
            return State.Success;
        }
    }
}
