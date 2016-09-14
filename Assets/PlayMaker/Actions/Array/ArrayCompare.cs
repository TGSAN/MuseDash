// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.

using System.Linq;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if 2 Array Variables have the same values.")]
    public class ArrayCompare : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The first Array Variable to test.")]
        public FsmArray array1;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The second Array Variable to test.")]
        public FsmArray array2;

        [Tooltip("Event to send if the 2 arrays have the same values.")]
        public FsmEvent SequenceEqual;

        [Tooltip("Event to send if the 2 arrays have different values.")]
        public FsmEvent SequenceNotEqual;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Bool variable.")]
        public FsmBool storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            array1 = null;
            array2 = null;
            SequenceEqual = null;
            SequenceNotEqual = null;
        }

        public override void OnEnter()
        {
            DoSequenceEqual();
            
            if (!everyFrame)
            {
                Finish();
            }
        }

        private void DoSequenceEqual()
        {
            if (array1.Values == null || array2.Values == null) return;

            storeResult.Value = array1.Values.SequenceEqual(array2.Values);

            Fsm.Event(storeResult.Value ? SequenceEqual : SequenceNotEqual);
        }

    }

}

