using UnityEngine;

namespace Items {
    public abstract class ItemBehaviour : IItemBehaviour {
        public abstract void Execute(MonoBehaviour coroutineHandler);
    }
}