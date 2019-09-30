using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.Composition;
using System.Windows.Media;

namespace VSIX_TextHightlight
{
  [Export(typeof(EditorFormatDefinition))]
  [Name("MarkerFormatDefinition/HighlightWordFormatDefinition")]
  [UserVisible(true)]
  internal class TextHighlightFormatDefinition : MarkerFormatDefinition
  {

    public TextHighlightFormatDefinition()
    {
      this.BackgroundColor = Colors.LightBlue;
      this.ForegroundColor = Colors.DarkBlue;
      this.DisplayName = "TextHighlight";
      this.ZOrder = 5;
    }

  }
}
