using UnityEngine;

public interface IState
{
    string Name { get; }
    void Enter();
    void Exit();
    void Update();
    void FixedUpdate();
}
