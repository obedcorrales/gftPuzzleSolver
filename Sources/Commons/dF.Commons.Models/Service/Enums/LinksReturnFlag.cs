namespace dF.Commons.Models.Service.Enums
{
    public sealed class LinksReturnFlag
    {
        #region Enums
        public static readonly LinksReturnFlag NoPreference = new LinksReturnFlag(0);
        public static readonly LinksReturnFlag PreferLinkID = new LinksReturnFlag(1);
        public static readonly LinksReturnFlag OnlyLinkID = new LinksReturnFlag(2);
        public static readonly LinksReturnFlag PreferHref = new LinksReturnFlag(3);
        public static readonly LinksReturnFlag OnlyHref = new LinksReturnFlag(4);
        #endregion

        #region Model
        private readonly int _flag;

        private LinksReturnFlag(int flag)
        {
            _flag = flag;
        }

        public int Flag { get { return _flag; } }

        public override string ToString()
        {
            return _flag.ToString();
        }

        public static implicit operator int(LinksReturnFlag flag)
        {
            return flag.Flag;
        }

        public static implicit operator string(LinksReturnFlag flag)
        {
            return flag.Flag.ToString();
        }

        public static implicit operator LinksReturnFlag(int flag)
        {
            switch (flag)
            {
                case 1:
                    return PreferLinkID;
                case 2:
                    return OnlyLinkID;
                case 3:
                    return PreferHref;
                case 4:
                    return OnlyHref;
                default:
                    return NoPreference;
            }
        }
        #endregion
    }
}
