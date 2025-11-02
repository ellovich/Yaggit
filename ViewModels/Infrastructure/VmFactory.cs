namespace ViewModels.Infrastructure;

public interface IVmFactory
{
    T GetVm<T>(Action<T>? afterCreation = null) where T : VmBase;

    VmBase GetVm(Type type, Action<VmBase>? afterCreation = null);

    Task<T> GetVmAsync<T>(Func<T, Task>? afterCreation = null) where T : VmBase;

    Task<VmBase> GetVmAsync(Type type, Func<VmBase, Task>? afterCreation = null);
}

public class VmFactory(Func<Type, VmBase> factory) : IVmFactory
{
    private readonly Func<Type, VmBase> _factory = factory;

    public VmBase GetVm(Type type, Action<VmBase>? afterCreation = null)
    {
        VmBase vm = _factory(type);
        afterCreation?.Invoke(vm);
        return vm;
    }

    public T GetVm<T>(Action<T>? afterCreation = null) where T : VmBase
    {
        return (T)GetVm(typeof(T), vm => afterCreation?.Invoke((T)vm));
    }

    public async Task<VmBase> GetVmAsync(Type type, Func<VmBase, Task>? afterCreation = null)
    {
        VmBase vm = _factory(type);
        if (afterCreation != null)
            await afterCreation(vm);
        return vm;
    }

    public async Task<T> GetVmAsync<T>(Func<T, Task>? afterCreation = null) where T : VmBase
    {
        VmBase vm = await GetVmAsync(typeof(T), async vmBase =>
        {
            if (afterCreation != null)
                await afterCreation((T)vmBase);
        });
        return (T)vm;
    }
}