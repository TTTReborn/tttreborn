// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;
using System.Collections.Generic;

namespace TTTReborn.UI
{
    public struct PanelContentData
    {
        public Action<PanelContent> Content;
        public string Title;
        public string ClassName;

        public PanelContentData(Action<PanelContent> content, string title = "", string className = "")
        {
            Content = content;
            Title = title;
            ClassName = className;
        }
    }

    public partial class PanelContent : Panel
    {
        public string Title { get; private set; } = "";

        public string ClassName { get; private set; } = "";

        public bool CanPrevious
        {
            get
            {
                return _historyIndex > 0;
            }
        }

        public bool CanNext
        {
            get
            {
                return _historyIndex < (_contentHistory.Count - 1);
            }
        }

        public Action<PanelContentData> OnPanelContentUpdated { get; set; }

        public PanelContentData? CurrentPanelContentData { get; private set; }

        private readonly List<PanelContentData> _contentHistory = new();

        private int _historyIndex = 0;

        public PanelContent(Sandbox.UI.Panel parent = null) : base(parent)
        {

        }

        public void Reset()
        {
            _contentHistory.Clear();
            _historyIndex = 0;

            DeleteChildren(true);
        }

        public void SetPanelContent(Action<PanelContent> createContent, string title = "", string className = "")
        {
            int historyOffset = _contentHistory.Count - _historyIndex - 1;

            if (historyOffset > 0)
            {
                _contentHistory.RemoveRange(_historyIndex + 1, historyOffset);
            }

            _contentHistory.Add(new PanelContentData(createContent, title, className));
            _historyIndex = _contentHistory.Count - 1;

            UpdatePanelContent();
        }

        public void Previous()
        {
            if (_historyIndex < 1)
            {
                return;
            }

            _historyIndex--;

            UpdatePanelContent();
        }

        public void Next()
        {
            if (_historyIndex > _contentHistory.Count - 2)
            {
                return;
            }

            _historyIndex++;

            UpdatePanelContent();
        }

        public void UpdatePanelContent()
        {
            DeleteChildren(true);

            string oldClassName = CurrentPanelContentData?.ClassName;

            if (!string.IsNullOrEmpty(oldClassName))
            {
                SetClass(oldClassName, false);
            }

            PanelContentData panelContentData = _contentHistory[_historyIndex];

            Title = panelContentData.Title;
            ClassName = panelContentData.ClassName;

            panelContentData.Content(this);

            OnPanelContentUpdated?.Invoke(panelContentData);

            CurrentPanelContentData = panelContentData;

            if (!string.IsNullOrEmpty(panelContentData.ClassName))
            {
                SetClass(panelContentData.ClassName, true);
            }
        }
    }
}
