﻿#if NET472
/*
 * This file is part of alphaTab.
 * Copyright © 2018, Daniel Kuschny and Contributors, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or at your option any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AlphaTab.Collections;
using AlphaTab.Model;

namespace AlphaTab.Platform.CSharp.WinForms
{
    public sealed class AlphaTabControl : Panel
    {
        private IEnumerable<Track> _tracks;

        private AlphaTabLayoutPanel _layoutPanel;
        private Settings _settings;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<Track> Tracks
        {
            get { return _tracks; }
            set
            {
                if (_tracks == value) return;

                var observable = _tracks as INotifyCollectionChanged;
                if (observable != null)
                {
                    observable.CollectionChanged -= OnTracksChanged;
                }

                _tracks = value;

                observable = _tracks as INotifyCollectionChanged;
                if (observable != null)
                {
                    observable.CollectionChanged += OnTracksChanged;
                }
                RenderTracks();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Settings Settings
        {
            get => _settings;
            set
            {
                if (_settings == value) return;
                _settings = value;
                OnSettingsChanged(value);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlphaTabApi<AlphaTabControl> Api { get; private set; }

        public AlphaTabControl()
        {
            _layoutPanel = new AlphaTabLayoutPanel();
            AutoScroll = true;
            Controls.Add(_layoutPanel);

            Settings = Settings.Defaults;
            Settings.EnablePlayer = true;
            Settings.EnableCursor = true;

            Api = new AlphaTabApi<AlphaTabControl>(new WinFormsUiFacade(this, _layoutPanel), this);
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            if (_layoutPanel != null)
            {
                _layoutPanel.Location = new Point(Padding.Left, Padding.Top);
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control != _layoutPanel)
            {
                Controls.Remove(e.Control);
            }
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            if (_layoutPanel != null)
            {
                _layoutPanel.BackColor = ForeColor;
            }
        }

        private void OnTracksChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RenderTracks();
        }

        public void RenderTracks()
        {
            if (Tracks == null) return;

            Score score = null;
            var trackIndexes = new FastList<int>();
            foreach (var track in Tracks)
            {
                if (score == null)
                {
                    score = track.Score;
                }

                if (score == track.Score)
                {
                    trackIndexes.Add(track.Index);
                }
            }

            if (score != null)
            {
                Api.RenderTracks(score, trackIndexes.ToArray());
            }
        }

        public event Action<Settings> SettingsChanged;
        private void OnSettingsChanged(Settings obj)
        {
            SettingsChanged?.Invoke(obj);
        }
    }
}
#endif