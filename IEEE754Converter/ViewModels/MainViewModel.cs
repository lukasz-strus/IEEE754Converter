using System.Globalization;
using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace IEEE754Converter.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string _internalCode = string.Empty;
    [ObservableProperty] private string _ieee754Code = string.Empty;

    [RelayCommand]
    private void InternalToIeee754()
    {
        var internalCode = uint.Parse(InternalCode, NumberStyles.HexNumber);
        var ieee754Code = ConvertInternalToIEEE754(internalCode);
        Ieee754Code = ieee754Code.ToString("X8");
        }

    [RelayCommand]
    private void Ieee754ToInternal()
    {
        var ieee754Code = uint.Parse(Ieee754Code, NumberStyles.HexNumber);
        var internalCode = ConvertIEEE754ToInternal(ieee754Code);
        InternalCode = internalCode.ToString("X8");
    }

    private uint ConvertInternalToIEEE754(uint internalCode)
    {
        // Rozdzielenie kodu wewnętrznego na części
        int sign = (int)((internalCode & 0x80000000) >> 31);
        int exponentInternal = (int)((internalCode & 0x7F800000) >> 23);
        uint mantissa = internalCode & 0x007FFFFF;

        // Przekształcenie wykładnika
        int exponentIEEE = exponentInternal - 127 + 127;

        // Złożenie wartości IEEE 754
        uint ieee754 = (uint)((sign << 31) | (exponentIEEE << 23) | mantissa);

        return ieee754;
    }

    private uint ConvertIEEE754ToInternal(uint ieee754Code)
    {
        // Rozdzielenie kodu IEEE 754 na części
        int sign = (int)((ieee754Code & 0x80000000) >> 31);
        int exponentIEEE = (int)((ieee754Code & 0x7F800000) >> 23);
        uint mantissa = ieee754Code & 0x007FFFFF;

        // Przekształcenie wykładnika
        int exponentInternal = exponentIEEE - 127 + 127;

        // Złożenie wartości wewnętrznej
        uint internalCode = (uint)((sign << 31) | (exponentInternal << 23) | mantissa);

        return internalCode;
    }
}