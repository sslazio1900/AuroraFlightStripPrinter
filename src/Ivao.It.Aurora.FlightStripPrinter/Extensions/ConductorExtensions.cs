using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ivao.It.Aurora.FlightStripPrinter.Extensions;

public static class ConductorExtensions
{
    public static async Task ShowDialogAsync<TViewModel>(
        this Conductor<object> cond,
        object? context = null,
        IDictionary<string, object>? settings = null,
        CancellationToken cancellationToken = default)
    {
        var vmToActivate = IoC.Get<TViewModel>();
        var viewdowManager = IoC.Get<IWindowManager>();

        if (vmToActivate is null) throw new NullReferenceException(nameof(vmToActivate));
        if (viewdowManager is null) throw new NullReferenceException(nameof(viewdowManager));

        await cond.ActivateItemAsync(vmToActivate, cancellationToken);
        await viewdowManager.ShowDialogAsync(vmToActivate, context, settings);
        await cond.DeactivateItemAsync(vmToActivate, true, cancellationToken);
    }
}