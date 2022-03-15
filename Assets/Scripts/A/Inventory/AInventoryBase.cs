using A.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Inventory
{
    public class AInventoryBase : ASavableObject
    {
        


        public override void FromJson(string json)
        {
            throw new System.NotImplementedException();
        }

        public override string ToJson()
        {
            throw new System.NotImplementedException();
        }

        public override void ResetValue()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AItemStack
    {

    }
}
