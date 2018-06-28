namespace dF.Commons.Models.Global.Enums
{
    public enum ErrorType
    {
        Empty = 0,

        DuplicateEntry = 1,
        EntityCreatedWithErrors = 2,
        EntityLocked = 3,
        EntityNotFound = 4,
        CommitFailure = 5,

        // »»» https://msdn.microsoft.com/en-us/library/z4c5tckx(v=vs.110).aspx
        IndexOutOfRange = 100,
        NullReference = 101,
        AccessViolation = 102,
        InvalidOperation = 103,
        Argument = 104,
        ArgumentNull = 105,
        ArgumentOutOfRange = 106,

        DivideByZero = 107,
        KeyNotFound = 108,

        // »»» https://msdn.microsoft.com/en-us/library/system.systemexception.aspx?f=255&MSPPError=-2147217396
        // System
        Arithmetic = 200,
        ArrayTypeMismatch = 201,
        BadImageFormat = 202,
        Format = 203,
        NotImplemented = 204,
        NotSupported = 205,
        OperationCanceled = 206,
        OutOfMemory = 207,
        StackOverflow = 208,
        Timeout = 209,
        UnauthorizedAccess = 210,

        // System.Runtime
        Serialization = 1000,

        // System.Security
        Authentication = 1101,
        Security = 1102,
        Verification = 1103,

        // System.ServiceModel
        Communication = 1200,
        QuotaExceeded = 1201
    }
}
