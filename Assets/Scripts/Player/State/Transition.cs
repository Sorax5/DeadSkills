using System;

public class Transition
{
    public string FromName;
    public string ToName;
    public Func<bool> Condition;
}
