using System;

namespace Sammak.Windsor.Plumbing
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DoNotMapAttribute : Attribute
    {
    }
}