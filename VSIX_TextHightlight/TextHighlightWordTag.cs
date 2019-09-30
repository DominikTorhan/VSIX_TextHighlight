using Microsoft.VisualStudio.Text.Tagging;

namespace VSIX_TextHightlight
{
  internal class TextHighlightWordTag : TextMarkerTag
  {
    public TextHighlightWordTag() : base("MarkerFormatDefinition/HighlightWordFormatDefinition") { }
  }
}
