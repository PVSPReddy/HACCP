using System;

namespace HACCP.Core
{
    public interface IDescriptor
    {
        object NativeDescriptor { get; }
        Guid ID { get; }
        string Name { get; }
    }
}