namespace dF.Commons.Security.Constants
{
    public enum Actions : byte
    {
        None = 0,
        Read = 1,
        ReadAll = 2,
        Add = 4,
        Remove = 8,
        Update = 16,
        Patch = 32
    }

    public enum Permissions : byte
    {
        None = 0,
        Deny = 1,
        Allow = 2
    }
}
