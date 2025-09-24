using UnityEngine;
namespace GDEUtils.StateMachine
{
    public class State<T> : MonoBehaviour
    {
        protected T owner;

        public virtual void Enter(T owner)
        {
            this.owner = owner;
        }

        public virtual void Execute() { }
        public virtual void Exit() { }
    }
}
