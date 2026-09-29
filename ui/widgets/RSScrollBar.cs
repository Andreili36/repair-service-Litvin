using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace RepairService.Ui.Widgets
{
    public static class RSScrollBar
    {
        private const string TemplateXaml = @"
<Style xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
       xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
       TargetType=""ScrollBar"">
    <Setter Property=""Width"" Value=""8""/>
    <Setter Property=""Background"" Value=""Transparent""/>
    <Setter Property=""Template"">
        <Setter.Value>
            <ControlTemplate TargetType=""ScrollBar"">
                <Grid Background=""Transparent"">
                    <Track x:Name=""PART_Track"" IsDirectionReversed=""True"">
                        <Track.DecreaseRepeatButton>
                            <RepeatButton Command=""ScrollBar.PageUpCommand""
                                          Background=""Transparent""
                                          BorderThickness=""0""
                                          Focusable=""False""/>
                        </Track.DecreaseRepeatButton>
                        <Track.Thumb>
                            <Thumb>
                                <Thumb.Template>
                                    <ControlTemplate TargetType=""Thumb"">
                                        <Border Background=""#3A3A50""
                                                CornerRadius=""4""
                                                Margin=""2""/>
                                    </ControlTemplate>
                                </Thumb.Template>
                            </Thumb>
                        </Track.Thumb>
                        <Track.IncreaseRepeatButton>
                            <RepeatButton Command=""ScrollBar.PageDownCommand""
                                          Background=""Transparent""
                                          BorderThickness=""0""
                                          Focusable=""False""/>
                        </Track.IncreaseRepeatButton>
                    </Track>
                </Grid>
                <ControlTemplate.Triggers>
                    <Trigger Property=""Orientation"" Value=""Horizontal"">
                        <Setter TargetName=""PART_Track"" Property=""IsDirectionReversed"" Value=""False""/>
                        <Setter Property=""Height"" Value=""8""/>
                        <Setter Property=""Width"" Value=""Auto""/>
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>";

        private static Style? _thin;

        public static Style Thin()
        {
            if (_thin != null) return _thin;

            using var stringReader = new StringReader(TemplateXaml);
            using var xmlReader = XmlReader.Create(stringReader);
            _thin = (Style)XamlReader.Load(xmlReader);
            return _thin;
        }
    }
}