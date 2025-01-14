using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Media.Imaging;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusMods.App.UI.Controls.Spine.Buttons.Icon;
using NexusMods.App.UI.Controls.Spine.Buttons.Image;
using NexusMods.App.UI.Extensions;
using NexusMods.App.UI.LeftMenu;
using NexusMods.App.UI.LeftMenu.Game;
using NexusMods.App.UI.LeftMenu.Home;
using NexusMods.App.UI.Routing;
using NexusMods.App.UI.Routing.Messages;
using NexusMods.DataModel.Abstractions;
using NexusMods.DataModel.Abstractions.Ids;
using NexusMods.DataModel.Games;
using NexusMods.DataModel.Loadouts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NexusMods.App.UI.Controls.Spine;

public class SpineViewModel : AViewModel<ISpineViewModel>, ISpineViewModel
{
    public IIconButtonViewModel Home { get; }

    public IIconButtonViewModel Add { get; }

    private ReadOnlyObservableCollection<IImageButtonViewModel> _games =
        Initializers.ReadOnlyObservableCollection<IImageButtonViewModel>();
    public ReadOnlyObservableCollection<IImageButtonViewModel> Games => _games;

    public Subject<SpineButtonAction> Activations { get; } = new();

    [Reactive]
    public ILeftMenuViewModel LeftMenu { get; set; } =
        Initializers.ILeftMenuViewModel;

    private readonly Subject<SpineButtonAction> _actions = new();
    private readonly ILogger<SpineViewModel> _logger;
    private readonly IGameLeftMenuViewModel _gameLeftMenuViewModel;
    public IObservable<SpineButtonAction> Actions => _actions;

    public SpineViewModel(ILogger<SpineViewModel> logger, IDataStore dataStore,
        IIconButtonViewModel addButtonViewModel,
        IIconButtonViewModel homeButtonViewModel,
        IHomeLeftMenuViewModel homeLeftMenuViewModel,
        IGameLeftMenuViewModel gameLeftMenuViewModel,
        IRouter router,
        IServiceProvider provider)
    {
        _logger = logger;

        Home = homeButtonViewModel;
        Add = addButtonViewModel;

        _gameLeftMenuViewModel = gameLeftMenuViewModel;


        this.WhenActivated(disposables =>
        {

            router.Messages
                .Subscribe(HandleMessage)
                .DisposeWith(disposables);

            dataStore.ObservableManagedGames()
                .Transform(game =>
                {
                    using var iconStream = game.Icon.GetStreamAsync().Result;
                    var vm = provider.GetRequiredService<IImageButtonViewModel>();
                    vm.Name = game.Name;
                    vm.Image = Bitmap.DecodeToWidth(iconStream, 48);
                    vm.IsActive = false;
                    vm.Tag = game;
                    vm.Click = ReactiveCommand.Create(() => NavigateToGame(game));
                    return vm;
                })
                .OnUI()
                .Bind(out _games)
                .Subscribe()
                .DisposeWith(disposables);

            Home.Click = ReactiveCommand.Create(() =>
            {
                _logger.LogTrace("Home selected");
                _actions.OnNext(new SpineButtonAction(Type.Home));
                LeftMenu = homeLeftMenuViewModel;
            });

            Add.Click = ReactiveCommand.Create(() =>
            {
                _logger.LogTrace("Add selected");
                _actions.OnNext(new SpineButtonAction(Type.Add));
            });

            Activations.Subscribe(HandleActivation)
                .DisposeWith(disposables);
        });
    }

    private void NavigateToGame(IGame game)
    {
        _logger.LogTrace("Game {Game} selected", game);
        _actions.OnNext(new SpineButtonAction(Type.Game, game));
        _gameLeftMenuViewModel.Game = game;
        LeftMenu = _gameLeftMenuViewModel;
    }

    private void HandleMessage(IRoutingMessage message)
    {
        if (message is NavigateToLoadout navigateToLoadout)
        {
            NavigateToGame(navigateToLoadout.Game);
        }
    }

    private void HandleActivation(SpineButtonAction action)
    {
        _logger.LogTrace("Activation {Action}", action);
        if (action.Type == Type.Game)
        {
            Home.IsActive = false;
            Add.IsActive = false;
            foreach (var game in Games)
            {
                game.IsActive = ReferenceEquals(game.Tag, action.Game);
            }
        }
        else
        {
            Home.IsActive = action.Type == Type.Home;
            Add.IsActive = action.Type == Type.Add;
            foreach (var game in Games)
            {
                game.IsActive = false;
            }
        }
    }
}
