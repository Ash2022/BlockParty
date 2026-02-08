namespace AppLovinMax
{
    public abstract class MaxSdkBase
    {
        public enum ConsentFlowUserGeography
    {
        /// <summary>
        /// User's geography is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The user is in GDPR region.
        /// </summary>
        Gdpr,

        /// <summary>
        /// The user is in a non-GDPR region.
        /// </summary>
        Other
    }
    }
}

