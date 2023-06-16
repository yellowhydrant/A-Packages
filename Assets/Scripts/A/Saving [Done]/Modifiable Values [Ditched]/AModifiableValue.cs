using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Modifiable Value")]
    public class AModifiableValue : ScriptableObject// //ASavableSimpleValue<float>
    {
        public List<AModifier> modifiers;

        public float value
        {
            get { return GetModifiedValue(); }
            set { originalValue = value; }
        }
        float originalValue;

        public float GetModifiedValue()
        {
            modifiers.OrderBy(x => x.priority);

            var val = originalValue;
            for (int i = 0; i < modifiers.Count; i++)
            {
                val = modifiers[i].ModifyValue(val);
            }
            return val;
        }

    }
}
