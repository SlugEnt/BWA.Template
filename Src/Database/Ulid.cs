using ByteAether.Ulid;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SlugEnt.BWA.Database;


/// <summary>
/// This is required for models that are based upon the ULID Id property.  It converts ULid to Bytes.
/// </summary>
public class UlidToBytesConverter : ValueConverter<Ulid, byte[]>
{
    private static readonly ConverterMappingHints DefaultHints = new ConverterMappingHints(size: 16);

    /// <summary>
    /// Constructor
    /// </summary>
    public UlidToBytesConverter() : this(DefaultHints) { }


    /// <summary>
    /// Converts the ULID to byte Array and back again.
    /// </summary>
    /// <param name="mappingHints"></param>
    public UlidToBytesConverter(ConverterMappingHints? mappingHints = null)
        : base(
               convertToProviderExpression: x => x.ToByteArray(),
               convertFromProviderExpression: x => Ulid.New(x),
               mappingHints: DefaultHints.With(mappingHints)) { }
}


/// <summary>
/// This is required for models that are based upon the ULID Id property.  It converts ULid to string
/// </summary>
public class UlidToStringConverter : ValueConverter<Ulid, string>
{
    private static readonly ConverterMappingHints DefaultHints = new ConverterMappingHints(size: 26);

    /// <summary>
    /// Constructor
    /// </summary>
    public UlidToStringConverter() : this(null) { }


    /// <summary>
    /// Convert Ulid to string and back again.
    /// </summary>
    /// <param name="mappingHints"></param>
    public UlidToStringConverter(ConverterMappingHints? mappingHints)
        : base(
               convertToProviderExpression: x => x.ToString(null,null),
               convertFromProviderExpression: x => Ulid.Parse(x,null),
               mappingHints: DefaultHints.With(mappingHints)) { }

}
