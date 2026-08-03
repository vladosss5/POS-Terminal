using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.Core.Enums;

namespace Terminal.ViewModels.Items;

public partial class SellingStepViewModel(SaleProcessStep step, string name) : ObservableObject
{
    public SaleProcessStep Step { get; init; } = step;

    public string Name { get; init; } = name;

    [ObservableProperty] 
    public partial bool IsActive { get; set; }
    
    [ObservableProperty] 
    public partial bool IsCompleted { get; set; }

    public void Activate(IEnumerable<SellingStepViewModel> allSteps)
    {
        foreach (var s in allSteps)
            s.IsActive = false;

        IsActive = true;
    }
}