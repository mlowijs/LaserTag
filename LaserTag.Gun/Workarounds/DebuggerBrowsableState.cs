/// <summary>
/// This fixes a bug in VS2015 with .NET MF 4.3.
/// </summary>
namespace System.Diagnostics
{
    public enum DebuggerBrowsableState
    {
        Never = 0,
        Collapsed = 2,
        RootHidden = 3
    }
}
