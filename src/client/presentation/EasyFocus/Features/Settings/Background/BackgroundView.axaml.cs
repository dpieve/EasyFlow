using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace EasyFocus.Features.Settings.Background;

public partial class BackgroundView : ReactiveUserControl<BackgroundViewModel>
{
    public BackgroundView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel is null)
            {
                return;
            }

            this.WhenAnyValue(v => v.ViewModel!.SelectedBackground)
                .DistinctUntilChanged()
                .Subscribe(b =>
                {
                    if (b.Contains("1"))
                    {
                        Background1Button.IsChecked = true;
                        Background2Button.IsChecked = false;
                        Background3Button.IsChecked = false;
                    }
                    else if (b.Contains("2"))
                    {
                        Background1Button.IsChecked = false;
                        Background2Button.IsChecked = true;
                        Background3Button.IsChecked = false;
                    }
                    else if (b.Contains("3"))
                    {
                        Background1Button.IsChecked = false;
                        Background2Button.IsChecked = false;
                        Background3Button.IsChecked = true;
                    }
                    else
                    {
                        Background1Button.IsChecked = false;
                        Background2Button.IsChecked = false;
                        Background3Button.IsChecked = false;
                    }
                })
                .DisposeWith(d);
        });
    }
}