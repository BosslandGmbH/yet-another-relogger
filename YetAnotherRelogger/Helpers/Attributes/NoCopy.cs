using System;

namespace YetAnotherRelogger.Helpers.Attributes
{
    /// <summary>
    ///     This attribute class is for when we are doing a Deep Copy and do not want to copy a specific Member, such as when
    ///     there is a circular (parent, child, parent) reference
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class NoCopy : Attribute
    {
    }
}