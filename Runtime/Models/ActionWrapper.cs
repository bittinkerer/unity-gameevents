
using System;
using System.Collections.Generic;

public sealed class ActionWrapper<T1,T2>
    : IEquatable<ActionWrapper<T1,T2>>, IEqualityComparer<ActionWrapper<T1,T2>>
{
    public Guid Guid { get; private set; }
    public Action<T1,T2> _Action { get; set; }

    public ActionWrapper(Guid id, Action<T1,T2> action)
    {
        Guid = id;
        _Action = action;
    }

    public ActionWrapper(Action<T1,T2> action) : this(default, action) { }

    public static implicit operator Action<T1, T2>(ActionWrapper<T1, T2> action) =>
        action._Action;

    public static implicit operator ActionWrapper<T1, T2>(Action<T1,T2> action) =>
        new(default, action);

    public bool Equals(ActionWrapper<T1, T2> other) =>
        Guid != default
        ? Guid == other.Guid
        : _Action == other._Action;

    public static bool operator ==(ActionWrapper<T1, T2> left, ActionWrapper<T1,T2> right) =>
        ((IEquatable<ActionWrapper<T1,T2>>) left).Equals(right);

    public static bool operator !=(ActionWrapper<T1, T2> left, ActionWrapper<T1, T2> right) =>
        !(left == right);

    public bool Equals(ActionWrapper<T1, T2> x, ActionWrapper<T1, T2> y) =>
        x == y;

    public override int GetHashCode() =>
        Guid != default ? Guid.GetHashCode() : _Action.GetHashCode();

    public int GetHashCode(ActionWrapper<T1, T2> obj) =>
        ((ActionWrapper<T1,T2>)obj).GetHashCode();

    public override bool Equals(object obj) =>
        this == (ActionWrapper<T1,T2>)obj;

    public override string ToString() =>
        $"Guid: {this.Guid}, Action: {this._Action}";
}