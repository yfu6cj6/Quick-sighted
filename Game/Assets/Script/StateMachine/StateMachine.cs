using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : Enum
{
    private readonly Dictionary<T, IState> _stateClassDic = new Dictionary<T, IState>();
    private readonly Dictionary<T, Action> _stateActionDic = new Dictionary<T, Action>();

    public T CurStete { get; private set; }

    public void RegisterStateByClass(T state, IState stateClass)
    {
        if (!_stateClassDic.ContainsKey(state))
        {
            _stateClassDic.Add(state, stateClass);
        }
    }

    public void UnregisterStateByClass(T state)
    {
        if (_stateClassDic.ContainsKey(state))
        {
            _stateClassDic[state] = null;
            _stateClassDic.Remove(state);
        }
    }

    public bool ChangeStateByClass(T toState)
    {
        if (!_stateClassDic.ContainsKey(toState))
        {
            Debug.LogError($"要切換的狀態未被註冊過 {toState}");
            return false;
        }

        if (CurStete != null && _stateClassDic.ContainsKey(CurStete))
        {
            _stateClassDic[CurStete].OutState();
        }

        CurStete = toState;
        _stateClassDic[CurStete].InState();
        return true;
    }

    public void RegisterStateByAction(T state, Action stateAction)
    {
        if (!_stateActionDic.ContainsKey(state))
        {
            _stateActionDic.Add(state, stateAction);
        }
    }

    public void UnregisterStateByAction(T state)
    {
        if (_stateActionDic.ContainsKey(state))
        {
            _stateActionDic[state] = null;
            _stateActionDic.Remove(state);
        }
    }

    public bool ChangeStateByAction(T toState)
    {
        if (!_stateActionDic.ContainsKey(toState))
        {
            Debug.LogError($"要切換的狀態未被註冊過 {toState}");
            return false;
        }

        if (CurStete != null && _stateActionDic.ContainsKey(CurStete))
        {
            _stateActionDic[CurStete].Invoke();
        }

        CurStete = toState;
        _stateActionDic[CurStete].Invoke();
        return true;
    }
}
