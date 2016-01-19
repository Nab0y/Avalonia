﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Linq;
using Perspex.Controls;
using Perspex.Input;
using ReactiveUI;

namespace Perspex.Diagnostics.ViewModels
{
    internal class DevToolsViewModel : ReactiveObject
    {
        private IControl _root;

        private ReactiveObject _content;

        private int _selectedTab;

        private TreePageViewModel _logicalTree;

        private TreePageViewModel _visualTree;

        private readonly ObservableAsPropertyHelper<IInputElement> _focusedControl;

        private readonly ObservableAsPropertyHelper<IInputElement> _pointerOverElement;

        public DevToolsViewModel(IControl root)
        {
            _root = root;
            _logicalTree = new TreePageViewModel(LogicalTreeNode.Create(root));
            _visualTree = new TreePageViewModel(VisualTreeNode.Create(root));

            this.WhenAnyValue(x => x.SelectedTab).Subscribe(index =>
            {
                switch (index)
                {
                    case 0:
                        Content = _logicalTree;
                        break;
                    case 1:
                        Content = _visualTree;
                        break;
                }
            });

            _focusedControl = KeyboardDevice.Instance
                .WhenAnyValue(x => x.FocusedElement)
                .ToProperty(this, x => x.FocusedControl);

            //_pointerOverElement = this.WhenAnyValue(x => x.Root, x => x as TopLevel)
            //    .Select(x => x?.GetObservable(TopLevel.PointerOverElementProperty) ?? Observable.Empty<IInputElement>())
            //    .Switch()
            //    .ToProperty(this, x => x.PointerOverElement);
        }

        public ReactiveObject Content
        {
            get { return _content; }
            private set { this.RaiseAndSetIfChanged(ref _content, value); }
        }

        public int SelectedTab
        {
            get { return _selectedTab; }
            set { this.RaiseAndSetIfChanged(ref _selectedTab, value); }
        }

        public IInputElement FocusedControl => _focusedControl.Value;

        //public IInputElement PointerOverElement => _pointerOverElement.Value;
    }
}
