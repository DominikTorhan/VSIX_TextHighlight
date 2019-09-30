using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VSIX_TextHightlight
{

  internal class TextHighlightTagger : ITagger<TextHighlightWordTag>
  {

    private readonly ITextView view;
    private readonly ITextBuffer sourceBuffer;
    private readonly ITextSearchService textSearchService;
    private NormalizedSnapshotSpanCollection wordSpans;
    private readonly object updateLock = new object();
    private const int minWordLength = 3;

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public TextHighlightTagger(ITextView view, ITextBuffer sourceBuffer, ITextSearchService textSearchService)
    {

      this.view = view;
      this.sourceBuffer = sourceBuffer;
      this.textSearchService = textSearchService;
      this.wordSpans = new NormalizedSnapshotSpanCollection();
      this.view.Selection.SelectionChanged += Selection_SelectionChanged;
    }

    private void Selection_SelectionChanged(object sender, EventArgs e)
    {
      UpdateWordAdornments();
    }

    private string GetSelection()
    {
      if (view.Selection.IsEmpty) return "";
      var selectionLenght = view.Selection.End.Position - view.Selection.Start.Position;
      if (selectionLenght < minWordLength) return "";
      return sourceBuffer.CurrentSnapshot.GetText(view.Selection.Start.Position, selectionLenght);
    }

    private void UpdateWordAdornments()
    {
      var str = GetSelection();
      if (str == "")
      {
        SynchronousUpdate(new NormalizedSnapshotSpanCollection(new SnapshotSpan()));
        return;
      }
      var findData = new FindData(str, sourceBuffer.CurrentSnapshot);
      findData.FindOptions = FindOptions.None | FindOptions.MatchCase;
      var wordSpans = new List<SnapshotSpan>();
      wordSpans.AddRange(textSearchService.FindAll(findData));
      SynchronousUpdate(new NormalizedSnapshotSpanCollection(wordSpans));
    }

    private void SynchronousUpdate(NormalizedSnapshotSpanCollection newSpans)
    {
      lock (updateLock)
      {
        wordSpans = newSpans;
        var snapshotSpan = new SnapshotSpan(sourceBuffer.CurrentSnapshot, 0, sourceBuffer.CurrentSnapshot.Length);
        TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(snapshotSpan));
      }
    }

    public IEnumerable<ITagSpan<TextHighlightWordTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
      if (spans.Count == 0 || this.wordSpans.Count == 0) yield break;
      var wordSpans = GetSpanCollection(spans);
      foreach (SnapshotSpan span in NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans))
      {
        yield return new TagSpan<TextHighlightWordTag>(span, new TextHighlightWordTag());
      }
    }

    private NormalizedSnapshotSpanCollection GetSpanCollection(NormalizedSnapshotSpanCollection spans)
    {
      if (spans[0].Snapshot == wordSpans[0].Snapshot) return wordSpans;
      var spansNew = wordSpans.Select(span => span.TranslateTo(spans[0].Snapshot, SpanTrackingMode.EdgeExclusive));
      return new NormalizedSnapshotSpanCollection(spansNew);

    }

  }
}
