using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Composition;

namespace VSIX_TextHightlight
{
  [Export(typeof(IViewTaggerProvider))]
  [ContentType("text")]
  [TagType(typeof(TextMarkerTag))]
  internal class TextHighlightTaggerProvider : IViewTaggerProvider
  {

    [Import]
    internal ITextSearchService TextSearchService { get; set; }

    [Import]
    internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
    {
      if (textView.TextBuffer != buffer) return null;
      return new TextHighlightTagger(textView, buffer, TextSearchService) as ITagger<T>;
    }
  }
}
