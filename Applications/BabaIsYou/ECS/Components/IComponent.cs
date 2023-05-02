namespace BabaIsYou.ECS.Components;

internal interface IComponent<T> where T : new()
{
    public T Clone();
}
