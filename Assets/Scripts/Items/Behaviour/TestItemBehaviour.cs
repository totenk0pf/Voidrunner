using Core.Logging;
using UnityEngine;

namespace Items.Behaviour {
    public class TestItemBehaviour : ItemBehaviour {
        public override void Execute(MonoBehaviour coroutineHandler) {
            NCLogger.Log($"Item activated!");
        }
    }
}