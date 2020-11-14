using System.Collections.Generic;

namespace RFramework.Common.Pool
{
    public delegate void TimerDelegate(object userData);

    public class Timer
    {
        #region Inner Data Structure

        private class Task
        {
            public int TaskId;
            public TimerDelegate TimerDelegate;
            public object UserData;

            private bool _isFinish;

            public Task()
            {
                _isFinish = false;
            }

            public void Start()
            {
                OnStart();
            }

            public void Update(float deltaTime)
            {
                OnUpdate(deltaTime);
            }

            public virtual bool IsFinish()
            {
                return _isFinish;
            }

            protected virtual void OnStart()
            {
            }

            protected virtual void OnUpdate(float deltaTime)
            {
            }

            protected void Finish()
            {
                _isFinish = true;
            }

            protected void InvokeDelegate()
            {
                TimerDelegate?.Invoke(UserData);
            }
        }

        private class DelaySecondsTask : Task
        {
            public float DelaySeconds;

            private float _timer;

            protected override void OnStart()
            {
                _timer = 0f;
            }

            protected override void OnUpdate(float deltaTime)
            {
                _timer += deltaTime;

                if (_timer >= DelaySeconds)
                {
                    Finish();
                    InvokeDelegate();
                }
            }
        }

        private class DelayFramesTask : Task
        {
            public int DelayFrames;

            private int _timer;

            protected override void OnStart()
            {
                _timer = 0;
            }

            protected override void OnUpdate(float deltaTime)
            {
                _timer += 1;
                if (_timer >= DelayFrames)
                {
                    Finish();
                    InvokeDelegate();
                }
            }
        }

        private class EverySecondsTask : Task
        {
            public float IntervalTime;
            public int Count;

            private float _timer;

            protected override void OnStart()
            {
                _timer = 0f;
                Count = -1;
            }

            protected override void OnUpdate(float deltaTime)
            {
                _timer += deltaTime;

                while (_timer >= IntervalTime)
                {
                    _timer -= IntervalTime;
                    InvokeDelegate();

                    if (Count >= 0)
                    {
                        Count--;
                        if (Count == 0)
                            Finish();
                    }
                }
            }
        }

        private class EveryFramesTask : Task
        {
            public int IntervalFrame;
            public int Count;

            private int _timer;

            protected override void OnStart()
            {
                _timer = 0;
                Count = -1;
            }

            protected override void OnUpdate(float deltaTime)
            {
                _timer += 1;

                while (_timer >= IntervalFrame)
                {
                    _timer -= IntervalFrame;
                    InvokeDelegate();

                    if (Count >= 0)
                    {
                        Count--;
                        if (Count == 0)
                            Finish();
                    }
                }
            }
        }

        private class DuringSecondsTask : Task
        {
            public float DuringTime;

            private float _timer;

            protected override void OnStart()
            {
                _timer = 0;
            }

            protected override void OnUpdate(float deltaTime)
            {
                _timer += deltaTime;
                if (_timer >= DuringTime)
                    Finish();
                else
                    InvokeDelegate();
            }
        }

        private class DuringFramesTask : Task
        {
            public int DuringFrames;

            private int _timer;

            protected override void OnStart()
            {
                _timer = 0;
            }

            protected override void OnUpdate(float deltaTime)
            {
                _timer += 1;
                if (_timer >= DuringFrames)
                    Finish();
                else
                    InvokeDelegate();
            }
        }

        #endregion

        private int _taskId;
        private int TaskId => _taskId++;

        private readonly List<Task> _workingTasks;
        private readonly List<Task> _waitingToAddTasks;

        private int _nullCount = 0;

        public Timer()
        {
            _taskId = 0;

            _workingTasks = new List<Task>();
            _waitingToAddTasks = new List<Task>();
        }

        public void Update(float deltaTime)
        {
            if (_waitingToAddTasks.Count > 0)
            {
                _workingTasks.AddRange(_waitingToAddTasks);
                _waitingToAddTasks.Clear();
            }

            for (var i = 0; i < _workingTasks.Count; i++)
            {
                var task = _workingTasks[i];
                if (task != null)
                {
                    task.Update(deltaTime);

                    if (task.IsFinish())
                    {
                        _workingTasks[i] = null;
                        _nullCount++;
                    }
                }
            }

            if (_nullCount > _workingTasks.Count / 3)
                ClearNullTask();
        }

        public int After(float seconds, TimerDelegate timerDelegate, object userData = null)
        {
            var task = CreateTask<DelaySecondsTask>(timerDelegate, userData);
            task.DelaySeconds = seconds;
            return task.TaskId;
        }

        public int AfterFrames(int frames, TimerDelegate timerDelegate, object userData = null)
        {
            var task = CreateTask<DelayFramesTask>(timerDelegate, userData);
            task.DelayFrames = frames;
            return task.TaskId;
        }

        public int Every(float seconds, TimerDelegate timerDelegate, object userData = null, int count = -1)
        {
            var task = CreateTask<EverySecondsTask>(timerDelegate, userData);
            task.IntervalTime = seconds;
            task.Count = count;
            return task.TaskId;
        }

        public int EveryFrames(int frames, TimerDelegate timerDelegate, object userData = null, int count = -1)
        {
            var task = CreateTask<EveryFramesTask>(timerDelegate, userData);
            task.IntervalFrame = frames;
            task.Count = count;
            return task.TaskId;
        }

        public int During(float seconds, TimerDelegate timerDelegate, object userData = null)
        {
            var task = CreateTask<DuringSecondsTask>(timerDelegate, userData);
            task.DuringTime = seconds;
            return task.TaskId;
        }

        public int DuringFrames(int frames, TimerDelegate timerDelegate, object userData = null)
        {
            var task = CreateTask<DuringFramesTask>(timerDelegate, userData);
            task.DuringFrames = frames;
            return task.TaskId;
        }

        public void Cancel(int taskId)
        {
            for (var i = 0; i < _waitingToAddTasks.Count; i++)
            {
                if (_waitingToAddTasks[i].TaskId == taskId)
                {
                    _waitingToAddTasks.RemoveAt(i);
                    return;
                }
            }

            for (var i = 0; i < _workingTasks.Count; i++)
            {
                if (_workingTasks[i].TaskId == taskId)
                {
                    _workingTasks[i] = null;
                    _nullCount++;
                    return;
                }
            }
        }

        public void CancelAll()
        {
            _waitingToAddTasks.Clear();

            for (var i = 0; i < _workingTasks.Count; i++)
            {
                _workingTasks[i] = null;
                _nullCount++;
            }
        }

        private T CreateTask<T>(TimerDelegate timerDelegate, object userData) where T : Task, new()
        {
            var task = new T {TaskId = TaskId, TimerDelegate = timerDelegate, UserData = userData};
            _waitingToAddTasks.Add(task);
            return task;
        }

        private void ClearNullTask()
        {
            for (var i = _workingTasks.Count - 1; i >= 0; i--)
            {
                if (_workingTasks[i] == null)
                    _workingTasks.RemoveAt(i);
            }

            _nullCount = 0;
        }
    }
}