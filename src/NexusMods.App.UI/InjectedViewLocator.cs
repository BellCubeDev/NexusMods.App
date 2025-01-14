using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace NexusMods.App.UI;

public class InjectedViewLocator : IViewLocator
{
    private readonly IServiceProvider _provider;
    private readonly MethodInfo _method;
    private readonly ILogger<InjectedViewLocator> _logger;

    public InjectedViewLocator(ILogger<InjectedViewLocator> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
        _method = GetType().GetMethod("ResolveViewInner", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        if (viewModel == null)
            return null;

        _logger.LogDebug("Finding View for {ViewModel}", viewModel.GetType().FullName);
        try
        {
            if (viewModel is IViewModel vm)
            {
                var intType = vm.ViewModelInterface;
                var method = _method.MakeGenericMethod(intType);
                var view = (IViewFor?)method.Invoke(this, Array.Empty<object>());
                if (view is IViewContract vc && contract is not null)
                {
                    vc.ViewContract = contract;
                }
                return view;
            }
            _logger.LogError("Failed to resolve view for {ViewModel}", typeof(T).FullName);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to resolve view for {ViewModel}", typeof(T).FullName);
            return null;
        }
    }

    /// <summary>
    /// This is a helper method used to simplify the casting involved in
    /// creating a view for a given view model. This is not dead code or typed
    /// incorrectly, it is used by the <see cref="ResolveView{T}"/> method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Local
    // ReSharper disable once ReturnTypeCanBeNotNullable
    private IViewFor? ResolveViewInner<T>() where T : class
    {
        return _provider.GetRequiredService<IViewFor<T>>();
    }
}
