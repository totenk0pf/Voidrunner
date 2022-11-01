using UnityEngine;

namespace Items {
    public interface IItemBehaviour {
        void Execute(MonoBehaviour coroutineHandler);
    }
}