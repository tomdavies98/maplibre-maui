using Org.Maplibre.Android.Style.Layers;
using JavaBoolean = Java.Lang.Boolean;
using JavaFloat = Java.Lang.Float;

namespace Maui.MapLibre.Handlers;

/// <summary>
/// Converts style paint dictionaries to MapLibre <see cref="PropertyValue"/> instances via <see cref="PropertyFactory"/>.
/// Raw PropertyValue(name, boxed value) does not apply colors correctly on Android (often renders black).
/// </summary>
internal static class LayerPaintPropertyConverter
{
    public static PropertyValue[] ToPropertyValues(IDictionary<string, object?> properties)
    {
        var list = new List<PropertyValue>(properties.Count);
        foreach (var entry in properties)
        {
            if (entry.Value is null)
                continue;

            var propertyValue = TryConvert(entry.Key, entry.Value);
            if (propertyValue is not null)
                list.Add(propertyValue);
        }

        return list.ToArray();
    }

    private static PropertyValue? TryConvert(string key, object value)
    {
        return key switch
        {
            "fill-color" when value is string fillColor => PropertyFactory.FillColor(fillColor),
            "fill-outline-color" when value is string outlineColor => PropertyFactory.FillOutlineColor(outlineColor),
            "fill-opacity" => PropertyFactory.FillOpacity(ToJavaFloat(value)),
            "fill-antialias" when value is bool antialias => PropertyFactory.FillAntialias(ToJavaBoolean(antialias)),
            "line-color" when value is string lineColor => PropertyFactory.LineColor(lineColor),
            "line-width" => PropertyFactory.LineWidth(ToJavaFloat(value)),
            "line-opacity" => PropertyFactory.LineOpacity(ToJavaFloat(value)),
            "circle-color" when value is string circleColor => PropertyFactory.CircleColor(circleColor),
            "circle-radius" => PropertyFactory.CircleRadius(ToJavaFloat(value)),
            "circle-opacity" => PropertyFactory.CircleOpacity(ToJavaFloat(value)),
            "circle-stroke-color" when value is string strokeColor => PropertyFactory.CircleStrokeColor(strokeColor),
            "circle-stroke-width" => PropertyFactory.CircleStrokeWidth(ToJavaFloat(value)),
            "circle-stroke-opacity" => PropertyFactory.CircleStrokeOpacity(ToJavaFloat(value)),
            _ => null,
        };
    }

    private static float ToFloat(object value) =>
        value switch
        {
            float f => f,
            double d => (float)d,
            int i => i,
            long l => l,
            _ => Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture),
        };

    private static JavaFloat ToJavaFloat(object value) => (JavaFloat)ToFloat(value);

    private static JavaBoolean ToJavaBoolean(bool value) => (JavaBoolean)value;
}
