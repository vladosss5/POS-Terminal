using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Terminal.ViewModels.Pages;

namespace Terminal.Views.Pages;

public partial class PrintingReceiptPageView : UserControl
{
    public PrintingReceiptPageView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Обработчик прокрутки списка чеков. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        var scrollViewer = sender as ScrollViewer;
        var viewModel = DataContext as PrintingReceiptPageViewModel;

        if (scrollViewer == null || viewModel == null || viewModel.IsLoading)
            return;
        
        var scrollableHeight = scrollViewer.Extent.Height - scrollViewer.Viewport.Height;
        var currentScrollOffset = scrollViewer.Offset.Y;
        
        if (scrollableHeight > 0 && 
            currentScrollOffset >= scrollableHeight * 0.7 && 
            viewModel.HasMoreItems)
        {
            await viewModel.LoadMoreReceiptsAsync();
        }
    }
}