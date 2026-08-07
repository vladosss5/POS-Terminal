using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Terminal.Core.Enums;

namespace Terminal.ViewModels.Items;

/// <summary>
/// Модель шага продажи.
/// </summary>
/// <param name="step">Тип шага.</param>
/// <param name="name">Название шага.</param>
public partial class SellingStepViewModel(SaleProcessStep step, string name) : ObservableObject
{
    /// <summary>
    /// Тип.
    /// </summary>
    public SaleProcessStep Step { get; init; } = step;

    /// <summary>
    /// Наименование.
    /// </summary>
    public string Name { get; init; } = name;

    /// <summary>
    /// Активен ли.
    /// </summary>
    [ObservableProperty] 
    public partial bool IsActive { get; set; }
    
    /// <summary>
    /// Выполнен ли.
    /// </summary>
    [ObservableProperty] 
    public partial bool IsCompleted { get; set; }

    /// <summary>
    /// Метод активации шага.
    /// </summary>
    /// <param name="allSteps">Коллекция всех шагов.</param>
    public void Activate(IEnumerable<SellingStepViewModel> allSteps)
    {
        foreach (var s in allSteps)
            s.IsActive = false;

        IsActive = true;
    }
}