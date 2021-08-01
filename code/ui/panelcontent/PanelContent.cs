using System;
using System.Collections.Generic;

using Sandbox.UI;

namespace TTTReborn.UI
{
    public struct PanelContentData
    {
        public string Title;
        public Action<PanelContent> Content;

        public PanelContentData(Action<PanelContent> content, string title = "")
        {
            Content = content;
            Title = title;
        }
    }

    public partial class PanelContent : TTTPanel
    {
        public string Title { get; private set; } = "";

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

        private readonly List<PanelContentData> _contentHistory = new();

        private int _historyIndex = 0;

        public PanelContent(Panel parent = null) : base()
        {
            Parent = parent;
        }

        public void Reset()
        {
            _contentHistory.Clear();
            _historyIndex = 0;

            DeleteChildren(true);
        }

        public void SetPanelContent(Action<PanelContent> createContent, string title = "")
        {
            _contentHistory.Add(new PanelContentData(createContent, title));
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

            PanelContentData panelContentData = _contentHistory[_historyIndex];

            Title = panelContentData.Title;

            panelContentData.Content(this);

            OnPanelContentUpdated?.Invoke(panelContentData);
        }
    }
}
