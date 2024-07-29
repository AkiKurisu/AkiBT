using System;
namespace Kurisu.AkiBT.DSL
{
    public interface ITypeContract
    {
        bool CanConvert(Type inputType, Type expectType);
        object Convert(in object value, Type inputType, Type expectType);
    }
}