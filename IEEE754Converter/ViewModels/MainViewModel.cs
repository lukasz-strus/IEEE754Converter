using System.Globalization;
using System.Windows;
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
            var inputString = InternalCode.Replace(" ", "");
            if (inputString == "00000000")
            {
                Ieee754Code = "00000000";
                FloatValue = "0.0000";
                return;
            }

            var isInputBinary = IsBinary(inputString);

            var convertedValue = Convert.ToUInt64(inputString, isInputBinary ? 2 : 16);

            var ieee754Value = ConvertInternalCodeToIeee754(convertedValue.ToString("X"), out var newBits);

            Ieee754Code = isInputBinary 
                ? newBits
                : ieee754Value.ToString("X");

            FloatValue = GetFloatFromHexString(ieee754Value.ToString("X")).ToString("0.0000");
        }
        catch (Exception)
        {
            ShowErrorMsg();
        }
    }

    [RelayCommand]
    private void Ieee754ToInternal()
    {
        try
        {
            var inputString = Ieee754Code.Replace(" ", "");
            if (inputString == "00000000")
            {
                InternalCode = "00000000";
                FloatValue = "0.0000";
                return;
            }
            var isInputBinary = IsBinary(inputString); 

            var convertedValue = Convert.ToUInt64(inputString, isInputBinary ? 2 : 16);

            var internalValue = ConvertIeee754ToInternalCode(convertedValue.ToString("X"), out var newBits);
            InternalCode = isInputBinary 
                ? newBits
                : internalValue.ToString("X8");
            FloatValue = GetFloatFromHexString(convertedValue.ToString("X")).ToString("0.0000");
        }
        catch (Exception)
        {
            ShowErrorMsg();
        }

    }

    private static ulong ConvertInternalCodeToIeee754(string hexString, out string newBits)
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

        newBits = signBit + exponentBits + mantissaBits;
        return Convert.ToUInt32(newBits, 2);
    }

    private static ulong ConvertIeee754ToInternalCode(string hexString, out string newBits)
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

        newBits = signBit + exponentBits + mantissaBits;
        return Convert.ToUInt32(newBits, 2);
    }


    private static float GetFloatFromHexString(string hexString)
    {
        var num = uint.Parse(hexString, NumberStyles.AllowHexSpecifier);
        var floatVal = BitConverter.GetBytes(num);
        return BitConverter.ToSingle(floatVal, 0);
    }

    private static bool IsBinary(string input) => input.All(c => c is '0' or '1');

    private static void ShowErrorMsg() => MessageBox.Show("Nieprawidłowa wartość!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
}