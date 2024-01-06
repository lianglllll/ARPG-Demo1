using UnityEngine;

// ReSharper disable once CheckNamespace
namespace QFX.IFX
{
    public class IFX_ControlledObject : MonoBehaviour
    {
        public bool RunAtStart;
        public float RunDelay;
        public bool SetupAtStart = true;

        public bool IsRunning { get; private set; }

        private void OnEnable()
        {
            if (SetupAtStart)
                Setup();

            if (RunAtStart)
                IFX_InvokeUtil.RunLater(this, Run, RunDelay);
        }

        public virtual void Setup()
        {
        }

        public virtual void Run()
        {
            IsRunning = true;
        }

        public virtual void Stop()
        {
            IsRunning = false;
        }

        public void RunWithDelay()
        {
            IFX_InvokeUtil.RunLater(this, Run, RunDelay);
        }
    }
}