using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace IEEE754Converter.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string _internalCode = string.Empty;
    [ObservableProperty] private string _ieee754Code = string.Empty;
    [ObservableProperty] private string _floatValue = string.Empty;

    [RelayCommand]
    private void InternalToIeee754()
    {
        try
        {
            var hexString = InternalCode.Replace(" ", "");
            
            Ieee754Code = ConvertInternalCodeToIeee754(hexString).ToString("X");
            FloatValue = GetFloatFromHexString(Ieee754Code).ToString("0.0000");
        }
        catch (FormatException e)
        {
            Console.WriteLine($"Błąd: {e.Message}");
        }
    }

    [RelayCommand]
    private void Ieee754ToInternal()
    {
        try
        {
            var hexString = Ieee754Code.Replace(" ", "");

            FloatValue = GetFloatFromHexString(hexString).ToString("0.0000");
            InternalCode = ConvertIeee754ToInternalCode(hexString).ToString("X8");
        }
        catch (FormatException e)
        {
            Console.WriteLine($"Błąd: {e.Message}");
        }

    }

    private static ulong ConvertInternalCodeToIeee754(string hexString)
    {
        var internalInt = uint.Parse(hexString, NumberStyles.AllowHexSpecifier);

        var bits = Convert.ToString(internalInt, 2).PadLeft(32, '0');

        var signBit = bits[..1];
        var exponentBits = bits.Substring(1, 8);
        var mantissaBits = bits[9..];

        var exponent = Convert.ToInt32(exponentBits, 2);
        exponent >>= 1;
        exponent += 127;
        exponentBits = Convert.ToString(exponent, 2).PadLeft(8, '0');

        signBit = signBit == "0" ? "1" : "0";

        var newBits = signBit + exponentBits + mantissaBits;
        return Convert.ToUInt32(newBits, 2);
    }

    private static ulong ConvertIeee754ToInternalCode(string hexString)
    {
        var ieee754Int = uint.Parse(hexString, NumberStyles.AllowHexSpecifier);

        var bits = Convert.ToString(ieee754Int, 2).PadLeft(32, '0');

        var signBit = bits[..1];
        var exponentBits = bits.Substring(1, 8);
        var mantissaBits = bits[9..];

        var exponent = Convert.ToInt32(exponentBits, 2);
        exponent -= 127;
        exponent <<= 1;
        exponentBits = Convert.ToString(exponent, 2).PadLeft(8, '0');

        signBit = signBit == "0" ? "1" : "0";

        var newBits = signBit + exponentBits + mantissaBits;
        return Convert.ToUInt32(newBits, 2);
    }


    private static float GetFloatFromHexString(string hexString)
    {
        var num = uint.Parse(hexString, NumberStyles.AllowHexSpecifier);
        var floatVal = BitConverter.GetBytes(num);
        return BitConverter.ToSingle(floatVal, 0);
    }
}